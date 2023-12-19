using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/FlappyHappy - Pipe Manager")]
    public class PipeManager : MonoBehaviour
    {
        [SerializeField] private Core core;
        [SerializeField] private GameObject pipePrefab;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float spawnDistanceInterval = 3;
        [SerializeField] private Transform bird;
        [SerializeField] private Camera camera;

        private List<Pipe> instances = new List<Pipe>();
        private List<Pipe> inactiveInstances = new List<Pipe>();

        private bool startSpawning;

        private float distanceTraveled;

        private float minSpawnDistance;
        private float maxSpawnDistance;

        private void Start()
        {
            core.ListenToGameStart(OnGameStart);
            core.ListenToGameRestart(OnGameRestart);
            core.ListenToGameOver(OnGameOver);

            RecalculateStandEndPoints();
        }

        private void RecalculateStandEndPoints()
        {
            // Get maximum/minimum point left side of screen. For spawning/despawning of pipes, making it impossible to see any pop-in.
            minSpawnDistance = -10f;// RaycastPlayerfieldPlane(new Vector3(0, camera.scaledPixelHeight * 0.5f)).x - 1;
            maxSpawnDistance = 10f;// RaycastPlayerfieldPlane(new Vector3(camera.scaledPixelWidth, camera.scaledPixelHeight * 0.5f)).x + 1;
        }

        private Vector3 RaycastPlayerfieldPlane(Vector3 screenPosition)
        {
            camera = Camera.main;
            if (camera == null)
                return default;

            Ray ray = camera.ScreenPointToRay(screenPosition, Camera.MonoOrStereoscopicEye.Mono);

            // Creates a plane that aligns with the position of the bird
            Plane plane = new Plane(new Vector3(0, 0, -1), transform.position);

            //Initialise the enter variable
            float enter;

            if (plane.Raycast(ray, out enter))
            {
                //Get the point that is clicked
                Vector3 hitPoint = ray.GetPoint(enter);

                return hitPoint;
            }

            return default;
        }

        private void OnGameRestart()
        {
            int instanceCount = instances.Count;

            for (int i = instanceCount - 1; i >= 0; i--)
            {
                instances[i].gameObject.SetActive(false);
                inactiveInstances.Add(instances[i]);
                instances.RemoveAt(i);
            }

            // Required due to the potential camera angle change
            RecalculateStandEndPoints();
        }

        private void OnGameStart()
        {
            startSpawning = true;
        }

        private void OnGameOver()
        {
            startSpawning = false;
        }

        private void Update()
        {
            if (!startSpawning)
                return;

            var offset = Time.deltaTime * (moveSpeed * core.GetSpeed());
            distanceTraveled += offset;

            for (int i = instances.Count - 1; i >= 0; i--)
            {
                var pos = instances[i].transform.position;
                pos.x -= offset;

                instances[i].transform.position = pos;

                if (pos.x < minSpawnDistance)
                {
                    instances[i].gameObject.SetActive(false);
                    inactiveInstances.Add(instances[i]);
                    instances.RemoveAt(i);
                }
            }

            if (distanceTraveled > spawnDistanceInterval)
            {
                SpawnPipe();
                distanceTraveled = 0;
            }
        }

        private void SpawnPipe()
        {
            Vector3 spawnPos = new Vector3(maxSpawnDistance, transform.position.y, transform.position.z);

            if (inactiveInstances.Count > 0)
            {
                int instanceIndex = inactiveInstances.Count - 1;
                GameObject getInstance = inactiveInstances[instanceIndex].gameObject;
                getInstance.transform.position = spawnPos;
                getInstance.gameObject.SetActive(true);
                instances.Add(inactiveInstances[instanceIndex]);
                inactiveInstances.RemoveAt(instanceIndex);
                return;
            }

            var obj = Instantiate(pipePrefab, spawnPos, pipePrefab.transform.rotation);
            obj.transform.SetParent(transform);
            obj.gameObject.SetActive(true);
            instances.Add(obj.GetComponent<Pipe>());
        }
    }
}