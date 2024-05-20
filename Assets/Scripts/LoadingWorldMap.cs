using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingWorldMap : MonoBehaviour
{
    public GameObject objectToManage;  // Assign the GameObject to manage in the Inspector

    // Function to load (activate) the GameObject
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object has the Player tag (or any other condition you want)
        if (other.CompareTag("Player"))
        {
            LoadGameObject();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the colliding object has the Player tag (or any other condition you want)
        if (other.CompareTag("Player"))
        {
            UnloadGameObject();
        }
    }
    public void LoadGameObject()
    {
        if (objectToManage != null)
        {
            objectToManage.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No GameObject assigned to manage.");
        }
    }

    // Function to unload (deactivate) the GameObject
    public void UnloadGameObject()
    {
        if (objectToManage != null)
        {
            objectToManage.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No GameObject assigned to manage.");
        }
    }
}
