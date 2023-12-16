using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/FlappyHappy - Random Camera Angle")]
    public class RandomCameraAngle : MonoBehaviour
    {
        [Serializable]
        public class CameraAngle
        {
            public Vector3 position;
            public Vector3 rotation;
        }

        [SerializeField] private Core core;

        [SerializeField]
        private List<CameraAngle> cameraAngles = new List<CameraAngle>();

        private void Start()
        {
            core.ListenToGameRestart(OnGameRestart);
        }

        private void OnGameRestart()
        {
            ApplyRandomCameraAngle();
        }

        private void ApplyRandomCameraAngle()
        {
            CameraAngle randomAngle = cameraAngles[Random.Range(0, cameraAngles.Count)];

            transform.localPosition = randomAngle.position;
            transform.rotation = Quaternion.Euler(randomAngle.rotation);
        }

#if UNITY_EDITOR

        [SerializeField] private bool storeCurrentCameraAngle;

        private void OnValidate()
        {
            if (storeCurrentCameraAngle)
            {
                storeCurrentCameraAngle = false;
            }
            else
            {
                return;
            }

            cameraAngles.Add(new CameraAngle
            {
                position = transform.position,
                rotation = transform.rotation.eulerAngles
            });
        }

#endif
    }
}