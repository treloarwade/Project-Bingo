using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GridBrushBase;

public class BuoyScript : MonoBehaviour
{
    public float maxRotation = 2f;
    public float rotationSpeed = 5f;
    private float currentRotation = 0f;
    private float rotationDirection = 1f;
    Rigidbody2D body;
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        RotateSprite();
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
