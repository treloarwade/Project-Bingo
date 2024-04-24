using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Rotate the object
        float rotationSpeed = 100f; // Adjust rotation speed as needed
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        // Generate a random number between 0 and 9
        int randomNumber = Random.Range(0, 10);

        // Check if the random number is less than 1 (which happens 1/10 times)
        if (randomNumber < 1)
        {
            // Trigger the collision event
            Loader.Load(Loader.Scene.Battle);
        }
    }
}
