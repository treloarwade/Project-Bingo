using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NpcMovementScript : MonoBehaviour
{
    public float speed = 1f; // Speed of movement
    public float maxRotation = 5f;
    public float rotationSpeed = 50f;

    private float currentRotation = 0f;
    private float rotationDirection = 1f;
    public bool conversation = false;
    private Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(MoveObject());
    }
    [ServerRpc(RequireOwnership = false)]
    public void ConversationServerRpc(bool convo)
    {
        conversation = convo;
    }

    private void FixedUpdate()
    {
        // Only rotate if moving and no dialog active
        if (!conversation && rb.velocity.magnitude > 0.4f)
        {
            RotateSprite();
        }
        else
        {
            currentRotation = 0f;
            rb.SetRotation(currentRotation);
        }
    }

    private IEnumerator MoveObject()
    {
        while (true)
        {
            // Move right for 11 seconds (unless dialog appears)
            float moveTime = 0f;
            while (moveTime < 11f)
            {
                if (!conversation)
                {
                    rb.velocity = Vector2.right * speed;
                    moveTime += Time.deltaTime;
                }
                else
                {
                    rb.velocity = Vector2.zero;
                }
                yield return null;
            }

            // Pause for 5 seconds (unless dialog appears)
            float pauseTime = 0f;
            rb.velocity = Vector2.zero;
            while (pauseTime < 5f)
            {
                if (!conversation)
                {
                    pauseTime += Time.deltaTime;
                }
                yield return null;
            }

            // Move left for 11 seconds (unless dialog appears)
            moveTime = 0f;
            while (moveTime < 11f)
            {
                if (!conversation)
                {
                    rb.velocity = Vector2.left * speed;
                    moveTime += Time.deltaTime;
                }
                else
                {
                    rb.velocity = Vector2.zero;
                }
                yield return null;
            }

            // Pause for 5 seconds (unless dialog appears)
            pauseTime = 0f;
            rb.velocity = Vector2.zero;
            while (pauseTime < 5f)
            {
                if (!conversation)
                {
                    pauseTime += Time.deltaTime;
                }
                yield return null;
            }
        }
    }

    private void RotateSprite()
    {
        currentRotation += rotationDirection * rotationSpeed * Time.fixedDeltaTime;

        if (Mathf.Abs(currentRotation) >= maxRotation)
        {
            rotationDirection *= -1;
        }

        rb.SetRotation(currentRotation);
    }
}