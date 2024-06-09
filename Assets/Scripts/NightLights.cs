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
    public SpriteRenderer[] skyscraperupper1;
    public SpriteRenderer[] skyscraperlower1;
    public SpriteRenderer[] skyscraperupper2;
    public SpriteRenderer[] skyscraperlower2;
    public SpriteRenderer[] skyscraperupper3;
    public SpriteRenderer[] skyscraperlower3;

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
        SwapSpriteForSkyscraperUpper1(19);
        SwapSpriteForSkyscraperLower1(17);
        SwapSpriteForSkyscraperUpper2(23);
        SwapSpriteForSkyscraperLower2(21);
        SwapSpriteForSkyscraperUpper3(27);
        SwapSpriteForSkyscraperLower3(25);
        EnableAllChildGameObjectsForArray(misc);
        EnableAllChildGameObjectsForArray(skyscraperlower1);
        EnableAllChildGameObjectsForArray(skyscraperupper1);
        EnableAllChildGameObjectsForArray(skyscraperlower2);
        EnableAllChildGameObjectsForArray(skyscraperupper2);
        EnableAllChildGameObjectsForArray(skyscraperlower3);
        EnableAllChildGameObjectsForArray(skyscraperupper3);
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
        DisableAllChildGameObjectsForArray(skyscraperlower1);
        DisableAllChildGameObjectsForArray(skyscraperupper1);
        DisableAllChildGameObjectsForArray(skyscraperlower2);
        DisableAllChildGameObjectsForArray(skyscraperupper2);
        DisableAllChildGameObjectsForArray(skyscraperlower3);
        DisableAllChildGameObjectsForArray(skyscraperupper3);
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
        SwapSpriteForSkyscraperUpper1(18);
        SwapSpriteForSkyscraperLower1(16);
        SwapSpriteForSkyscraperUpper2(22);
        SwapSpriteForSkyscraperLower2(20);
        SwapSpriteForSkyscraperUpper3(26);
        SwapSpriteForSkyscraperLower3(24);
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
    public void SwapSpriteForSkyscraperUpper1(int id)
    {
        SwapSprite(skyscraperupper1, id);
    }
    public void SwapSpriteForSkyscraperLower1(int id)
    {
        SwapSprite(skyscraperlower1, id);
    }
    public void SwapSpriteForSkyscraperUpper2(int id)
    {
        SwapSprite(skyscraperupper2, id);
    }
    public void SwapSpriteForSkyscraperLower2(int id)
    {
        SwapSprite(skyscraperlower2, id);
    }
    public void SwapSpriteForSkyscraperUpper3(int id)
    {
        SwapSprite(skyscraperupper3, id);
    }
    public void SwapSpriteForSkyscraperLower3(int id)
    {
        SwapSprite(skyscraperlower3, id);
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
        House(skyscraperupper1, amount);
        House(skyscraperlower1, amount);
        House(skyscraperupper2, amount);
        House(skyscraperlower2, amount);
        House(skyscraperupper3, amount);
        House(skyscraperlower3, amount);
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
