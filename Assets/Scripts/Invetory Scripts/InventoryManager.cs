using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class ItemData
{
    public int ID;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> Items = new List<Item>();
    public ItemDatabase itemDatabase;

    public Transform ItemContent;
    public GameObject InventoryItem;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Add(Item item)
    {
        Items.Add(item);
        SaveInventory();
    }

    public void Remove(Item item)
    {
        Items.Remove(item);
        SaveInventory();
    }

    public void ListItems()
    {
        LoadInventory();
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in Items)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();

            itemName.text = item.Name;
            itemIcon.sprite = item.Icon;
        }
    }
    public void InstantiateInventoryItems()
    {
        LoadInventory();
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in Items)
        {
            GameObject inventoryItem = Instantiate(InventoryItem, ItemContent);
            Button button = inventoryItem.GetComponent<Button>();
            int itemId = item.ID; // Store item ID in a local variable to avoid closure issues

            // Add an event listener to the button's onClick event
            button.onClick.AddListener(() => OnInventoryItemClick(itemId));
            var itemName = inventoryItem.transform.Find("ItemName").GetComponent<Text>();
            var itemIcon = inventoryItem.transform.Find("ItemIcon").GetComponent<Image>();

            itemName.text = item.Name;
            itemIcon.sprite = item.Icon;
            // Customize the inventory item UI element (e.g., set text and icon)
            // ...
        }
    }

    // Method to handle item click
    private void OnInventoryItemClick(int itemId)
    {
        // Switch statement to handle different item interactions based on item ID
        switch (itemId)
        {
            case 0: // Example: Equip Knife
                Debug.Log("Equipping Knife");
                KnifeLoader knifeLoader = FindObjectOfType<KnifeLoader>(); // Assuming KnifeLoader is attached to a GameObject in the scene
                if (knifeLoader != null)
                {
                    knifeLoader.ToggleKnife(0);
                }
                else
                {
                    Debug.LogWarning("KnifeLoader script not found.");
                }
                break;
            case 1: // Example: Equip Knife
                Debug.Log("Equipping Knife");
                KnifeLoader knifeLoader1 = FindObjectOfType<KnifeLoader>(); // Assuming KnifeLoader is attached to a GameObject in the scene
                if (knifeLoader1 != null)
                {
                    knifeLoader1.ToggleKnife(1);
                }
                else
                {
                    Debug.LogWarning("KnifeLoader script not found.");
                }
                break;
            case 2: // Example: Equip Knife
                Debug.Log("Equipping Knife");
                KnifeLoader knifeLoader2 = FindObjectOfType<KnifeLoader>(); // Assuming KnifeLoader is attached to a GameObject in the scene
                if (knifeLoader2 != null)
                {
                    knifeLoader2.ToggleKnife(2);
                }
                else
                {
                    Debug.LogWarning("KnifeLoader script not found.");
                }
                break;
            // Add more cases for other items
            default:
                Debug.LogWarning("Unknown item ID: " + itemId);
                break;
        }
    }
    private void SaveInventory()
    {
        List<ItemData> itemDataList = new List<ItemData>();
        foreach (var item in Items)
        {
            itemDataList.Add(new ItemData { ID = item.ID });
        }

        string json = JsonUtility.ToJson(new ItemListWrapper { Items = itemDataList });
        File.WriteAllText(Application.persistentDataPath + "/inventory.json", json);
    }

    private void LoadInventory()
    {
        string path = Application.persistentDataPath + "/inventory.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ItemListWrapper wrapper = JsonUtility.FromJson<ItemListWrapper>(json);

            Items.Clear();
            foreach (var itemData in wrapper.Items)
            {
                Item item = itemDatabase.GetItemByID(itemData.ID);
                if (item != null)
                {
                    Items.Add(item);
                }
            }
        }
    }

    [System.Serializable]
    private class ItemListWrapper
    {
        public List<ItemData> Items;
    }
}
