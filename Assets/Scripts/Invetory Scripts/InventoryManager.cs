using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEditorInternal.Profiling.Memory.Experimental;

[System.Serializable]
public class ItemData
{
    public int ID;
}

public class InventoryManager : NetworkBehaviour
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
    public void SetItem(int itemId, int playerNumber)
    {
        GameObject playerObject = GameObject.Find($"Player{playerNumber}");
        KnifeLoader knifeLoader = playerObject.GetComponent<KnifeLoader>();
        FoodScript foodscript = playerObject.GetComponent<FoodScript>();
        foodscript.UnequipFood();
        knifeLoader.UnequipKnife();
        switch (itemId)
        {
            case -1:
                Debug.Log("Unequipping");
                break;
            case 0: // Example: Equip Knife
                Debug.Log("Equipping Knife");
                if (knifeLoader != null)
                {
                    knifeLoader.EquipKnife(0);
                }
                else
                {
                    Debug.LogWarning("KnifeLoader script not found.");
                }
                break;
            case 1: // Example: Equip Knife
                Debug.Log("Equipping Knife");
                if (knifeLoader != null)
                {
                    knifeLoader.EquipKnife(1);
                }
                else
                {
                    Debug.LogWarning("KnifeLoader script not found.");
                }
                break;
            case 2: // Example: Equip Knife
                Debug.Log("Equipping Knife");
                if (knifeLoader != null)
                {
                    knifeLoader.EquipKnife(2);
                }
                else
                {
                    Debug.LogWarning("KnifeLoader script not found.");
                }
                break;
            case 3:
                if (foodscript != null)
                {
                    foodscript.EquipFood(1);
                }
                break;
            case 4:
                if (foodscript != null)
                {
                    foodscript.EquipFood(0);
                }
                break;
            case 5:
                if (foodscript != null)
                {
                    foodscript.EquipFood(2);
                }
                break;
            // Add more cases for other items
            default:
                Debug.LogWarning("Unknown item ID: " + itemId);
                break;
        }
        DayAndNight.Instance.SyncVisuals();

    }
    // Method to handle item click
    private void OnInventoryItemClick(int itemId)
    {
        SwitchItemsServerRpc(itemId, NetworkManager.Singleton.LocalClientId);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SwitchItemsServerRpc(int itemId, ulong clientId)
    {
        // Get the local player's NetworkObject
        NetworkObject localPlayerObject = NetworkManager.Singleton.SpawnManager
            .GetPlayerNetworkObject(clientId);

        // Null check for safety
        if (localPlayerObject != null)
        {
            // Get the Player component
            Player localPlayer = localPlayerObject.GetComponent<Player>();

            if (localPlayer != null)
            {
                if (itemId == localPlayer.itemEquipped.Value)
                {
                    localPlayer.itemEquipped.Value = -1;
                }
                else
                {
                    localPlayer.itemEquipped.Value = itemId;
                }
            }
            else
            {
                Debug.LogError("Player component not found on local player object");
            }
        }
        else
        {
            Debug.LogError("Local player NetworkObject not found");
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InspectServerRpc(int itemId, ulong clientId)
    {
        // Get the local player's NetworkObject
        NetworkObject localPlayerObject = NetworkManager.Singleton.SpawnManager
            .GetPlayerNetworkObject(clientId);

        // Null check for safety
        if (localPlayerObject != null)
        {
            // Get the Player component
            Player localPlayer = localPlayerObject.GetComponent<Player>();

            if (localPlayer != null)
            {
                if (itemId == localPlayer.itemEquipped.Value)
                {
                    localPlayer.itemEquipped.Value = -1;
                }
                else
                {
                    localPlayer.itemEquipped.Value = itemId;
                }
            }
            else
            {
                Debug.LogError("Player component not found on local player object");
            }
        }
        else
        {
            Debug.LogError("Local player NetworkObject not found");
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
