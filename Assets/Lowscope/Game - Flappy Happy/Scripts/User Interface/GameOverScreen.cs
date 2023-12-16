using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/FlappyHappy - Game Over Screen")]
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField] private float targetY;
        [SerializeField] private float hiddenY;
        [SerializeField] private float moveTime = 0.5f;
        [SerializeField] private float displayPauseTime = 0.5f;

        [SerializeField] private Core core;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Text currentScoreText;
        [SerializeField] private Text bestScoreText;

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
            gameObject.SetActive(false);
        }

        private void OnScoreChanged(int score)
        {
            currentScore = score;
        }

        private void OnGameOver()
        {
            transform.position = new Vector3(transform.position.x, hiddenY, 0);
            gameObject.SetActive(true);

            if (moveTime > 0)
            {
                StartCoroutine(MoveUp());
            }
            else
            {
                rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, targetY, 0);
            }

            currentScoreText.text = $"SCORE {currentScore.ToString()}";

            bool newRecord = currentScore > bestScore;

            if (newRecord)
            {
                bestScore = currentScore;
                PlayerPrefs.SetInt("Flappy_BestScore", bestScore);
            }

            bestScoreText.text = $"{(newRecord ? "NEW! -> " : "")}{bestScore.ToString()} BEST";
        }

        private IEnumerator MoveUp()
        {
            yield return new WaitForSeconds(displayPauseTime);

            float t = 0;

            while (t < moveTime)
            {
                t += Time.deltaTime;
                rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, Mathf.Lerp(hiddenY, targetY, Easing.Quadratic.InOut(t / moveTime)), 0);
                yield return null;
            }
        }
    }
}