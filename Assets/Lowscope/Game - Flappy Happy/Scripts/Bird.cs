using UnityEngine;

namespace Lowscope.ArcadeGame.FlappyHappy
{
    [AddComponentMenu("Lowscope/Flappy Happy/FlappyHappy - Bird")]
    public class Bird : MonoBehaviour
    {
        [Header("References")]

        [SerializeField] private Core core;
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform leftEye;
        [SerializeField] private Transform rightEye;

        [SerializeField] private AudioSource soundFlap;
        [SerializeField] private AudioSource soundDeathImpact;
        [SerializeField] private AudioSource soundImpact;
        [SerializeField] private AudioSource soundScore;

        [Header("Configuration - Air")]
        [SerializeField] private float jumpForce;
        [SerializeField] private float maximumHeight;
        [SerializeField] private float gravityMultiplier = 3;

        [Header("Configuration - Collision Impulses")]
        [SerializeField] private float moveToCameraImpulse = 1;
        [SerializeField] private float groundCollisionImpulse = 10;
        [SerializeField] private float wallCollisionImpulse = 2;

        [Header("Configuration - Starting Float")]
        [SerializeField] private float startFloatSpeed = 4;
        [SerializeField] private float startFloatDistance = 0.2f;

        [Header("Configuration - Angles")]
        [SerializeField] private float maximumHeightAngle = 30;
        [SerializeField] private float fallingAngularVelocity = -3;
        [SerializeField] private float tapAngleImpulse = 5;

        private float t;
        private float animationTime;

        private bool gameStarted;

        private Vector3 startPosition;
        private Quaternion startRotation;
        private RigidbodyConstraints rigidbodyConstraints;
        private int lastCollisionID;

        private bool useGravity;

        private void Start()
        {
            // Subscribe to Core events.
            core.ListenToGameStart(OnGameStarted);
            core.ListenToGameRestart(OnGameRestart);
            core.ListenToGameTap(OnTap);

            // Store the starting positions as 
            // a reference for when the game restarts.
            startPosition = transform.position;
            startRotation = transform.rotation;
            rigidbodyConstraints = rigidBody.constraints;
        }

        // Called after the restart button has been pressed
        // The bird is not responding to your inputs yet, but is in a floating state.
        private void OnGameRestart()
        {
            gameStarted = false;

            lastCollisionID = 0;

            useGravity = false;
            //rigidBody.useGravity = false;
            rigidBody.velocity = new Vector3(0, 0, 0);
            rigidBody.angularVelocity = new Vector3(0, 0, 0);
            rigidBody.constraints = rigidbodyConstraints;

            transform.position = startPosition;
            transform.rotation = startRotation;

            SetDeadEyes(false);
        }

        // When the game actually starts, we start the gravity and trigger
        // The game started event.
        private void OnGameStarted()
        {
            gameStarted = true;

            useGravity = true;
            //rigidBody.useGravity = true;
        }

        private void OnTap()
        {
            // Reset the velocity of the gravity. Making it push up instantly.
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpForce);

            // Push the angle based on the tapAngleImpulse. This makes the bird rotate upwards.
            rigidBody.angularVelocity = new Vector3(0, 0, tapAngleImpulse);

            // Play the flap animation, where beaks open and wings go up and down.
            animator.PlayInFixedTime("Bird_Flap");

            soundFlap.Play();
        }

        private void Update()
        {
            // Dont do anything if the game is over.
            if (core.IsGameOver())
            {
                return;
            }

            // Float the bird up and down if the game has not started yet.
            if (!gameStarted)
            {
                Vector3 newPosition = startPosition;
                newPosition.y = startPosition.y + (Mathf.Cos(t * startFloatSpeed) * startFloatDistance);
                transform.position = newPosition;
                transform.rotation = startRotation;

                if (animationTime > 0.5f)
                {
                    animator.Play("Bird_Flap_Loop");
                    animationTime = 0;
                }

                t += Time.deltaTime;
                animationTime += Time.deltaTime;

                // Since the game has not started, we exit the code here.
                return;
            }

            Vector3 euler = transform.rotation.eulerAngles;

            // Clamp the bird rotation to a maximum angle.
            if (euler.x > maximumHeightAngle && rigidBody.velocity.y > 0)
            {
                transform.rotation = Quaternion.Euler(maximumHeightAngle, -90, 0);
                rigidBody.angularVelocity = new Vector3(0, 0, 0);
            }
            else
            {
                // Rotate the bird downwards if we have a negative velocity
                if (rigidBody.velocity.y < 0)
                {
                    rigidBody.angularVelocity = new Vector3(0, 0, fallingAngularVelocity);
                }
            }

            // Ensure the bird cannot flap above the pipes.
            if (transform.localPosition.y > maximumHeight)
            {
                var activePos = transform.localPosition;
                activePos.y = maximumHeight;
                transform.localPosition = activePos;
            }
        }

        // If we hit an object, such as the ground or a pipe.
        private void OnCollisionEnter(Collision collision)
        {
            // Trigger game over event
            if (!core.IsGameOver())
            {
                core.GameOver();
                SetDeadEyes(true);

                soundDeathImpact.Play();
            }

            // Start pushing the bird with impulses, using the contact point
            var contactPoint = collision.contacts[0];
            bool isBelowPlayer = contactPoint.point.y < transform.position.y;

            rigidBody.constraints = RigidbodyConstraints.None;
            rigidBody.AddForce(contactPoint.normal * ((isBelowPlayer) ?
                groundCollisionImpulse : wallCollisionImpulse), ForceMode.VelocityChange);
            rigidBody.AddForce(Vector3.up);
            rigidBody.AddForce((Vector3.back * 2) * moveToCameraImpulse, ForceMode.Impulse);
            rigidBody.angularVelocity = new Vector3(0, 0, 15);

            // Play a sound
            soundImpact.Play();
        }

        private void FixedUpdate()
        {
            if (useGravity)
            {
                rigidBody.AddForce(Physics.gravity * gravityMultiplier);
            }
        }

        private void SetDeadEyes(bool dead)
        {
            // Other side of eyes contains the other art.
            Vector3 eyeScale = Vector3.one * ((dead) ? -1 : 1);
            leftEye.transform.localScale = eyeScale;
            rightEye.transform.localScale = eyeScale;
        }

        // Used to increment score.
        private void OnTriggerEnter(Collider other)
        {
            int instanceID = other.gameObject.GetInstanceID();

            // Don't allow duplicate collisions from the same score source.
            if (instanceID == lastCollisionID)
            {
                return;
            }

            // Set the collision id to the last pipe the bird has collided with.
            lastCollisionID = other.gameObject.GetInstanceID();

            // Dont do anything if the game has ended
            if (core.IsGameOver())
            {
                return;
            }

            core.IncrementScore();

            soundScore.Play();
        }
    }
}