using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/Arcade Machine/FlappyHappy - Arcade Machine")]
    public class ArcadeMachine : MonoBehaviour
    {
#if UNITY_EDITOR
        // Clear values in case game was restarted in editor
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void ResetDictionary()
        {
            if (gameHidePositions != null)
                gameHidePositions.Clear();
        }
#endif
        
        private static HashSet<Vector3> gameHidePositions = new HashSet<Vector3>();
        
        public enum AntiAliasing { None = 1, TwoTimes = 2, FourTimes = 4, EightTimes = 8 }
        public enum Events { OnGameOver, OnScore, OnPlayDuration, OnPressCount, OnStartGame, OnTotalPoints, OnTotalPlayDuration }
        public enum Condition { HigherThen, LowerThen, Equal, HigherEquals, LowerEquals }

        [Serializable]
        public class Event
        {
            public Events events;
            public Condition condition;
            public int conditionValue;
            public bool callOnce = true;
            public UnityEventInt action;

            private bool hasBeenCalled;

            public void Call(int value)
            {
                if (callOnce && hasBeenCalled)
                    return;

                action.Invoke(value);

                hasBeenCalled = true;
            }
        }

        [Serializable]
        public class SaveData
        {
            public int GameOverCount;
            public int PlayDuration;
            public int PressCount;
            public int StartGameCount;
            public int TotalPoints;
        }

        [Header("Configuration")]
        [SerializeField, Range(8, 2048)] private int resolution = 1024;
        [SerializeField] private AntiAliasing antiAliasing = AntiAliasing.TwoTimes;
        [SerializeField] private FilterMode filterMode = FilterMode.Bilinear;

        [SerializeField] private float buttonActivationRadius = 2;
        [SerializeField] private Vector3 gameHidePosition = new Vector3(0, -10000, 0);
        [SerializeField] private bool useOnMouseDown = true;
        [SerializeField] private string[] cullingTriggerDetectionTags = new string[2] { "Player", "MainCamera" };

        [Tooltip("Change this if you want scoring to be done individually for each machine.")]
        [SerializeField] private string saveId = "LS_HF_SaveData";

        [Header("References")]
        [SerializeField] private Core core;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private GameObject arcadeGameObjects;
        [SerializeField] private ArcadeMachineButton arcadeButton;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private ArcadeCullingTrigger cullingTrigger;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private MeshRenderer screen;
        [SerializeField] private Light screenLight;
        [SerializeField] private CanvasScaler[] gameCanvasScalers;

        [Header("Machine Callbacks")]
        [SerializeField] private Event[] events;

        [Header("Screen Light - Multiplied by scale")]
        [SerializeField] private float lightIntensity = 5.3f;
        [SerializeField] private float lightRange = 2.59f;

        [Header("Debug")]
        [SerializeField] private SaveData saveData;

        private RenderTexture renderTexture;
        private DateTime stopDate;
        private DateTime startDate;
        
        private bool visibleForCamera;
        private int withinCollisionCount;

        #region Basic Frustum Culling

        private void OnBecameVisible()
        {
            if (!visibleForCamera)
            {
                visibleForCamera = true;
                UpdateCameraState();
            }
        }

        private void OnBecameInvisible()
        {
            if (visibleForCamera)
            {
                visibleForCamera = false;
                UpdateCameraState();
            }
        }

        private void UpdateCameraState()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            if (arcadeGameObjects != null)
                arcadeGameObjects.gameObject.SetActive(visibleForCamera && withinCollisionCount > 0);
        }

        #endregion

