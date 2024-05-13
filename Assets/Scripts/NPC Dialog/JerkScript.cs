using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JerkScript : MonoBehaviour
{
    public Sprite[] frames;
    public GameObject dialogBox;
    public Text dialogText;
    private float lastActivationTime;
    private int interactionCount = 0;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (Time.time - lastActivationTime >= 5f)
        {
            dialogBox.SetActive(true);
            lastActivationTime = Time.time;

            // Increment the interaction count
            interactionCount++;

            // Set dialog based on interaction count
            switch (interactionCount)
            {
                case 1:
                    dialogText.text = "Rich Jerk: I guess I have to teach you a lesson in Finance scrub.";
                    break;
                case 2:
                    dialogText.text = "Rich Jerk: You think that's funny? My dad works at company, I can get you banned.";
                    spriteRenderer.sprite = frames[0];
                    break;
                case 3:
                    dialogText.text = "Rich Jerk: Get out of here before I tell my dad to ban you.";
                    spriteRenderer.sprite = frames[1];
                    break;
                default:
                    // If the player interacts more than three times, reset the interaction count
                    dialogText.text = "Rich Jerk: Get out of here before I tell my dad to ban you.";
                    break;
            }
        }
    }
}


