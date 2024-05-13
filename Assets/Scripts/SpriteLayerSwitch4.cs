using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLayerSwitch4 : MonoBehaviour
{
    public GameObject bingo;
    public float height;
    void Update()
    {
        Debug.Log(bingo.transform.position);
        // Check if the bingo GameObject's Y position is below a certain threshold
        if (bingo.transform.position.y < height)
        {
            // Switch to the background sorting layer
            SwitchSortingLayer("background");

        }
        else
        {
            // Switch to the default sorting layer
            SwitchSortingLayer("Default");

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
