using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/FlappyHappy - Game Button")]
    public class GameButton : MonoBehaviour
    {
        private enum Function { Restart, Start, Rate, Make, Leaderboard, LoadScene, PauseGame }

        [SerializeField] private Core core;
        [SerializeField] private Button button;
        [SerializeField] private Function buttonFunction;
        [SerializeField] private string sceneToLoad;

        private void Start()
        {
            button.onClick.AddListener(OnClickButton);
        }

        // Gets called upon component add
        private void Reset()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }

        private void OnClickButton()
        {
            switch (buttonFunction)
            {
                case Function.Restart:
                    core.RestartGame();
                    break;
                case Function.Start:
                    core.StartGame();
                    break;
                case Function.Rate:
                    Application.OpenURL("https://low-scope.com/plugins/");
                    break;
                case Function.Make:
                    Application.OpenURL("https://low-scope.com/plugins/");
                    break;
                case Function.Leaderboard:
                    Application.OpenURL("https://low-scope.com/plugins/");
                    break;
                case Function.LoadScene:
                    SceneManager.LoadScene(sceneToLoad);
                    break;
                case Function.PauseGame:
                    core.PauseGame(true);
                    break;
            }
        }
    }
}