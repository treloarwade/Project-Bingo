using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item Item;

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
                    knifeLoader.ToggleKnife(); // Call the ToggleKnife method
                }
                else
                {
                    Debug.LogWarning("KnifeLoader script not found.");
                }
                break;
            // Add more cases for other items
            default:
                Debug.LogWarning("Unknown item ID: " + Item.ID);
                break;
        }

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

