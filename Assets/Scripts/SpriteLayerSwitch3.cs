using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLayerSwitch3 : MonoBehaviour
{
    public GameObject bingo;
    public Collider2D above;
    public Collider2D above2;
    public Collider2D below;
    public Collider2D below2;
    public float height;
    void Update()
    {
        // Check if the bingo GameObject's Y position is below a certain threshold
        if (bingo.transform.position.y < height)
        {
            // Switch to the background sorting layer
            SwitchSortingLayer("background");

            // Enable the "below" collider and disable the "above" collider
            if (below != null)
                below.enabled = true;
            if (below2 != null)
                below2.enabled = true;
            if (above != null)
                above.enabled = false;
            if (above2 != null)
                above2.enabled = false;
        }
        else
        {
            // Switch to the default sorting layer
            SwitchSortingLayer("Default");

            // Enable the "above" collider and disable the "below" collider
            if (above != null)
                above.enabled = true;
            if (above2 != null)
                above2.enabled = true;
            if (below != null)
                below.enabled = false;
            if (below2 != null)
                below2.enabled = false;
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
