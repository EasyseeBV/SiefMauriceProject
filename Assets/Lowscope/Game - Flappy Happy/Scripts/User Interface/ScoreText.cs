using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/FlappyHappy - Score Text")]
    public class ScoreText : MonoBehaviour
    {
        [SerializeField] private Core core;

        [SerializeField] private Text text;

        [SerializeField] private float bounceTime = 0.5f;
        [SerializeField] private float punchSize = 1.5f;

        private void Start()
        {
            core.ListenToGameScoreChange(OnScoreChange);
            core.ListenToGameOver(OnGameOver);
            core.ListenToGameStart(OnGameStart);
            core.ListenToGameRestart(OnGameRestart);
        }

        private void OnGameRestart()
        {
            text.text = "0";
        }

        private void OnGameStart()
        {
            text.text = "0";
        }

        private void OnGameOver()
        {
            text.text = "GAME OVER";
        }

        private void OnScoreChange(int score)
        {
            text.text = score.ToString();
            StartCoroutine(PunchScale());
        }

        private IEnumerator PunchScale()
        {
            float t = 0;
            Vector3 baseScale = transform.localScale;
            Vector3 punchScale = transform.localScale * punchSize;

            while (t < bounceTime)
            {
                t += Time.deltaTime;
                transform.localScale = Vector3.Lerp(punchScale, baseScale, t / bounceTime);
                yield return null;
            }
        }
    }
}