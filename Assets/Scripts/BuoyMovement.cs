using UnityEngine;

public class BuoyMovement : MonoBehaviour
{
    public float movementSpeed = 2f;
    public float switchDirectionInterval = 2f; // Interval to randomly switch direction

    private float currentMovementSpeed; // Current movement speed (with direction)
    private float switchDirectionTimer; // Timer to track when to switch direction

    void Start()
    {
        // Start moving to the right initially
        currentMovementSpeed = movementSpeed;
        switchDirectionTimer = switchDirectionInterval;
    }

    void Update()
    {
        // Update the switch direction timer
        switchDirectionTimer -= Time.deltaTime;

        // Check if it's time to switch direction
        if (switchDirectionTimer <= 0f)
        {
            // Randomly determine whether to switch direction
            if (Random.value < 0.5f)
            {
                // Switch to moving right
                currentMovementSpeed = Mathf.Abs(movementSpeed);
            }
            else
            {
                // Switch to moving left
                currentMovementSpeed = -Mathf.Abs(movementSpeed);
            }

            // Reset the switch direction timer
            switchDirectionTimer = Random.Range(switchDirectionInterval * 0.5f, switchDirectionInterval * 1.5f);
        }

        // Move the object horizontally
        transform.Translate(Vector3.right * currentMovementSpeed * Time.deltaTime);
    }
}
