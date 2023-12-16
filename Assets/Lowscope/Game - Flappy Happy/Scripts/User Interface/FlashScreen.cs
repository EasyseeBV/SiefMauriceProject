using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/FlappyHappy - Flash Screen")]
    public class FlashScreen : MonoBehaviour
    {
        [SerializeField] private Core core;
        [SerializeField] private Image image;
        [SerializeField] private float flashTime = 0.5f;

        private void Start()
        {
            core.ListenToGameOver(OnGameOver);
        }

        private void OnEnable()
        {
            image.enabled = false;
        }

        private void OnGameOver()
        {
            StartCoroutine(FlashImage());

            IEnumerator FlashImage()
            {
                Color c = new Color(1, 1, 1, 0);
                image.color = c;
                image.enabled = true;

                float t = 0;

                while (t < flashTime)
                {
                    yield return null;
                    t += Time.deltaTime;
                    c.a = Mathf.Lerp(1, 0, t / flashTime);
                    image.color = c;
                }

                image.enabled = false;
            }
        }
    }
}