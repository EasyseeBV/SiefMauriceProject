using UnityEngine;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/FlappyHappy - Pipe")]
    public class Pipe : MonoBehaviour
    {
        [SerializeField] private float minHeight;
        [SerializeField] private float maxHeight;

        [SerializeField] private float minOffset;
        [SerializeField] private float maxOffset;

        [SerializeField] private Transform top;
        [SerializeField] private Transform bottom;

        [SerializeField] private Transform sprite;

        private float baseTopHeight, baseBottomHeight, spawnHeight;

        private void Awake()
        {
            baseTopHeight = top.transform.localPosition.y;
            baseBottomHeight = bottom.transform.localPosition.y;
            spawnHeight = transform.position.y;
        }

        private void OnDrawGizmosSelected()
        {
            var pos = transform.position;
            Gizmos.DrawWireSphere(new Vector3(pos.x, minHeight, pos.z), 0.5f);
            Gizmos.DrawWireSphere(new Vector3(pos.x, maxHeight, pos.z), 0.5f);
        }

        private void OnEnable()
        {
            var pos = transform.position;
            pos.y = spawnHeight + Random.Range(minHeight, maxHeight);
            transform.position = pos;

            var spritePosition = sprite.transform.position;
            spritePosition.y = spawnHeight - 0.1360159f;
            sprite.transform.position = spritePosition;

            var pipeOffset = Random.Range(minOffset, maxOffset);
            bottom.transform.localPosition = new Vector3(0, baseBottomHeight - pipeOffset, 0);
            top.transform.localPosition = new Vector3(0, baseTopHeight + pipeOffset, 0);
        }
    }
}