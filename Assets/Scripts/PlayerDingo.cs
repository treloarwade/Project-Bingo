using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using DingoIDs;

public class DingoSelector : MonoBehaviour
{
    public List<DingoIDs.DingoID> availableDingos; // List of available Dingos

    private void Start()
    {
        // Select a random Dingo from the list
        DingoIDs.DingoID dingoOnLeft = GetRandomDingo();

        // Display Dingo information (optional)
        Debug.Log("Dingo on the left:");
        DisplayDingoInfo(dingoOnLeft);
    }

    // Method to get a random Dingo from the list
    private DingoIDs.DingoID GetRandomDingo()
    {
        int randomIndex = Random.Range(0, availableDingos.Count);
        return availableDingos[randomIndex];
    }

    // Method to display Dingo information (optional)
    private void DisplayDingoInfo(DingoIDs.DingoID dingo)
    {
        Debug.Log($"Name: {dingo.Name}");
        Debug.Log($"Type: {dingo.Type}");
        Debug.Log($"Description: {dingo.Description}");
        Debug.Log($"HP: {dingo.HP}");
        Debug.Log($"Attack: {dingo.Attack}");
        Debug.Log($"Defense: {dingo.Defense}");
        Debug.Log($"Speed: {dingo.Speed}");
        Debug.Log($"Sprite: {dingo.Sprite}");
    }
}

