using UnityEngine;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/FlappyHappy - Scroller")]
    public class Scroller : MonoBehaviour
    {
        [SerializeField] private Core core;
        [SerializeField] private float offset;
        [SerializeField] private float scrollSpeed;
        [SerializeField] private bool useSpeedMultiplier = true;

        float t;

        private void Awake()
        {
            t = offset;
        }

        private void Update()
        {
            if (t > 1)
            {
                t = 0;
            }
            else
            {
                float speedMultiplier = useSpeedMultiplier ? core.GetSpeed() : 1;

                t += Time.deltaTime * ((scrollSpeed / 5) * speedMultiplier);
            }

            Vector3 v = transform.localPosition;
            v.x = Mathf.Lerp(0, -5, t);
            transform.localPosition = v;
        }
    }
}