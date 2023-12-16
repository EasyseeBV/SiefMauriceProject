using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/Arcade Machine/FlappyHappy - Arcade GameOver Screen")]
    public class ArcadeGameOverScreen : MonoBehaviour
    {
        [SerializeField] private Core core;

        [SerializeField] private Text gameOverText;
        [SerializeField] private Text retryText;

        [SerializeField] private float retryTextFlickerInterval = 1f;
        [SerializeField] private int retryTextFlickerCount = 3000;

        private int currentScore;
        private int bestScore;

        private void Start()
        {
            core.ListenToGameOver(OnGameOver);
            core.ListenToGameScoreChange(OnScoreChanged);
            core.ListenToGameRestart(OnGameRestart);

            bestScore = PlayerPrefs.GetInt("Flappy_BestScore", 0);

            gameObject.SetActive(false);
        }

        private void OnGameRestart()
        {
            currentScore = 0;
            gameObject.SetActive(false);
        }

        private void OnScoreChanged(int score)
        {
            currentScore = score;
        }

        private void OnGameOver()
        {
            gameObject.SetActive(true);

            gameOverText.text = currentScore.ToString();
        }

        private void OnEnable()
        {
            if (retryTextFlickerInterval > 0)
            {
                StartCoroutine(FlickerTextCoroutine(retryText, retryTextFlickerInterval, retryTextFlickerCount));
            }
        }

        // Coroutine gets stopped automatically on disable.
        private IEnumerator FlickerTextCoroutine(Text text, float interval, int count)
        {
            // Cache the WaitForSeconds to reduce garbage collection
            var waitSecond = new WaitForSeconds(interval);
            int totalFlickers = count;

            while (totalFlickers > 0)
            {
                yield return waitSecond;
                text.enabled = !text.enabled;
                totalFlickers--;
            }

            text.enabled = true;
        }
    }
}