using UnityEngine;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/Arcade Machine/FlappyHappy - Arcade Culling Trigger")]
    public class ArcadeCullingTrigger : MonoBehaviour
    {
        public UnityEventInt OnCountUpdate = new UnityEventInt();

        int withinCollisionCount;

        private string[] tags;

        public void Configure(string[] tags)
        {
            this.tags = tags;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (HasMatchingTag(other.gameObject))
            {
                withinCollisionCount++;
                OnCountUpdate.Invoke(withinCollisionCount);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (HasMatchingTag(other.gameObject))
            {
                withinCollisionCount--;
                OnCountUpdate.Invoke(withinCollisionCount);
            }
        }

        private bool HasMatchingTag(GameObject go)
        {
            int tagCount = tags.Length;
            for (int i = 0; i < tagCount; i++)
            {
                if (go.CompareTag(tags[i]))
                    return true;
            }

            return false;
        }
    }
}