using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Move1 : MonoBehaviour
{
    public HealthBar healthBar;
    public int maxHealth = 100;
    public int currentHealth = 99;
    public int damage = 3;
    public Text Textfield;

    public GameObject FirstMove;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }


    // Update is called once per frame
    public void OnMouseDown()
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
    }

    public void SetText(string text)
    {
        Textfield.text = text;
    }
}
