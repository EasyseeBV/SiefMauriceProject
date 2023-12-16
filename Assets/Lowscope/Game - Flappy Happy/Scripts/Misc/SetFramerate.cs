using UnityEngine;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/FlappyHappy - Set Framerate")]
    public class SetFramerate : MonoBehaviour
    {
        [SerializeField] private int frameRate;

        private void Awake()
        {
            Application.targetFrameRate = frameRate;
        }
    }
}