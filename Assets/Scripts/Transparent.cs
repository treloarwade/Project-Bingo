using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparent : MonoBehaviour
{
    // Reference to the sprite renderer component
    private SpriteRenderer spriteRenderer;

    // Transparency level when inside the collider
    public float transparencyLevel = 0.5f; // Change this value as needed

    private void Start()
    {
        // Get the SpriteRenderer component attached to the GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure the SpriteRenderer component is not null
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found!");
        }
    }

    // Called when another Collider2D enters the trigger collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Set the transparency level of the sprite
            ChangeTransparency(transparencyLevel);
        }
    }

    // Called when another Collider2D exits the trigger collider
    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the object exiting the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Reset the transparency level of the sprite
            ChangeTransparency(1f); // 1f is fully opaque
        }
    }

    // Function to change the transparency of the sprite
    private void ChangeTransparency(float alpha)
    {
        // Ensure the sprite renderer component is not null
        if (spriteRenderer != null)
        {
            // Get the current color of the sprite
            Color spriteColor = spriteRenderer.color;

            // Set the alpha value of the color
            spriteColor.a = alpha;

            // Apply the new color to the sprite
            spriteRenderer.color = spriteColor;
        }
    }
}
