using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DingoSystem;
using Inventory;

public class InventoryButton : MonoBehaviour
{
    public Inventory.PlayerInventory inventory; // Reference to the player's inventory
    public Text inventoryText; // Reference to the UI text element to display inventory contents

    void Start()
    {
        if (inventoryText == null)
        {
            Debug.LogError("InventoryText reference is not set.");
            return;
        }

        if (inventory == null)
        {
            Debug.LogError("Inventory reference is not set.");
            return;
        }
    }

    // Method called when the button is clicked
    public void OnButtonClick()
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory or InventoryText reference is not set.");
            return;
        }

        // Get the Dingos in the inventory
        List<PlayerDingo> dingos = inventory.GetDingos();

        // Prepare inventory text
        System.Text.StringBuilder inventoryContent = new System.Text.StringBuilder();
        inventoryContent.Append("Inventory:\n");

        // Add each Dingo's nickname to the inventory text
        foreach (PlayerDingo dingo in dingos)
        {
            inventoryContent.Append("- ").Append(dingo.Nickname).Append("\n");
        }

        // Display inventory content in UI text element
        inventoryText.text = inventoryContent.ToString();
    }
}
