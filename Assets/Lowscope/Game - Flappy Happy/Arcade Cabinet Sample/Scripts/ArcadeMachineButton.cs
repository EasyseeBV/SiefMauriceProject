using System.Collections;
using UnityEngine;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/Arcade Machine/FlappyHappy - Arcade Machine Button")]
    public class ArcadeMachineButton : MonoBehaviour
    {
        [SerializeField] private ArcadeMachine arcadeGameMachine;

        private Coroutine tapCoroutine;
        private float interactionDistance;

        private Camera mainCamera;

        public void Configure(float interactionDistance)
        {
            this.interactionDistance = interactionDistance;
        }

        private void OnMouseDown()
        {
            if (mainCamera == null)
            {
                mainCamera = GetMainCamera();
            }

            float distance = Vector3.Distance(mainCamera.transform.position, transform.position);

            if (distance < interactionDistance)
            {
                arcadeGameMachine.PressButton(true);
            }

        }

        // Called from the arcade machine, to make it simple to display this animation
        // When you use a different means of input.
        public void DisplayButtonAnimation()
        {
            transform.localPosition = new Vector3(0, -0.1f, 0);

            if (tapCoroutine == null)
                tapCoroutine = StartCoroutine(TapButtonBounce());
        }

        /// <summary>
        /// Attempts to get a camera, even if the MainCamera tag is not set for a camera.
        /// </summary>
        /// <returns></returns>
        private Camera GetMainCamera()
        {
            var getAllCameras = Camera.allCameras;

            if (getAllCameras.Length == 1)
            {
                return getAllCameras[0];
            }

            float closestDistance = float.MaxValue;
            int closestIndex = -1;
            int cameraCount = getAllCameras.Length;

            for (int i = 0; i < cameraCount; i++)
            {
                Vector3 targetPos = getAllCameras[i].transform.position;
                Vector3 thisPos = transform.position;

                float distance = Vector3.SqrMagnitude(targetPos - thisPos);

                if (closestIndex == -1 || distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            return getAllCameras[closestIndex];
        }

        private IEnumerator TapButtonBounce()
        {
            while (transform.localPosition.y != 0)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(0, 0, 0), Time.deltaTime);
                yield return null;
            }

            tapCoroutine = null;
        }
    }
}