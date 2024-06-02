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

    private void Start()
    {
        currentlyEquipped = -1;
    }
    // Toggle between equipping and unequipping the knife
    public void ToggleKnife(int ID)
    {
            EquipKnife(ID);
    }
    public void Inspect()
    {
        StartCoroutine(InspectAnimation());
    }
    IEnumerator InspectAnimation()
    {

        float startTime = Time.time;
        Renderer renderer = Knife.GetComponent<Renderer>();
        Vector3 originalPosition = Knife.transform.localPosition;
        Quaternion originalRotation = Knife.transform.rotation;
        Vector3 moveposition = new Vector3(-0.352f, -0.1f, 0);
        while (Time.time - startTime < 1)
        {
            float fracJourney = (Time.time - startTime) / 1;
            Knife.transform.localPosition = Vector3.Lerp(originalPosition, moveposition, fracJourney);
            yield return null;
        }
        if (renderer != null)
        {
            renderer.sortingOrder = 7;
        }
        else
        {
            Debug.LogError("Renderer component not found on " + Knife.name);
        }
        startTime = Time.time;
        while (Time.time - startTime < 5)
        {
            Knife.transform.Rotate(Vector3.forward, 0.7f);
            yield return null;
        }
        if (renderer != null)
        {
            renderer.sortingOrder = 5;
        }
        else
        {
            Debug.LogError("Renderer component not found on " + Knife.name);
        }
        startTime = Time.time;
        while (Time.time - startTime < 1)
        {
            float fracJourney = (Time.time - startTime) / 1;
            Knife.transform.localPosition = Vector3.Lerp(moveposition, originalPosition, fracJourney);
            Knife.transform.localRotation = Quaternion.Slerp(Knife.transform.localRotation, originalRotation, fracJourney);
            yield return null;
        }

        yield return null;
    }
    private void EquipKnife(int ID)
    {
        if (Knife.activeSelf)
        {
            if (currentlyEquipped == ID)
            {
                movement.DecreaseRunSpeed();
                Knife.SetActive(false);
            }
        }
        else
        {
            Knife.SetActive(true);
            movement.IncreaseRunSpeed();
        }
        SpriteRenderer renderer = Knife.GetComponent<SpriteRenderer>();
        renderer.sprite = Color[ID];
        currentlyEquipped = ID;
    }

    private void UnequipKnife()
    {
        movement.DecreaseRunSpeed();
        Knife.SetActive(false);
    }
}
