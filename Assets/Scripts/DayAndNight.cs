using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAndNight : MonoBehaviour
{
    public bool isNight = false; // Track if it is currently night

    public void ToggleDayNight()
    {
        if (isNight)
        {
            Day();
        }
        else
        {
            Night();
        }

        // Toggle the state
        isNight = !isNight;
    }

    public void Night()
    {
        // Find all GameObjects with a SpriteRenderer component
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();

        foreach (SpriteRenderer sprite in sprites)
        {
            // Convert sprite color to grey
            sprite.color = Color.grey;
        }
    }

    public void Day()
    {
        // Find all GameObjects with a SpriteRenderer component
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();

        foreach (SpriteRenderer sprite in sprites)
        {
            // Convert sprite color to white
            sprite.color = Color.white;
        }
    }
}

