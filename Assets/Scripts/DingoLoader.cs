using DingoSystem;
using SimpleJSON;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public static class DingoLoader
{
    public static GameObject battlePrefab;
    public static GameObject dingoPrefab;
    

    // Start function to load battlePrefab from Resources
    public static void Start()
    {
        battlePrefab = Resources.Load<GameObject>("Prefabs/BattlePrefab");
        if (battlePrefab != null)
        {
            Debug.Log("Battle Prefab loaded successfully.");
        }
        else
        {
            Debug.LogError("Failed to load Battle Prefab.");
        }
        dingoPrefab = Resources.Load<GameObject>("Prefabs/DingoPrefab");
    }
    // Updated LoadPlayerDingoFromFile to return DingoID and not a GameObject
    public static DingoID LoadPlayerDingoFromFile(int slot)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "dingos.json");

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            JSONArray jsonDingos = JSON.Parse(jsonData) as JSONArray;

            if (jsonDingos != null && jsonDingos.Count > slot)
            {
                JSONObject dingoData = jsonDingos[slot].AsObject;

                // Return the DingoID (data)
                return new DingoID(
                    dingoData["DingoID"],
                    dingoData["Name"],
                    dingoData["Type"],
                    dingoData["Description"],
                    dingoData["CurrentHealth"],
                    dingoData["ATK"],
                    dingoData["DEF"],
                    dingoData["SPD"],
                    dingoData["Sprite"],
                    dingoData["MaxHealth"],
                    dingoData["XP"],
                    dingoData["MaxXP"],
                    dingoData["Level"]);
            }
        }

        return null; // Return null if not found
    }
    // In DingoLoader.cs
    public static int GetPlayerDingoCount(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            Debug.LogError("Invalid file path or file does not exist.");
            return 0;
        }

        string jsonData = File.ReadAllText(filePath);
        JSONArray jsonDingos = JSON.Parse(jsonData) as JSONArray;
        return jsonDingos != null ? jsonDingos.Count : 0;
    }
    public static NetworkDingo LoadRandomDingoFromList(List<DingoID> dingoList)
    {
        if (dingoList == null || dingoList.Count == 0)
        {
            Debug.LogError("Dingo list is empty or null!");
            return null;
        }

        // Pick a random Dingo from the list
        DingoID randomDingo = dingoList[UnityEngine.Random.Range(0, dingoList.Count)];

        // Load the DingoPrefab
        GameObject dingoPrefab = Resources.Load<GameObject>("Prefabs/DingoPrefab");

        if (dingoPrefab == null)
        {
            Debug.LogError("DingoPrefab not found in Resources/Prefabs!");
            return null;
        }

        // Instantiate the prefab
        GameObject dingoInstance = GameObject.Instantiate(dingoPrefab);
        NetworkDingo networkDingo = dingoInstance.GetComponent<NetworkDingo>();

        if (networkDingo == null)
        {
            Debug.LogError("NetworkDingo script not found on instantiated prefab!");
            return null;
        }

        // Assign values to NetworkVariables using DingoID properties
        networkDingo.id.Value = randomDingo.ID; // DingoID
        networkDingo.name.Value = new FixedString64Bytes(randomDingo.Name); // Name
        networkDingo.type.Value = new FixedString64Bytes(randomDingo.Type); // Type
        networkDingo.spritePath.Value = new FixedString128Bytes(randomDingo.Sprite); // Sprite

        networkDingo.hp.Value = randomDingo.HP; // HP
        networkDingo.maxHP.Value = randomDingo.MaxHP; // MaxHP
        networkDingo.attack.Value = randomDingo.Attack; // Attack
        networkDingo.defense.Value = randomDingo.Defense; // Defense
        networkDingo.speed.Value = randomDingo.Speed; // Speed
        networkDingo.xp.Value = randomDingo.XP; // XP
        networkDingo.maxXP.Value = randomDingo.MaxXP; // MaxXP
        networkDingo.level.Value = randomDingo.Level; // Level

        // Assign move IDs to NetworkVariables (using randomDingo's Moves)
        if (randomDingo.Moves.Count >= 1)
            networkDingo.move1.Value = randomDingo.Moves[0].MoveID; // Move 1
        if (randomDingo.Moves.Count >= 2)
            networkDingo.move2.Value = randomDingo.Moves[1].MoveID; // Move 2
        if (randomDingo.Moves.Count >= 3)
            networkDingo.move3.Value = randomDingo.Moves[2].MoveID; // Move 3
        if (randomDingo.Moves.Count >= 4)
            networkDingo.move4.Value = randomDingo.Moves[3].MoveID; // Move 4

        Debug.Log($"Instantiated Random Dingo: {randomDingo.Name} (ID {randomDingo.ID})");

        return networkDingo;
    }

    private static NetworkDingo TryLoadFromSlot(string filePath, int slot)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("Dingos.json not found!");
            return null;
        }

        // Read JSON file
        string jsonData = File.ReadAllText(filePath);
        JSONArray jsonDingos = JSON.Parse(jsonData) as JSONArray;

        // Check if slot is valid and has data
        if (jsonDingos == null || jsonDingos.Count <= slot)
        {
            return null;
        }

        JSONObject dingoData = jsonDingos[slot].AsObject;

        // Check if this slot has valid Dingo data
        if (dingoData == null || !dingoData.HasKey("DingoID"))
        {
            return null;
        }

        // Load the DingoPrefab
        GameObject dingoPrefab = Resources.Load<GameObject>("Prefabs/DingoPrefab");
        if (dingoPrefab == null)
        {
            Debug.LogError("DingoPrefab not found in Resources/Prefabs!");
            return null;
        }

        // Instantiate the prefab
        GameObject dingoInstance = GameObject.Instantiate(dingoPrefab);
        NetworkDingo networkDingo = dingoInstance.GetComponent<NetworkDingo>();
        if (networkDingo == null)
        {
            Debug.LogError("NetworkDingo script not found on instantiated prefab!");
            return null;
        }

        // Assign values to NetworkVariables
        networkDingo.id.Value = dingoData["DingoID"];
        networkDingo.name.Value = new FixedString64Bytes(dingoData["Name"].Value);
        networkDingo.type.Value = new FixedString64Bytes(dingoData["Type"].Value);
        networkDingo.spritePath.Value = new FixedString128Bytes(dingoData["Sprite"].Value);

        networkDingo.hp.Value = dingoData["CurrentHealth"];
        networkDingo.maxHP.Value = dingoData["MaxHealth"];
        networkDingo.attack.Value = dingoData["ATK"];
        networkDingo.defense.Value = dingoData["DEF"];
        networkDingo.speed.Value = dingoData["SPD"];
        networkDingo.xp.Value = dingoData["XP"];
        networkDingo.maxXP.Value = dingoData["MaxXP"];
        networkDingo.level.Value = dingoData["Level"];

        // Assign move IDs correctly from JSON
        networkDingo.move1.Value = dingoData.HasKey("Move1ID") ? dingoData["Move1ID"] : 0;
        networkDingo.move2.Value = dingoData.HasKey("Move2ID") ? dingoData["Move2ID"] : 0;
        networkDingo.move3.Value = dingoData.HasKey("Move3ID") ? dingoData["Move3ID"] : 0;
        networkDingo.move4.Value = dingoData.HasKey("Move4ID") ? dingoData["Move4ID"] : 0;

        Debug.Log($"Instantiated Dingo: {dingoData["Name"]} (ID {dingoData["DingoID"]}) from slot {slot}.");

        return networkDingo;
    }
    public static NetworkDingo LoadPrefabWithStats(int slot)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "dingos.json");

        // First try to load from the specified slot
        NetworkDingo loadedDingo = TryLoadFromSlot(filePath, slot);

        // If slot was empty or invalid, get a random Dingo from database
        if (loadedDingo == null)
        {
            loadedDingo = LoadRandomDingoFromList(DingoDatabase.allDingos);


        }

        return loadedDingo;
    }

    public static int[] LoadDingoMovesToSend(int slot)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "dingos.json");

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            JSONArray jsonDingos = JSON.Parse(jsonData) as JSONArray;

            if (jsonDingos != null && jsonDingos.Count > slot)
            {
                JSONObject dingoData = jsonDingos[slot].AsObject;

                return new int[]
                {
dingoData["Move1ID"],
dingoData["Move2ID"],
dingoData["Move3ID"],
dingoData["Move4ID"]
                };
            }
        }

        Debug.LogError("Failed to load moves: File not found or slot invalid.");
        return null;
    }
    public static DingoMove[] LoadDingoMoves(int slot)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "dingos.json");

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            JSONArray jsonDingos = JSON.Parse(jsonData) as JSONArray;

            if (jsonDingos != null && jsonDingos.Count > slot)
            {
                JSONObject dingoData = jsonDingos[slot].AsObject;
                DingoID dingo = DingoDatabase.GetDingoByID(dingoData["DingoID"]);

                return new DingoMove[]
                {
                DingoDatabase.GetMoveByID(dingoData["Move1ID"], dingo),
                DingoDatabase.GetMoveByID(dingoData["Move2ID"], dingo),
                DingoDatabase.GetMoveByID(dingoData["Move3ID"], dingo),
                DingoDatabase.GetMoveByID(dingoData["Move4ID"], dingo)
                };
            }
        }

        Debug.LogError("Failed to load moves: File not found or slot invalid.");
        return null;
    }
    public static DingoMove[] LoadDingoMovesByID(int dingoID)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "dingos.json");

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            JSONArray jsonDingos = JSON.Parse(jsonData) as JSONArray;

            if (jsonDingos != null)
            {
                foreach (JSONObject dingoData in jsonDingos)
                {
                    if (dingoData["DingoID"] == dingoID)
                    {
                        DingoID dingo = DingoDatabase.GetDingoByID(dingoID);
                        return new DingoMove[]
                        {
                        DingoDatabase.GetMoveByID(dingoData["Move1ID"], dingo),
                        DingoDatabase.GetMoveByID(dingoData["Move2ID"], dingo),
                        DingoDatabase.GetMoveByID(dingoData["Move3ID"], dingo),
                        DingoDatabase.GetMoveByID(dingoData["Move4ID"], dingo)
                        };
                    }
                }
            }
        }

        Debug.LogError("Failed to load moves: DingoID not found.");
        return null;
    }

    public static DingoMove[] LoadDingoMovesInput(int move1, int move2, int move3, int move4, int dingoID)
    {
        DingoID dingo = DingoDatabase.GetDingoByID(dingoID);
        return new DingoMove[]
        {
                        DingoDatabase.GetMoveByID(move1, dingo),
                        DingoDatabase.GetMoveByID(move2, dingo),
                        DingoDatabase.GetMoveByID(move3, dingo),
                        DingoDatabase.GetMoveByID(move4, dingo)
        };
    }

    public static string LoadPlayerDingoFromFileToSend()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "dingos.json");

        return filePath; // Return null if not found
    }
    public static DingoID LoadPlayerDingoFromFileToReceive(string filePath, int slot)
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            JSONArray jsonDingos = JSON.Parse(jsonData) as JSONArray;

            if (jsonDingos != null && jsonDingos.Count > slot)
            {
                JSONObject dingoData = jsonDingos[slot].AsObject;

                // Return the DingoID (data)
                return new DingoID(
                    dingoData["DingoID"],
                    dingoData["Name"],
                    dingoData["Type"],
                    dingoData["Description"],
                    dingoData["CurrentHealth"],
                    dingoData["ATK"],
                    dingoData["DEF"],
                    dingoData["SPD"],
                    dingoData["Sprite"],
                    dingoData["MaxHealth"],
                    dingoData["XP"],
                    dingoData["MaxXP"],
                    dingoData["Level"]);

        };
        }

        return null; // Return null if not found
    }
    public static NetworkDingo LoadNetworkDingoFromFileToReceive(string filePath, int slot)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("Dingos.json not found!");
            return null;
        }

        // Read JSON file
        string jsonData = File.ReadAllText(filePath);
        JSONArray jsonDingos = JSON.Parse(jsonData) as JSONArray;

        if (jsonDingos == null || jsonDingos.Count <= slot)
        {
            Debug.LogError($"No Dingo found in slot {slot}. Cannot instantiate prefab.");
            return null;
        }

        JSONObject dingoData = jsonDingos[slot].AsObject;

        // Load the DingoPrefab
        GameObject dingoPrefab = Resources.Load<GameObject>("Prefabs/DingoPrefab");

        if (dingoPrefab == null)
        {
            Debug.LogError("DingoPrefab not found in Resources/Prefabs!");
            return null;
        }

        // Instantiate the prefab
        GameObject dingoInstance = GameObject.Instantiate(dingoPrefab);
        NetworkDingo networkDingo = dingoInstance.GetComponent<NetworkDingo>();

        if (networkDingo == null)
        {
            Debug.LogError("NetworkDingo script not found on instantiated prefab!");
            return null;
        }

        // Assign values to NetworkVariables
        networkDingo.id.Value = dingoData["DingoID"];
        networkDingo.name.Value = new FixedString64Bytes(dingoData["Name"].Value);
        networkDingo.type.Value = new FixedString64Bytes(dingoData["Type"].Value);
        networkDingo.spritePath.Value = new FixedString128Bytes(dingoData["Sprite"].Value);

        networkDingo.hp.Value = dingoData["CurrentHealth"];
        networkDingo.maxHP.Value = dingoData["MaxHealth"];
        networkDingo.attack.Value = dingoData["ATK"];
        networkDingo.defense.Value = dingoData["DEF"];
        networkDingo.speed.Value = dingoData["SPD"];
        networkDingo.xp.Value = dingoData["XP"];
        networkDingo.maxXP.Value = dingoData["MaxXP"];
        networkDingo.level.Value = dingoData["Level"];

        // Assign move IDs correctly from JSON
        networkDingo.move1.Value = dingoData.HasKey("Move1ID") ? dingoData["Move1ID"] : 0;
        networkDingo.move2.Value = dingoData.HasKey("Move2ID") ? dingoData["Move2ID"] : 0;
        networkDingo.move3.Value = dingoData.HasKey("Move3ID") ? dingoData["Move3ID"] : 0;
        networkDingo.move4.Value = dingoData.HasKey("Move4ID") ? dingoData["Move4ID"] : 0;

        Debug.Log($"Instantiated Dingo: {dingoData["Name"]} (ID {dingoData["DingoID"]}) from slot {slot}.");

        return networkDingo;
    }
}
