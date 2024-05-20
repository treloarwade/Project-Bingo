using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        StartCoroutine(RandomlyChangeDirection());
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

    private IEnumerator RandomlyChangeDirection()
    {
        while (true)
        {
            // Wait for a random amount of time
            yield return new WaitForSeconds(Random.Range(1f, 5f));

            // Randomly change rotation direction
            rotationDirection = Random.Range(0, 2) == 0 ? -1f : 1f;
        }
    }
}
