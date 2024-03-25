using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DingoMoves; // Assuming you've defined DingoMoves namespace

public class Move1 : MonoBehaviour
{
    public HealthBar healthBar;
    public int maxHealth = 100;
    public int currentHealth = 99;
    public Text textField;
    public GameObject firstMove;

    // Declare damageamount at the class level
    private DingoMove damageamount;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        // Accessing a move from the MoveDatabase
        damageamount = MoveDatabase.Moves[0]; // Accessing the first move (index 0)
    }

    // Method to set the damage from a DingoMove object
    public void SetMoveDamage(int damage)
    {
        // Using the Damage of the move
        damage = damageamount.Damage;
    }

    public void OnMouseDown()
    {
        // Decrease health by the damage value
        currentHealth -= damageamount.Damage;
        // Update health bar
        healthBar.SetHealth(currentHealth);
    }
}