#if UNITY_EDITOR
        
        private void Reset()
        {
            gameCamera = GetComponentInChildren<Camera>(true);
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                if (renderTexture == null)
                    return;

                if (renderTexture.width != resolution || renderTexture.height != resolution
                    || renderTexture.antiAliasing != (int)antiAliasing || renderTexture.filterMode != filterMode)
                {
                    CreateApplyRenderTexture();
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(arcadeButton.transform.position, buttonActivationRadius);

            float scaleMagnitude = transform.lossyScale.magnitude;

            float newIntensity = lightIntensity * scaleMagnitude;
            float newRange = lightRange * scaleMagnitude;
            
            if (!Mathf.Approximately(screenLight.intensity, newIntensity))
                screenLight.intensity = newIntensity;
            
            if (!Mathf.Approximately(screenLight.range, newRange))
                screenLight.range = newRange;
        }

#endif

        private void Awake()
        {
            // In case the game hide position is already taken, move it down until there is space.
            while (gameHidePositions.Contains(gameHidePosition))
                gameHidePosition -= new Vector3(50, 0, 0);
            
            gameHidePositions.Add(gameHidePosition);
            
            if (core != null)
                core.SetCanBePaused(false);

            arcadeGameObjects.transform.SetParent(null);
            arcadeGameObjects.transform.position = gameHidePosition;
            arcadeGameObjects.transform.rotation = Quaternion.Euler(0, 0, 0);
            arcadeGameObjects.transform.localScale = new Vector3(1, 1, 1);
            arcadeGameObjects.SetActive(true);
            visibleForCamera = true;

            cullingTrigger.Configure(cullingTriggerDetectionTags);
            cullingTrigger.OnCountUpdate.AddListener(OnCullingTriggerChanged);
            arcadeButton.Configure(buttonActivationRadius);

            // Load save data on Awake.
            string jsonString = PlayerPrefs.GetString("LS_HF_SaveData", "");
            saveData = string.IsNullOrEmpty(jsonString) ? new SaveData()
                : JsonUtility.FromJson<SaveData>(jsonString);

            CreateApplyRenderTexture();

            core.ListenToGameStart(OnGameStart);
            core.ListenToGameOver(OnGameOver);
            core.ListenToGameScoreChange(OnGameScoreChange);
            core.ListenToGameTap(OnGameTap);
        }

        private void CreateApplyRenderTexture()
        {
            renderTexture = new RenderTexture(resolution, resolution, 16, RenderTextureFormat.ARGB32, 0);
            renderTexture.antiAliasing = (int)antiAliasing;
            renderTexture.filterMode = filterMode;

            gameCamera.targetTexture = renderTexture;
            screen.material.SetTexture("_BaseMap", renderTexture);
            screen.material.SetTexture("_MainTex", renderTexture);

            for (int i = 0; i < gameCanvasScalers.Length; i++)
            {
                gameCanvasScalers[i].scaleFactor = (resolution / (float)1024) * 1.5f;
            }
        }

        private void OnDestroy()
        {
            // Store save data on Destroy.
            PlayerPrefs.SetString("LS_HF_SaveData", JsonUtility.ToJson(saveData));
        }

        private void OnGameTap()
        {
            saveData.PressCount++;
            CallEvent(Events.OnPressCount, saveData.PressCount);
        }

        private void OnGameScoreChange(int score)
        {
            saveData.TotalPoints++;
            CallEvent(Events.OnTotalPoints, saveData.TotalPoints);

            CallEvent(Events.OnScore, score);
        }

        private void OnGameStart()
        {
            saveData.StartGameCount++;
            CallEvent(Events.OnStartGame, saveData.StartGameCount);

            musicSource.time = 0;
            musicSource.Play();

            startDate = DateTime.Now;
        }

        private void OnGameOver()
        {
            saveData.GameOverCount++;
            CallEvent(Events.OnGameOver, saveData.GameOverCount);

            musicSource.Stop();
            stopDate = DateTime.Now;

            int addedTimeDuration = Mathf.RoundToInt((float)(startDate - stopDate).Duration().TotalSeconds);
            saveData.PlayDuration += addedTimeDuration;
            CallEvent(Events.OnPlayDuration, addedTimeDuration);
            CallEvent(Events.OnTotalPlayDuration, saveData.PlayDuration);
        }

        private void CallEvent(Events checkEvent, int count)
        {
            int eventCount = events.Length;
            for (int i = 0; i < eventCount; i++)
            {
                if (events[i].events == checkEvent)
                {
                    switch (events[i].condition)
                    {
                        case Condition.HigherThen:
                            if (count > events[i].conditionValue)
                                events[i].Call(count);

                            break;
                        case Condition.LowerThen:
                            if (count < events[i].conditionValue)
                                events[i].Call(count);

                            break;
                        case Condition.Equal:
                            if (count == events[i].conditionValue)
                                events[i].Call(count);

                            break;
                        case Condition.HigherEquals:
                            if (count >= events[i].conditionValue)
                                events[i].Call(count);

                            break;
                        case Condition.LowerEquals:
                            if (count <= events[i].conditionValue)
                                events[i].Call(count);

                            break;
                    }
                }
            }
        }

        private void OnCullingTriggerChanged(int count)
        {
            withinCollisionCount = count;
            UpdateCameraState();
        }

        public void PressButton(bool comesFromMouseDown = false)
        {
            if (!useOnMouseDown && comesFromMouseDown)
                return;

            arcadeButton.DisplayButtonAnimation();

            if ((DateTime.Now - stopDate).TotalSeconds < 0.5f)
                return;

            core.Tap();

            if (core.IsGameOver())
            {
                core.RestartGame();
            }
        }
    }
}