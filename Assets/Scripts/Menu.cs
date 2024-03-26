using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DingoSystem;

public class InventoryButton : MonoBehaviour
{
    public PlayerInventory inventory; // Reference to the player's inventory
    public Text inventoryText; // Reference to the UI text element to display inventory contents

    void Start()
    {
        // Find the PlayerInventory object in the scene
        // Find the UI text element in the scene
        inventoryText = GameObject.Find("InventoryText").GetComponent<Text>(); // Replace "InventoryText" with the name of your UI text element
    }

    // Method called when the button is clicked
    public void OnButtonClick()
    {
        // Get the Dingos in the inventory
        List<PlayerDingo> dingos = inventory.GetDingos();

        // Prepare inventory text
        string inventoryContent = "Inventory:\n";

        // Add each Dingo's nickname to the inventory text
        foreach (PlayerDingo dingo in dingos)
        {
            inventoryContent += "- " + dingo.Nickname + "\n";
        }

        // Display inventory content in UI text element
        inventoryText.text = inventoryContent;
    }
}
