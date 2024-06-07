using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class NightLights : MonoBehaviour
{
    public SpriteRenderer[] misc;
    public SpriteRenderer[] mushroomhouses;
    public SpriteRenderer[] spottedmushroomhouses;
    public SpriteRenderer[] bluemushroomhouses;
    public SpriteRenderer[] bluespottedmushroomhouses;
    public SpriteRenderer[] redmushroomhouses;
    public SpriteRenderer[] redspottedmushroomhouses;
    public SpriteRenderer[] houses;
    public SpriteRenderer[] redhouses;

    public Sprite[] sprites;
    public void SwapSprite(SpriteRenderer[] houses, int id)
    {
        Sprite newSprite = sprites[id];
        foreach (SpriteRenderer house in houses)
        {
            house.sprite = newSprite;
        }
    }
    public void TurnLightsOnForAllMushroomHouses()
    {
        SwapSpriteForHouses(13);
        SwapSpriteForRedHouses(15);
        SwapSpriteForMushroomHouses(1);
        SwapSpriteForSpottedMushroomHouses(3);
        SwapSpriteForRedMushroomHouses(5);
        SwapSpriteForRedSpottedMushroomHouses(7);
        SwapSpriteForBlueMushroomHouses(9);
        SwapSpriteForBlueSpottedMushroomHouses(11);
        EnableAllChildGameObjectsForArray(misc);
        EnableAllChildGameObjectsForArray(houses);
        EnableAllChildGameObjectsForArray(redhouses);
        EnableAllChildGameObjectsForArray(mushroomhouses);
        EnableAllChildGameObjectsForArray(spottedmushroomhouses);
        EnableAllChildGameObjectsForArray(bluemushroomhouses);
        EnableAllChildGameObjectsForArray(bluespottedmushroomhouses);
        EnableAllChildGameObjectsForArray(redmushroomhouses);
        EnableAllChildGameObjectsForArray(redspottedmushroomhouses);
    }
    public void TurnLightsOffForAllMushroomHouses()
    {
        DisableAllChildGameObjectsForArray(misc);
        DisableAllChildGameObjectsForArray(houses);
        DisableAllChildGameObjectsForArray(redhouses);
        DisableAllChildGameObjectsForArray(mushroomhouses);
        DisableAllChildGameObjectsForArray(spottedmushroomhouses);
        DisableAllChildGameObjectsForArray(bluemushroomhouses);
        DisableAllChildGameObjectsForArray(bluespottedmushroomhouses);
        DisableAllChildGameObjectsForArray(redmushroomhouses);
        DisableAllChildGameObjectsForArray(redspottedmushroomhouses);
        SwapSpriteForHouses(12);
        SwapSpriteForRedHouses(14);
        SwapSpriteForMushroomHouses(0);
        SwapSpriteForSpottedMushroomHouses(2);
        SwapSpriteForRedMushroomHouses(4);
        SwapSpriteForRedSpottedMushroomHouses(6);
        SwapSpriteForBlueMushroomHouses(8);
        SwapSpriteForBlueSpottedMushroomHouses(10);
    }

    public void SwapSpriteForMushroomHouses(int id)
    {
        SwapSprite(mushroomhouses, id);
    }

    public void SwapSpriteForSpottedMushroomHouses(int id)
    {
        SwapSprite(spottedmushroomhouses, id);
    }

    public void SwapSpriteForBlueMushroomHouses(int id)
    {
        SwapSprite(bluemushroomhouses, id);
    }

    public void SwapSpriteForBlueSpottedMushroomHouses(int id)
    {
        SwapSprite(bluespottedmushroomhouses, id);
    }

    public void SwapSpriteForRedMushroomHouses(int id)
    {
        SwapSprite(redmushroomhouses, id);
    }

    public void SwapSpriteForRedSpottedMushroomHouses(int id)
    {
        SwapSprite(redspottedmushroomhouses, id);
    }
    public void SwapSpriteForHouses(int id)
    {
        SwapSprite(houses, id);
    }
    public void SwapSpriteForRedHouses(int id)
    {
        SwapSprite(redhouses, id);
    }
    public void House(SpriteRenderer[] houses, float amount)
    {
        foreach (SpriteRenderer house in houses)
        {
            house.color = new Color(amount, amount, amount, 1);
        }
    }
    public void ChangeColor(float amount)
    {   
        House(misc, amount);
        House(mushroomhouses, amount);
        House(spottedmushroomhouses, amount);
        House(bluemushroomhouses, amount);
        House(bluespottedmushroomhouses, amount);
        House(redmushroomhouses, amount);
        House(redspottedmushroomhouses, amount);
        House(houses, amount);
        House(redhouses, amount);
    }
    public void EnableAllChildGameObjects(SpriteRenderer parentRenderer)
    {
        foreach (Transform child in parentRenderer.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void EnableAllChildGameObjectsForArray(SpriteRenderer[] renderers)
    {
        foreach (SpriteRenderer renderer in renderers)
        {
            EnableAllChildGameObjects(renderer);
        }
    }
    public void DisableAllChildGameObjects(SpriteRenderer parentRenderer)
    {
        foreach (Transform child in parentRenderer.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void DisableAllChildGameObjectsForArray(SpriteRenderer[] renderers)
    {
        foreach (SpriteRenderer renderer in renderers)
        {
            DisableAllChildGameObjects(renderer);
        }
    }

}
