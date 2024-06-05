using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLayerSwitcher5 : MonoBehaviour
{
    public GameObject bingo;
    public float height;
    void Update()
    {
        // Check if the bingo GameObject's Y position is below a certain threshold
        if (bingo.transform.position.y < height)
        {
            // Switch to the background sorting layer
            SwitchSortingLayer("shoes");

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
