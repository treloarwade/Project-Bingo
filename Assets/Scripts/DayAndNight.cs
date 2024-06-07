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
            StartCoroutine(TurntoNight(10f));
        }
    }
    public void Night()
    {
        isNight = true;
        StopAllCoroutines();
        StartCoroutine(TurntoNight(10f));
    }
    IEnumerator DayTime()
    {
        yield return new WaitForSeconds(670f);
        StartCoroutine(TurntoNight(10f));
        yield return null;
    }

    public void Day()
    {
        isNight = false;
        StopAllCoroutines();
        StartCoroutine(EndNight());
    }
    IEnumerator TurntoNight(float speed)
    {
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
        isNight = true;

        float[] greyLevels = { 0.95f, 0.90f, 0.85f, 0.80f, 0.75f, 0.70f, 0.65f, 0.60f, 0.55f, 0.50f };

        foreach (float greyLevel in greyLevels)
        {
            foreach (SpriteRenderer sprite in sprites)
            {
                Color currentColor = sprite.color;
                sprite.color = new Color(greyLevel, greyLevel, greyLevel, currentColor.a);
            }
            yield return new WaitForSeconds(5f / speed);
        }

        StartCoroutine(NightTime());
        yield return null;
    }

    IEnumerator NightTime()
    {
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites)
        {
            Color currentColor = sprite.color;
            sprite.color = new Color(0.5f, 0.5f, 0.5f, currentColor.a);
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
        StartCoroutine(TurntoDay(10f));
        yield return null;
    }
    IEnumerator TurntoDay(float speed)
    {
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
        isNight = false;

        float[] greyLevels = { 0.55f, 0.60f, 0.65f, 0.70f, 0.75f, 0.80f, 0.85f, 0.90f, 0.95f, 1.0f };

        foreach (float greyLevel in greyLevels)
        {
            foreach (SpriteRenderer sprite in sprites)
            {
                Color currentColor = sprite.color;
                sprite.color = new Color(greyLevel, greyLevel, greyLevel, currentColor.a);
            }
            yield return new WaitForSeconds(5f / speed);
        }

        StartCoroutine(DayTime());
        yield return null;
    }


}

