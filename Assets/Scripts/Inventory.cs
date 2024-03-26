using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DingoSystem;
using DingoMoves; // Assuming you have a namespace for Dingo moves

public class PlayerInventory
{
    private List<PlayerDingo> dingos; // List to hold Dingos in the inventory

    public PlayerInventory()
    {
        dingos = new List<PlayerDingo>(); // Initialize the list
    }

    // Method to add a Dingo to the inventory
    public void AddDingo(PlayerDingo dingo)
    {
        if (dingos.Count < 3) // Check if the inventory is not full
        {
            dingos.Add(dingo); // Add the Dingo to the inventory
            Debug.Log("Added " + dingo.Nickname + " to the inventory.");
        }
        else
        {
            Debug.LogWarning("Inventory is full. Cannot add more Dingos.");
        }
    }

    // Method to remove a Dingo from the inventory
    public void RemoveDingo(PlayerDingo dingo)
    {
        if (dingos.Contains(dingo)) // Check if the Dingo is in the inventory
        {
            dingos.Remove(dingo); // Remove the Dingo from the inventory
            Debug.Log("Removed " + dingo.Nickname + " from the inventory.");
        }
        else
        {
            Debug.LogWarning(dingo.Nickname + " is not in the inventory. Cannot remove.");
        }
    }

    // Method to get the Dingos in the inventory
    public List<PlayerDingo> GetDingos()
    {
        return dingos;
    }
}

