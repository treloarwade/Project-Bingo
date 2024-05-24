using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GardenerScript : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    public Interactor interactor;
    public bool Trigger1;
    public bool Trigger2;
    public bool Trigger3;
    public bool Trigger4;
    public bool Trigger5;
    public Sprite[] frames;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        int count = Flower.destroyedFlowerCount;
        if (count >= 1 && count <= 49 && !Trigger1)
        {
            dialogBox.SetActive(true);
            dialogText.text = "Gardener: Please be careful not to step on my flowers...";
            Trigger1 = true;
            spriteRenderer.sprite = frames[0];
        }
        else if (count >= 50 && count <= 99 && !Trigger2)
        {
            dialogBox.SetActive(true);
            dialogText.text = "Gardener: Why are you doing this?";
            Trigger2 = true;
            spriteRenderer.sprite = frames[1];
        }
        else if (count >= 100 && count <= 199 && !Trigger3)
        {
            dialogBox.SetActive(true);
            dialogText.text = "Gardener: STOP STEPPING ON MY FLOWERS";
            Trigger3 = true;
        }
        else if (count >= 200 && !Trigger4)
        {
            dialogBox.SetActive(true);
            dialogText.text = "Gardener: The garden is devastated";
            Trigger4 = true;
        }
    }
    public void Shingy()
    {
        if (!dialogBox.activeSelf)
        {
            dialogBox.SetActive(true);

            int count = Flower.destroyedFlowerCount;
            Debug.Log($"Destroyed Flower Count: {count}");

            // Set dialog text based on the number of destroyed flowers
            if (count == 0)
            {
                dialogText.text = "Gardener: Feel free to pick one.";
            }
            else if (count >= 1 && count <= 49)
            {
                dialogText.text = "Gardener: Please be careful not to step on my flowers...";
            }
            else if (count >= 50 && count <= 99)
            {
                dialogText.text = "Gardener: STOP STEPPING ON MY FLOWERS";
            }
            else if (count >= 100 && count <= 199)
            {
                dialogText.text = "Gardener: STOP STEPPING ON MY FLOWERS";
            }
            else if (count >= 200)
            {
                dialogText.text = "**I'll insert a difficult battle here**";
            }
            interactor.TurnOff();
        }
    }
}
