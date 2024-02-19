using UnityEngine;

public class FiveFingerTap : MonoBehaviour
{
    public GameObject objectToActivate; // Assign in Inspector
    private int tapCount = 0;
    private float holdTimer = 0f;
    private bool isHolding = false;

    private float NotHoldingTimer;

    void Update()
    {
        // Check for 5 touches
        if (Input.touchCount == 5)
        {
            bool allTouchesBegan = true;
            bool anyTouchEnded = false;

            // Check each touch phase
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase != TouchPhase.Began)
                {
                    allTouchesBegan = false;
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    anyTouchEnded = true;
                }
            }

            // If all 5 fingers tapped
            if (allTouchesBegan)
            {
                tapCount++;
            }

            // Start hold timer if 5 fingers are still touching after taps
            if (tapCount >= 5 && !anyTouchEnded)
            {
                isHolding = true;
                holdTimer += Time.deltaTime;
            }
            NotHoldingTimer = 1f;
        }

        if (tapCount > 0)
        {
            if (NotHoldingTimer > 0)
                NotHoldingTimer -= Time.deltaTime;
            if (NotHoldingTimer <= 0)
                ResetGesture();
        }

        // Check for hold time
        if (isHolding && holdTimer >= 2f)
        {
            // Activate the object
            objectToActivate.SetActive(true);
            // Reset gesture to avoid repeated activations
            ResetGesture();
        }
    }

    void ResetGesture()
    {
        tapCount = 0;
        holdTimer = 0f;
        isHolding = false;
    }
}
