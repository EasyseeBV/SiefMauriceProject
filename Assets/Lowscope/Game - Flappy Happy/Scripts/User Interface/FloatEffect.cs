using UnityEngine;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/FlappyHappy - Float Effect")]
    public class FloatEffect : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;

        [SerializeField] private float floatDistance;
        [SerializeField] private float speed = 0.5f;
        [SerializeField] private float timeOffset;
        [SerializeField] private bool bounce;

        Vector3 pos;

        private void Awake()
        {
            pos = rectTransform.anchoredPosition;
        }

        void Update()
        {
            float time = Mathf.Sin((Time.time * speed) + timeOffset);

            if (bounce)
            {
                time = Mathf.Abs(time);
            }

            rectTransform.anchoredPosition = new Vector3(pos.x, pos.y + (time * floatDistance), pos.z);
        }
    }
}