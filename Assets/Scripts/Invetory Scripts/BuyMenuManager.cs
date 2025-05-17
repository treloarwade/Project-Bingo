using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class BuyMenuManager : NetworkBehaviour
{
    public static BuyMenuManager Instance;
    public int[] itemList;
    public Transform ItemContent;
    public GameObject BuyItemPrefab;
    public ItemDatabase itemDatabase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void DisplayItemsForSale(List<int> itemIDs)
    {
        // Clear existing items
        foreach (Transform child in ItemContent)
        {
            Destroy(child.gameObject);
        }

        // Create new items for sale
        foreach (int itemID in itemIDs)
        {
            Item item = itemDatabase.GetItemByID(itemID);
            if (item != null)
            {
                GameObject buyItem = Instantiate(BuyItemPrefab, ItemContent);

                // Set item details
                var itemName = buyItem.transform.Find("ItemName").GetComponent<Text>();
                var itemIcon = buyItem.transform.Find("ItemIcon").GetComponent<Image>();
                var itemPrice = buyItem.transform.Find("ItemPrice").GetComponent<Text>();
                RectTransform iconRect = itemIcon.GetComponent<RectTransform>();
                iconRect.sizeDelta *= 0.75f;

                itemName.text = item.Name;
                itemIcon.sprite = item.Icon;
                itemPrice.text = "Price: $" + item.Price;
                // Add click listener
                Button button = buyItem.GetComponent<Button>();
                button.onClick.AddListener(() => OnBuyItemClick(itemID));
            }
        }
        InventoryManager.Instance.ListItems(false);
    }
    private void OnBuyItemClick(int itemID)
    {
        // Get the item from database
        Item itemToAdd = itemDatabase.GetItemByID(itemID);
        if (itemToAdd != null)
        {
            // Check if player has enough money
            if (InventoryManager.Instance.TryRemoveMoney(itemToAdd.Price))
            {
                // Add the item to inventory
                InventoryManager.Instance.Add(itemToAdd);
                Debug.Log($"Purchased item {itemID} for {itemToAdd.Price} money");
            }
            else
            {
                Debug.Log($"Not enough money to buy item {itemID}. Price: {itemToAdd.Price}");
                // You might want to add some visual/audio feedback here for the player
            }
        }
        else
        {
            Debug.LogWarning($"Item with ID {itemID} not found in database");
        }
        InventoryManager.Instance.ListItems(false);
    }
    public void OnSellItemClick(int itemID)
    {
        // Get the item from database
        Item itemToAdd = itemDatabase.GetItemByID(itemID);
        if (itemToAdd != null)
        {
            // Check if player has enough money
            InventoryManager.Instance.AddMoney(itemToAdd.Price);
            InventoryManager.Instance.Remove(itemToAdd);

        }
        else
        {
            Debug.LogWarning($"Item with ID {itemID} not found in database");
        }
        InventoryManager.Instance.ListItems(false);
    }

    // Overload to use the default itemList array
    public void DisplayItemsForSale()
    {
        DisplayItemsForSale(new List<int>(itemList));
    }
}