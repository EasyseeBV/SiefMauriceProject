using UnityEngine;
using UnityEngine.UI;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/FlappyHappy - Pause Screen")]
    public class PauseScreen : MonoBehaviour
    {
        [SerializeField] private Core core;
        [SerializeField] private GameObject content;
        [SerializeField] private Button button;

        private void Start()
        {
            core.ListenToGamePause(OnGamePause);
            button.onClick.AddListener(OnPressScreen);
        }

        private void OnPressScreen()
        {
            core.PauseGame(false);
        }

        private void OnGamePause(bool paused)
        {
            content.SetActive(paused);
            gameObject.SetActive(paused);
        }
    }
}