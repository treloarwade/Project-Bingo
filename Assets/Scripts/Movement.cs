using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Movement : NetworkBehaviour
{
    Rigidbody2D body;

    float horizontal;
    float vertical;
    public Text MovementSpeed;

    public float runSpeed = 2.0f;
    public float maxRotation = 5f;
    public float rotationSpeed = 50f;

    private float currentRotation = 0f;
    private float rotationDirection = 1f;

    private int runSpeedIndex = 0;
    private float[] runSpeedValues = { 2f, 3f, 4f, 5f, 10f, 20f }; // Array of smoothTime values to cycle through

    // Reference to the camera
    public Camera mainCamera;

    // Camera follow parameters
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    // Reference to the dialog manager
    public DialogManager dialogManager;
    public BoatScript boatScript;
    private Vector3 savedPosition;
    public bool movementEnabled = true;

    // Store original Rigidbody properties
    private RigidbodyType2D originalBodyType;
    private float originalDrag;
    private float originalAngularDrag;
    private bool originalGravityScale;
    public void LoadCoordinates()
    {
        // Load the position and rotation from PlayerPrefs
        savedPosition = new Vector3(
            PlayerPrefs.GetFloat("PosX", 0),
            PlayerPrefs.GetFloat("PosY", 0),
            PlayerPrefs.GetFloat("PosZ", 0)
        );
        transform.position = savedPosition;
    }
    public void SaveCoordinates()
    {
        // Save the position and rotation of the object
        PlayerPrefs.SetFloat("PosX", transform.position.x);
        PlayerPrefs.SetFloat("PosY", transform.position.y);
        PlayerPrefs.SetFloat("PosZ", transform.position.z);

        // Save PlayerPrefs to disk
        PlayerPrefs.Save();

        Debug.Log("Coordinates saved.");
    }
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        originalBodyType = body.bodyType;
        originalDrag = body.drag;
        originalAngularDrag = body.angularDrag;
        originalGravityScale = body.gravityScale > 0;
        LoadCoordinates();
    }
    public void SetPhysicsStateForBattle(bool inBattle)
    {
        if (body == null) return;

        body.bodyType = inBattle ? RigidbodyType2D.Static : originalBodyType;

        if (!inBattle)
        {
            // Reset velocities when exiting battle
            body.velocity = Vector2.zero;
            body.angularVelocity = 0f;
        }

        movementEnabled = !inBattle;
    }
    public void SetRigidbodyStatic(bool makeStatic)
    {
        if (body == null) return;

        if (makeStatic)
        {
            // Save current velocity if needed
            // body.velocity = Vector2.zero;
            // body.angularVelocity = 0f;

            // Make static
            body.bodyType = RigidbodyType2D.Static;
        }
        else
        {
            // Restore to original dynamic state
            body.bodyType = originalBodyType;
            body.drag = originalDrag;
            body.angularDrag = originalAngularDrag;
            body.gravityScale = originalGravityScale ? 1f : 0f;

            // Reset velocity
            body.velocity = Vector2.zero;
            body.angularVelocity = 0f;
        }
    }
    void Update()
    {
        if (!IsOwner) return;
        if (!movementEnabled)
        {
            horizontal = 0f;
            vertical = 0f;
            UpdateMovement(); // Ensure movement is updated immediately
            return;
        }
        // Check if the dialog box is active, if yes, disable movement
        if (dialogManager != null && dialogManager.IsDialogActive())
        {
            horizontal = 0f;
            vertical = 0f;
            UpdateMovement(); // Ensure movement is updated immediately
            return; // Exit Update function to prevent further processing
        }

        // If dialog box is not active, allow player movement
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        UpdateMovement(); // Ensure movement is updated immediately
    }


    private void FixedUpdate()
    {
        // Skip physics updates if Rigidbody is static or kinematic
        if (body.bodyType != RigidbodyType2D.Dynamic || !movementEnabled)
        {
            return;
        }

        Vector2 movement = new Vector2(horizontal * runSpeed, vertical * runSpeed);
        body.velocity = movement;

        if (movement.magnitude > 0)
        {
            RotateSprite();
        }
        else
        {
            currentRotation = 0f;
            body.SetRotation(currentRotation);
        }
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

    void UpdateMovement()
    {
        // Enable or disable player movement based on movementEnabled flag
        Movement movementScript = GetComponent<Movement>();
        if (movementScript != null)
        {
            //movementScript.enabled = !dialogManager.IsDialogActive();
        }
    }
    public void IncreaseRunSpeed()
    {
        runSpeed += 4.0f;
    }
    public void DecreaseRunSpeed()
    {
        runSpeed -= 4.0f;
    }
    public void OnClick()
    {
        // Increment the runSpeed index
        runSpeedIndex = (runSpeedIndex + 1) % runSpeedValues.Length;

        // Update runSpeed to the value at the current index
        runSpeed = runSpeedValues[runSpeedIndex];

        Debug.Log("runSpeed toggled to: " + runSpeed);
    }
}
