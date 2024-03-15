using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInteraction : MonoBehaviour
{
    private void Update()
    {
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Get the first touch

            // Check if the touch phase is on the object
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position); // Create a ray from the touch position
                RaycastHit hit;

                // Check if the ray hits an object with a collider
                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the object hit by the ray is this object
                    if (hit.collider.gameObject == gameObject)
                    {
                        // Perform interaction logic here
                        Debug.Log("Touched object: " + gameObject.name);
                    }
                }
            }
        }
    }
}
