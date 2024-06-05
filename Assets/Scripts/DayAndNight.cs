using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAndNight : MonoBehaviour
{
    public bool isNight = false; // Track if it is currently night
    private NightLights nightLights;
    public void ToggleDayNight()
    {
        if (isNight)
        {
            Day();
            Debug.Log("bingo");
        }
        else
        {
            Night();
        }

        // Toggle the state
    }
    public void TurnToNightIf()
    {
        if (!isNight)
        {
            isNight = true;
            StopAllCoroutines();
            StartCoroutine(TurntoNight());
        }
    }
    public void Night()
    {
        isNight = true;
        StartCoroutine(NightTime());
    }
    IEnumerator DayTime()
    {
        yield return new WaitForSeconds(670f);
        StartCoroutine(TurntoNight());
        yield return null;
    }

    public void Day()
    {
        isNight = false;
        StartCoroutine(EndNight());
    }
    IEnumerator TurntoNight()
    {

        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
        isNight = true;
        foreach (SpriteRenderer sprite in sprites)
        {
            // Convert sprite color to grey
            sprite.color = new Color(0.95f, 0.95f, 0.95f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            // Convert sprite color to grey
            sprite.color = new Color(0.90f, 0.90f, 0.90f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            // Convert sprite color to grey
            sprite.color = new Color(0.85f, 0.85f, 0.85f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            // Convert sprite color to grey
            sprite.color = new Color(0.80f, 0.80f, 0.80f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            // Convert sprite color to grey
            sprite.color = new Color(0.75f, 0.75f, 0.75f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            // Convert sprite color to grey
            sprite.color = new Color(0.70f, 0.70f, 0.70f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.color = new Color(0.65f, 0.65f, 0.65f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.color = new Color(0.60f, 0.60f, 0.60f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.color = new Color(0.55f, 0.55f, 0.55f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            // Convert sprite color to grey
            sprite.color = Color.grey;
        }
        StartCoroutine(NightTime());
        yield return null;
    }
    IEnumerator NightTime()
    {
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites)
        {
            // Convert sprite color to grey
            sprite.color = Color.grey;
        }
        nightLights = GetComponent<NightLights>();
        nightLights.TurnLightsOnForAllMushroomHouses();
        nightLights.ChangeColor(0.80f);
        yield return new WaitForSeconds(670f);
        StartCoroutine(EndNight());
        yield return null;
    }
    IEnumerator EndNight()
    {
        nightLights.TurnLightsOffForAllMushroomHouses();
        StartCoroutine(TurntoDay());
        yield return null;
    }
    IEnumerator TurntoDay()
    {
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
        isNight = false;
        foreach (SpriteRenderer sprite in sprites)
        {
            // Lighten sprite color
            sprite.color = new Color(0.55f, 0.55f, 0.55f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            // Lighten sprite color
            sprite.color = new Color(0.60f, 0.60f, 0.60f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            // Lighten sprite color
            sprite.color = new Color(0.65f, 0.65f, 0.65f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            // Lighten sprite color
            sprite.color = new Color(0.70f, 0.70f, 0.70f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            // Lighten sprite color
            sprite.color = new Color(0.75f, 0.75f, 0.75f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            // Lighten sprite color
            sprite.color = new Color(0.80f, 0.80f, 0.80f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            // Lighten sprite color
            sprite.color = new Color(0.85f, 0.85f, 0.85f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            // Lighten sprite color
            sprite.color = new Color(0.90f, 0.90f, 0.90f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            // Lighten sprite color
            sprite.color = new Color(0.95f, 0.95f, 0.95f, 1);
        }
        yield return new WaitForSeconds(5f);
        foreach (SpriteRenderer sprite in sprites)
        {
            // Restore original sprite color
            sprite.color = Color.white;
        }
        StartCoroutine(DayTime());
        yield return null;
    }

}

