using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class KnifeLoader : MonoBehaviour
{
    public GameObject Knife;
    public Sprite[] Color;
    public Movement movement;
    public int currentlyEquipped;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isCoroutineRunning = false;

    private void Start()
    {
        currentlyEquipped = -1;
        originalPosition = Knife.transform.localPosition;
        originalRotation = Knife.transform.rotation;
    }
    public void Inspect()
    {
        StartCoroutine(InspectAnimation());
    }
    IEnumerator InspectAnimation()
    {
        float startTime = Time.time;
        Renderer renderer = Knife.GetComponent<Renderer>();
        Vector3 moveposition = new Vector3(-0.352f, -0.1f, 0);
        if (!isCoroutineRunning)
        {
            while (Time.time - startTime < 1)
            {
                float fracJourney = (Time.time - startTime) / 1;
                Knife.transform.localPosition = Vector3.Lerp(originalPosition, moveposition, fracJourney);
                yield return null;
            }
        }

        startTime = Time.time;
        while (Time.time - startTime < 5)
        {
            Knife.transform.Rotate(Vector3.forward, 0.7f);
            Knife.transform.localPosition = moveposition;
            renderer.sortingOrder = 7;
            isCoroutineRunning = true;
            yield return null;
        }
        isCoroutineRunning = false;
        yield return new WaitForSeconds(0.05f);
        if (renderer != null)
        {
            renderer.sortingOrder = 5;
        }
        if (!isCoroutineRunning)
        {
            startTime = Time.time;
            while (Time.time - startTime < 1)
            {
                float fracJourney = (Time.time - startTime) / 1;
                Knife.transform.localPosition = Vector3.Lerp(moveposition, originalPosition, fracJourney);
                Knife.transform.localRotation = Quaternion.Slerp(Knife.transform.localRotation, originalRotation, fracJourney);
                yield return null;
            }
        }
        yield return null;
    }
    public void EquipKnife(int ID)
    {
        SpriteRenderer renderer = Knife.GetComponent<SpriteRenderer>();
        if (renderer.enabled)
        {
            if (currentlyEquipped == ID)
            {
                movement.DecreaseRunSpeed();
                renderer.enabled = false;
            }
        }
        else
        {
            renderer.enabled = true;
            movement.IncreaseRunSpeed();
        }
        renderer.sprite = Color[ID];
        currentlyEquipped = ID;
    }

    public void UnequipKnife()
    {
        SpriteRenderer renderer = Knife.GetComponent<SpriteRenderer>();
        if (renderer.enabled)
        {
            movement.DecreaseRunSpeed();
            renderer.enabled = false;
        }
    }
}
