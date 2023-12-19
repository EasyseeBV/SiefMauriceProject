using UnityEngine;
using UnityEngine.Events;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/FlappyHappy - Core")]
    public class Core : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Editor Only")]
        [SerializeField] private bool pauseInEditor;
#endif

        private UnityEvent onGameStart = new UnityEvent();
        private UnityEvent onGameRestart = new UnityEvent();
        private UnityEvent onGameOver = new UnityEvent();
        private UnityEventInt onGameScoreChanged = new UnityEventInt();
        private UnityEvent onGameTap = new UnityEvent();
        private UnityEventBool onGamePause = new UnityEventBool();

        private bool gameStarted;
        private bool gameOver;
        private int score;

        public void ListenToGameStart(UnityAction listener) => onGameStart.AddListener(listener);
        public void ListenToGameOver(UnityAction listener) => onGameOver.AddListener(listener);
        public void ListenToGameRestart(UnityAction listener) => onGameRestart.AddListener(listener);
        public void ListenToGamePause(UnityAction<bool> listener) => onGamePause.AddListener(listener);
        public void ListenToGameTap(UnityAction listener) => onGameTap.AddListener(listener);
        public void ListenToGameScoreChange(UnityAction<int> listener) => onGameScoreChanged.AddListener(listener);

        private bool canPause = true;
        
        public void SetCanBePaused(bool state)
        {
            canPause = state;
        }
        
        public void PauseGame(bool pause)
        {
            if (!canPause) 
                return;
            
            Time.timeScale = (pause) ? 0 : 1;
            onGamePause.Invoke(pause);
        }

        public bool IsGameOver()
        {
            return gameOver;
        }

        public void StartGame()
        {
            gameStarted = true;
            onGameStart.Invoke();
        }

        public void RestartGame()
        {
            onGameRestart.Invoke();
            gameStarted = false;
            gameOver = false;
            score = 0;
        }

        public void GameOver()
        {
            onGameOver.Invoke();
            gameOver = true;
        }

        public float GetSpeed()
        {
            return gameOver ? 0 : 1;
        }

        public void IncrementScore()
        {
            score++;
            if (score >= 20)
            {
                GameManager.Instance.LevelFinished();
            }
            onGameScoreChanged.Invoke(score);
        }

        public void Tap()
        {
            if (gameOver)
            {
                RestartGame();
                return;
            }

            if (!gameStarted)
            {
                StartGame();
            }

            onGameTap.Invoke();
        }

        private void OnApplicationFocus(bool focus)
        {
#if UNITY_EDITOR
            if (!pauseInEditor)
                return;
#endif

            if (!focus)
                PauseGame(true);
        }

        private void OnApplicationPause(bool pause)
        {
#if UNITY_EDITOR
            if (!pauseInEditor)
                return;
#endif

            if (pause)
                PauseGame(true);
        }
    }
}