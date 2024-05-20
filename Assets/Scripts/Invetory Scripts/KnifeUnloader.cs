using UnityEngine;

public class KnifeUnloader : MonoBehaviour
{
    public int itemIDToCheck = 0; // The ID of the item to check

    private void Awake()
    {
        // Check if the InventoryManager instance is initialized
        if (InventoryManager.Instance != null)
        {
            // Check if the item is already in the inventory
            if (HasItem())
            {
                // Unload (deactivate) this GameObject if the item is found
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("InventoryManager instance is not initialized.");
        }
    }

    // Method to check if the item exists in the inventory
    private bool HasItem()
    {
        foreach (var item in InventoryManager.Instance.Items)
        {
            if (item.ID == itemIDToCheck)
            {
                return true;
            }
        }
        return false;
    }
}
