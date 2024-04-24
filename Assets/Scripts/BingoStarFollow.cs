using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GridBrushBase;

public class BingoStarFollow : MonoBehaviour
{
    public Transform target; // Reference to the player object
    public float distance = 2f; // Distance to maintain from the target
    public float smoothTime = 0.5f;
    private Vector3 velocity = Vector3.zero;
    private int smoothTimeIndex = 0;
    private float[] smoothTimeValues = { 5.0f, 4.5f, 4.0f, 3.5f, 3.0f, 2.5f, 2.0f, 1.5f, 1.0f }; // Array of smoothTime values to cycle through
    private float maxRotation = 5f;
    private float rotationSpeed = 0.3f;
    private float currentRotation = 0f;
    private float rotationDirection = 1f;
    public Rigidbody2D body;
    public Text Speed;


    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the direction from the target to the BingoStar
            Vector3 directionToTarget = transform.position - target.position;
            directionToTarget.z = 0f; // Ignore the Z-axis for 2D movement

            // Normalize the direction and multiply by the desired distance
            Vector3 targetPosition = target.position + directionToTarget.normalized * distance;

            // Smoothly interpolate towards the target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
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
    public void OnClick()
    {
        // Increment the smoothTime index
        smoothTimeIndex = (smoothTimeIndex + 1) % smoothTimeValues.Length;

        // Update smoothTime to the value at the current index
        smoothTime = smoothTimeValues[smoothTimeIndex];

        Debug.Log("smoothTime toggled to: " + smoothTime);
        Speed.text = "Speed: " + smoothTime.ToString();
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
}
