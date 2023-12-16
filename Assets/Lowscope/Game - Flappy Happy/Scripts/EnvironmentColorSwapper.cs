using UnityEngine;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/FlappyHappy - Environment Color Swapper")]
    public class EnvironmentColorSwapper : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Core core;
        [SerializeField] private Material birdBody;
        [SerializeField] private Material[] environmentMaterials;

        [Header("Configuration")]
        [SerializeField] private Color[] birdColors;
        [SerializeField] private Texture2D[] environmentTextures;
        [SerializeField] private string textureParameter = "_MainTex";

#if UNITY_EDITOR

        [Header("Editor Only")]
        [SerializeField] private bool inspectConfiguration;
        [SerializeField] private int indexToInspect;

        [SerializeField, HideInInspector] private float lastBirdHue;

        // OnValidate is great for editor-time debugging.
        // Each time a value changes in the inspector, it gets called. 
        // It also gets called when the game starts and stops.

        private void OnValidate()
        {
            if (inspectConfiguration)
            {
                ChangeEnvironmentTexture(indexToInspect, true);
            }
        }

#endif

        private void Start()
        {
            if (core != null)
            {
                // Call OnGameRestart when the Core script gets notified that it has to restart
                core.ListenToGameRestart(OnGameRestart);
            }

            ChangeEnvironmentTexture();
        }

        private void OnGameRestart()
        {
            ChangeEnvironmentTexture();
        }

        private void ChangeEnvironmentTexture(int index = -1, bool onValidate = false)
        {
            int totalTextures = environmentTextures.Length;
            if (totalTextures == 0)
            {
                return;
            }

            // Teritary operator. What this means is, if index isn't -1, then choose the first option.
            // Else choose index. (state)? ifTrue : ifFalse;

            int configurationIndex = (index == -1) ? Random.Range(0, totalTextures) : index;
            var getRandomTexture = environmentTextures[configurationIndex];

            foreach (var item in environmentMaterials)
            {
                item.SetTexture(textureParameter, getRandomTexture);
            }

            birdBody.color = birdColors[configurationIndex];
        }
    }
}