using UnityEngine;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/FlappyHappy - Game Input")]
    public class GameInput : MonoBehaviour
    {
        [SerializeField] private Core core;

        void Update()
        {
            bool pressed = false;

            int touchCount = Input.touchCount;

            for (int i = 0; i < touchCount; i++)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    pressed = true;
                    break;
                }
            }

#if UNITY_EDITOR || UNITY_STANDALONE
            pressed = Input.GetMouseButtonDown(0);
#endif

            if (pressed)
            {
                core.Tap();
            }
        }
    }
}