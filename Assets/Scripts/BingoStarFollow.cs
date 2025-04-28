using UnityEngine;
using UnityEngine.UI;

public class BingoStarFollow : MonoBehaviour
{
    public Transform target; // Reference to the player object
    public float distance = 2f; // Distance to maintain from the target
    public float smoothTime = 0.5f;
    private Vector3 velocity = Vector3.zero;
    private int smoothTimeIndex = 0;
    private float[] smoothTimeValues = { 5.0f, 4.5f, 4.0f, 3.5f, 3.0f, 2.5f, 2.0f, 1.5f, 1.0f };
    private float maxRotation = 5f;
    private float minRotationSpeed = 0.3f;
    private float maxRotationSpeed = 5f;
    private float currentRotation = 0f;
    private float rotationDirection = 1f;
    public Rigidbody2D body;
    public Text Speed;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;
    private float maxVelocityForSpeed = 3f; // Velocity at which we reach max rotation speed

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 directionToTarget = transform.position - target.position;
            directionToTarget.z = 0f;
            Vector3 targetPosition = target.position + directionToTarget.normalized * distance;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            // Flip sprite based on X velocity
            if (velocity.x > 0.1f && !facingRight)
            {
                Flip();
            }
            else if (velocity.x < -0.1f && facingRight)
            {
                Flip();
            }
        }

        if (velocity.magnitude > 0)
        {
            RotateSprite();
        }
        else
        {
            currentRotation = 0f;
            body.SetRotation(currentRotation);
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
        else
        {
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    public void OnClick()
    {
        smoothTimeIndex = (smoothTimeIndex + 1) % smoothTimeValues.Length;
        smoothTime = smoothTimeValues[smoothTimeIndex];
        Debug.Log("smoothTime toggled to: " + smoothTime);
        Speed.text = "Speed: " + smoothTime.ToString();
    }

    private void RotateSprite()
    {
        // Calculate rotation speed based on velocity magnitude (clamped between min and max)
        float velocityRatio = Mathf.Clamp01(velocity.magnitude / maxVelocityForSpeed);
        float currentRotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, velocityRatio);

        currentRotation += rotationDirection * currentRotationSpeed * Time.fixedDeltaTime;

        if (Mathf.Abs(currentRotation) >= maxRotation)
        {
            rotationDirection *= -1;
        }

        body.SetRotation(currentRotation);
    }
}