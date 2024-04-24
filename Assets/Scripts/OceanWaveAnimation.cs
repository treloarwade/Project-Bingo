using UnityEngine;

public class OceanWaveAnimation : MonoBehaviour
{
    public Sprite[] waveFrames; // Array of sprite frames representing the ocean waves
    public float frameRate = 0.5f; // Frames per second for the animation
    public float animationDuration = 10f; // Duration of each animation cycle

    private SpriteRenderer spriteRenderer;
    private int currentFrameIndex = 0;
    private float timer = 0f;
    private float animationTimer = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (waveFrames.Length > 0)
        {
            spriteRenderer.sprite = waveFrames[0]; // Set initial sprite frame
        }
    }

    void Update()
    {
        // Update animation timer
        animationTimer += Time.deltaTime;

        // Check if it's time to reset animation
        if (animationTimer >= animationDuration)
        {
            // Reset animation timer
            animationTimer = 0f;
        }

        // Update timer
        timer += Time.deltaTime;

        // Check if it's time to switch frames
        if (timer >= 1f / frameRate)
        {
            // Increment current frame index
            currentFrameIndex = (currentFrameIndex + 1) % waveFrames.Length;

            // Update sprite renderer with the new frame
            spriteRenderer.sprite = waveFrames[currentFrameIndex];

            // Reset timer
            timer = 0f;
        }
    }
}
