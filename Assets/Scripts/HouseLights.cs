using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseLights : MonoBehaviour
{
    public SpriteRenderer[] redspottedmushroomhouses;

    public Sprite[] sprites;
    public void SwapSprite(SpriteRenderer[] houses, int id)
    {
        Sprite newSprite = sprites[id];
        foreach (SpriteRenderer house in houses)
        {
            house.sprite = newSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
