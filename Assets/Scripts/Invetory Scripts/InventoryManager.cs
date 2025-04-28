using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEditorInternal.Profiling.Memory.Experimental;
using static UnityEditor.Progress;

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
    public GameObject inventoryScreen;
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
    public void InstantiateBattleInventoryItems()
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
            button.onClick.AddListener(() => OnInventoryItemClick2(itemId));
            var itemName = inventoryItem.transform.Find("ItemName").GetComponent<Text>();
            var itemIcon = inventoryItem.transform.Find("ItemIcon").GetComponent<Image>();

            itemName.text = item.Name;
            itemIcon.sprite = item.Icon;
            // Customize the inventory item UI element (e.g., set text and icon)
            // ...
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void UnequipServerRpc(ulong clientId)
    {
        Unequip(PlayerManager.GetPlayerNumberByClientId(clientId));
    }
    public void Unequip(int playerNumber)
    {
        GameObject playerObject = GameObject.Find($"Player{playerNumber}");
        KnifeLoader knifeLoader = playerObject.GetComponent<KnifeLoader>();
        FoodScript foodscript = playerObject.GetComponent<FoodScript>();
        foodscript.UnequipFood();
        knifeLoader.UnequipKnife();
    }
    private void SetPlayerEquippedValue(int playerNumber, ulong clientId)
    {
        NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        if (playerObject != null)
        {
            Player player = playerObject.GetComponent<Player>();
            if (player != null && player.itemEquipped.Value == playerNumber)
            {
                player.itemEquipped.Value = playerNumber;
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void UnequipItemServerRpc(int itemId, ulong clientId)
    {

        SwitchItemsServerRpc(-1, clientId, false);
        // Remove the item from inventory
        Item itemToRemove = Items.Find(item => item.ID == itemId);
        if (itemToRemove != null)
        {
            Items.Remove(itemToRemove);
            SaveInventory();
            Debug.Log($"Removed and unequipped item {itemId} for player {clientId}");
        }
        else
        {
            Debug.LogWarning($"Item with ID {itemId} not found in inventory.");
        }
    }
    [ClientRpc]
    public void SetItemClientRpc(int itemId, int playerNumber)
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
        inventoryScreen.SetActive(false);


    }
    [ClientRpc]
    public void SetFoodClientRpc(int itemId, int playerNumber)
    {
        GameObject playerObject = GameObject.Find($"Player{playerNumber}");
        FoodScript foodscript = playerObject.GetComponent<FoodScript>();
        switch (itemId)
        {
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
        inventoryScreen.SetActive(false);


    }


    public void UseItemInBattle(int itemId, int playerNumber)
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
                BattleStarter.Instance.RequestFeedDingoServerRpc(NetworkManager.Singleton.LocalClientId, PlayerManager.Instance.PlayerNumber.Value, 3);
                SetFoodClientRpc(itemId, playerNumber);

                if (foodscript != null)
                {
                    foodscript.ForceFoodActive(1);
                }
                break;
            case 4:
                BattleStarter.Instance.RequestFeedDingoServerRpc(NetworkManager.Singleton.LocalClientId, PlayerManager.Instance.PlayerNumber.Value, 4);
                SetFoodClientRpc(itemId, playerNumber);

                if (foodscript != null)
                {
                    foodscript.ForceFoodActive(0);
                }
                break;
            case 5:
                BattleStarter.Instance.RequestFeedDingoServerRpc(NetworkManager.Singleton.LocalClientId, PlayerManager.Instance.PlayerNumber.Value, 5);
                SetFoodClientRpc(itemId, playerNumber);

                if (foodscript != null)
                {
                    foodscript.ForceFoodActive(2);
                }
                break;
            // Add more cases for other items
            default:
                Debug.LogWarning("Unknown item ID: " + itemId);
                break;
        }
        DayAndNight.Instance.SyncVisuals();
        inventoryScreen.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveItemServerRpc(int itemId, ulong clientId)
    {
        // Get the player's network object
        NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        if (playerObject == null)
        {
            Debug.LogWarning($"Player with clientId {clientId} not found.");
            return;
        }

        Player player = playerObject.GetComponent<Player>();
        if (player == null)
        {
            Debug.LogWarning("Player component not found.");
            return;
        }

        player.itemEquipped.Value = -1;


        NotifyItemWasUsedClientRpc(itemId, clientId);
    }
    [ClientRpc]
    public void NotifyItemWasUsedClientRpc(int itemId, ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;
        // Remove from global inventory (can adapt per-player inventories later if needed)
        Item itemToRemove = Items.Find(item => item.ID == itemId);
        if (itemToRemove != null)
        {
            Items.Remove(itemToRemove);
            SaveInventory();
            Debug.Log($"Removed item {itemId} from player {clientId}'s inventory.");
        }
        else
        {
            Debug.LogWarning($"Item with ID {itemId} not found in inventory.");
        }
    }
    private void OnInventoryItemClick(int itemId)
    {
        SwitchItemsServerRpc(itemId, NetworkManager.Singleton.LocalClientId, false);

    }
    private void OnInventoryItemClick2(int itemId)
    {
        Player PlayerNumber = GetPlayerComponent(NetworkManager.Singleton.LocalClientId);
        UseItemInBattle(itemId, PlayerNumber.playerNumber);
        SwitchItemsServerRpc(itemId, NetworkManager.Singleton.LocalClientId, true);

    }
    private Player GetPlayerComponent(ulong clientId)
    {
        NetworkObject localPlayerObject = NetworkManager.Singleton.SpawnManager
    .GetPlayerNetworkObject(clientId);

        // Null check for safety
        if (localPlayerObject != null)
        {
            // Get the Player component
            Player localPlayer = localPlayerObject.GetComponent<Player>();

            if (localPlayer != null)
            {
                return localPlayer;

            }
            else
            {
                Debug.LogError("Player component not found on local player object");
                return null;
            }
        }
        else
        {
            Debug.LogError("Local player NetworkObject not found");
            return null;

        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SwitchItemsServerRpc(int itemId, ulong clientId, bool bypassUnequip)
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
                    if (!bypassUnequip)
                    {
                        localPlayer.itemEquipped.Value = -1;

                    }

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
