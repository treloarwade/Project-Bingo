using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using static UnityEngine.GridBrushBase;

public class BoatScript : MonoBehaviour
{
    Rigidbody2D body;
    float horizontal;
    float vertical;
    public bool movementEnabled = true;
    public bool boatModeEnabled = false;
    public GameObject Bingo;
    public GameObject Boat;
    private SpriteRenderer spriteRenderer;
    public Sprite[] frames;
    public Sprite[] frames2;
    public float maxRotation = 2f;
    public float rotationSpeed = 5f;
    private float currentRotation = 0f;
    private float rotationDirection = 1f;
    private int currentFrameIndex = 0;
    private float timer = 0f;
    private float animationTimer = 0f;
    public float frameRate = 0.5f;
    public float animationDuration = 10f;
    public Transform BoatCamera;
    public Transform BingoCamera;
    private Vector3 BoatPosition = new Vector3(108.830002f, 69.2600021f, 0);
    public Collider2D collision;
    public void StartBoat()
    {
        movementEnabled = false;
        UpdateMovement();
        StartCoroutine(BoatAnimation());
    }
    public void StopBoat()
    {
        StartCoroutine(StopBoatAnimation());
    }
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void RotateSprite()
    {
        currentRotation += rotationDirection * rotationSpeed * Time.fixedDeltaTime;

        if (Mathf.Abs(currentRotation) >= maxRotation)
        {
            rotationDirection *= -1;
        }

        body.SetRotation(currentRotation);
    }
    IEnumerator BoatAnimation()
    {
        collision.enabled = false;
        Interactor BingoScript = FindObjectOfType<Interactor>();
        BingoScript.isInRange = false;
        CameraFollow cameraFollowScript = FindObjectOfType<CameraFollow>();
        Vector3 targetPosition = new Vector3(109.37f, 69.25f, 0);
        float startTime = Time.time;

        while (Time.time - startTime < 1f)
        {
            float fracJourney = (Time.time - startTime) / 100f;
            Bingo.transform.localPosition = Vector3.Lerp(Bingo.transform.localPosition, targetPosition, fracJourney);
            Bingo.transform.Rotate(Vector3.forward, 1);
            yield return null;
        }
        Bingo.SetActive(false);
        Bingo.transform.localPosition = Boat.transform.localPosition;
        Bingo.transform.localPosition = new Vector3(Bingo.transform.localPosition.x, Bingo.transform.localPosition.y, -10);
        spriteRenderer.sprite = frames[0];

        boatModeEnabled = true;
        movementEnabled = true;
        UpdateMovement();
        cameraFollowScript.target = BoatCamera;
        gameObject.tag = "Player";
        collision.enabled = true;
    }
    IEnumerator StopBoatAnimation()
    {
        collision.enabled = false;
        Interactor BingoScript = FindObjectOfType<Interactor>();
        BingoScript.isInRange = false;
        movementEnabled = false;
        UpdateMovement();
        CameraFollow cameraFollowScript = FindObjectOfType<CameraFollow>();
        Vector3 targetPosition = new Vector3(108.87f, 67.25f, 0);
        float startTime = Time.time;
        while (Time.time - startTime < 2)
        {
            float fracJourney = (Time.time - startTime) / 100;
            Boat.transform.localRotation = Quaternion.Lerp(Boat.transform.localRotation, Quaternion.identity, fracJourney);
            Boat.transform.localPosition = Vector3.Lerp(Boat.transform.localPosition, BoatPosition, fracJourney);

            yield return null;
        }
        startTime = Time.time;
        Bingo.SetActive(true);
        spriteRenderer.sprite = frames[1];
        boatModeEnabled = false;
        while (Time.time - startTime < 1f)
        {
            float fracJourney = (Time.time - startTime) / 100f;
            Boat.transform.localPosition = Vector3.Lerp(Boat.transform.localPosition, BoatPosition, fracJourney);
            Bingo.transform.localPosition = Vector3.Lerp(Bingo.transform.localPosition, targetPosition, fracJourney);
            Bingo.transform.Rotate(Vector3.forward, 1);
            yield return null;
        }
        body.velocity = Vector3.zero;
        movementEnabled = true;
        UpdateMovement();
        cameraFollowScript.target = BingoCamera;
        gameObject.tag = "Untagged";
    }

    private void Update()
    {

        if (boatModeEnabled)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

        }
    }
    private void FixedUpdate()
    {
        RotateSprite();

        if (boatModeEnabled)
        {
            Vector2 movement = new Vector2(horizontal * .5f, vertical * .5f);


            if (movement.magnitude > 0)
            {
                Vector2 premovement = body.velocity;
                if (premovement.magnitude < 5 )
                {
                    body.velocity = premovement + movement;
                }

                // Check if moving right or left
                if (horizontal > 0)
                {
                    spriteRenderer.flipX = true; // Not flipped (facing right)
                }
                else if (horizontal < 0)
                {
                    spriteRenderer.flipX = false; // Flipped (facing left)
                }
                if(spriteRenderer.sprite = frames[0])
                {
                    spriteRenderer.sprite = frames2[0];
                }
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
                    currentFrameIndex = (currentFrameIndex + 1) % frames2.Length;

                    // Update sprite renderer with the new frame
                    spriteRenderer.sprite = frames2[currentFrameIndex];

                    // Reset timer
                    timer = 0f;
                }
            }
            else
            {
                spriteRenderer.sprite = frames[0];
            }
        }
    }
    void UpdateMovement()
    {
        // Enable or disable player movement based on movementEnabled flag
        Movement movementScript = GetComponent<Movement>();
        if (movementScript != null)
        {
            movementScript.enabled = movementEnabled;
        }
    }
}
