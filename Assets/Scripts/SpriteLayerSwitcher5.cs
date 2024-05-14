using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLayerSwitcher5 : MonoBehaviour
{
    private bool playerInsideTrigger = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInsideTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInsideTrigger = false;
        }
    }

    private void Update()
    {
        // If the player is inside the trigger, keep the sorting layer as "Default"
        // Otherwise, switch it to "Shoes"
        if (!playerInsideTrigger)
        {
            SwitchSortingLayer("Default");
        }
        else
        {
            SwitchSortingLayer("shoes");
        }
    }

    public void SwitchSortingLayer(string sortingLayerName)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("Renderer component not found.");
            return;
        }

        renderer.sortingLayerName = sortingLayerName;
    }
}
