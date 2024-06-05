using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class NightLights : MonoBehaviour
{
    public GameObject[] nightlights;
    public SpriteRenderer[] mushroomhouses;
    public Sprite[] sprites;
    public void TurnOffNightLights()
    {
        foreach (GameObject light in nightlights)
        {
            light.SetActive(false);
        }
    }
    public void TurnOnNightLights()
    {
        foreach (GameObject light in nightlights)
        {
            light.SetActive(true);
        }
    }

    public void SwapSprite(int id)
    {
        Sprite newSprite = sprites[id];
        foreach (SpriteRenderer house in mushroomhouses)
        {
            house.sprite = newSprite;
        }
    }
    public void ChangeColor(float amount)
    {
        foreach (SpriteRenderer house in mushroomhouses)
        {
            house.color = new Color(amount, amount, amount, 1);
        }

    }
}
