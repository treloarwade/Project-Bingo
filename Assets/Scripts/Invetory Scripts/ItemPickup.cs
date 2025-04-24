using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item Item;
    public InventoryManager inventoryManager;

    private void ReloadMenu()
    {
        // Find the InventoryManager in the scene
        inventoryManager = FindObjectOfType<InventoryManager>();

        // Ensure the InventoryManager was found
        if (inventoryManager != null)
        {
            // Call the method
            inventoryManager.InstantiateInventoryItems();
        }
        else
        {
            Debug.LogError("InventoryManager not found in the scene.");
        }
    }
    void Pickup()
    {
        InventoryManager.Instance.Add(Item);

        // Switch statement to handle different item interactions based on item ID
        switch (Item.ID)
        {
            case 0: // Example: Equip Knife
                KnifeLoader knifeLoader = FindObjectOfType<KnifeLoader>(); // Find the KnifeLoader script
                if (knifeLoader != null)
                {
                    knifeLoader.EquipKnife(Item.ID); // Call the ToggleKnife method
                }
                else
                {
                    Debug.LogWarning("KnifeLoader script not found.");
                }
                break;
            case 1: // Example: Equip Knife
                KnifeLoader knifeLoader1 = FindObjectOfType<KnifeLoader>(); // Find the KnifeLoader script
                if (knifeLoader1 != null)
                {
                    knifeLoader1.EquipKnife(Item.ID); // Call the ToggleKnife method
                }
                else
                {
                    Debug.LogWarning("KnifeLoader script not found.");
                }
                break;
            case 2: // Example: Equip Knife
                KnifeLoader knifeLoader2 = FindObjectOfType<KnifeLoader>(); // Find the KnifeLoader script
                if (knifeLoader2 != null)
                {
                    knifeLoader2.EquipKnife(Item.ID); // Call the ToggleKnife method
                }
                else
                {
                    Debug.LogWarning("KnifeLoader script not found.");
                }
                break;
            default:
                Debug.LogWarning("Unknown item ID: " + Item.ID);
                break;
        }
        ReloadMenu();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Pickup();
        }
    }
}

