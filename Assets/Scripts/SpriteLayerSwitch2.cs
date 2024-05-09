using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class SpriteLayerSwitcher2 : MonoBehaviour
{
    public GameObject bingo;
    public Collider2D above;
    public Collider2D below;
    public float height;
    void Update()
    {
        Debug.Log(bingo.transform.position);
        // Check if the bingo GameObject's Y position is below a certain threshold
        if (bingo.transform.position.y < height)
        {
            // Switch to the background sorting layer
            SwitchSortingLayer("background");

            // Enable the "below" collider and disable the "above" collider
            if (below != null)
                below.enabled = true;
            if (above != null)
                above.enabled = false;
        }
        else
        {
            // Switch to the default sorting layer
            SwitchSortingLayer("Default");

            // Enable the "above" collider and disable the "below" collider
            if (above != null)
                above.enabled = true;
            if (below != null)
                below.enabled = false;
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
