using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/FlappyHappy - Exit On Back Button")]
    public class ExitOnBackButton : MonoBehaviour
    {
        [SerializeField] private bool exitApplication;
        [SerializeField] private bool exitToScene;
        [SerializeField] private string sceneName;

        void Update()
        {
            // Reacts to back button
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (exitApplication)
                {
                    Application.Quit();
                }
                else if (exitToScene)
                {
                    SceneManager.LoadScene(sceneName);
                }
            }
        }
    }
}