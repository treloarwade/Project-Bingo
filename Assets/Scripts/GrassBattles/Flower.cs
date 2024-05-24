using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public Sprite[] sprites;
    public Collider2D col;
    private SpriteRenderer spriteRenderer;
    [Range(0.0f, 1.0f)]
    public float switchChance = 0.5f;
    public static int destroyedFlowerCount = 0;
    void Start()
    {
        // Get the SpriteRenderer component attached to this GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (Random.value < switchChance)
        {
            spriteRenderer.sprite = sprites[0];

            float randomX = Random.Range(-0.08f, 0.08f);
            float randomY = Random.Range(-0.15f, -0.05f);
            // Create a new Vector3 with the random values
            Vector3 randomMovement = new Vector3(randomX, randomY, 0f);  // Keep Z at 0

            // Update the position with the random movement
            transform.position += randomMovement;
            // Randomly rotate the Z axis
            float randomZRotation = Random.Range(0f, 360f);
            Debug.Log($"Random Z Rotation: {randomZRotation}");

            // Create the target rotation with the random Z rotation
            Quaternion targetRotation = Quaternion.Euler(0, 0, randomZRotation);

            // Update the object's rotation
            transform.rotation = targetRotation;

            // Debug the rotation to verify
            Debug.Log($"Applied Rotation: {transform.rotation.eulerAngles}");
            // 50% chance to flip the X axis
            if (Random.value > 0.5f)
            {
                // Flip the X axis by scaling it to -1
                Vector3 localScale = transform.localScale;
                localScale.x *= -1;
                transform.localScale = localScale;
            }
            destroyedFlowerCount++;
            Debug.Log(destroyedFlowerCount);
            col.enabled = false;
        }

    }
}

