using DingoSystem;
using SimpleJSON;
using System.IO;
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
    public static DingoID LoadPlayerDingoFromFileToRecieve(string filePath, int slot)
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


    private static GameObject LoadPlayerDingoFromJsonObject(JSONObject dingoData)
    {
        DingoID playerDingo = new DingoID(
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

        DingoID playerDingoMoves = DingoDatabase.GetDingoByID(dingoData["DingoID"]);
        DingoMove[] moves = new DingoMove[] {
            DingoDatabase.GetMoveByID(dingoData["Move1ID"], playerDingoMoves),
            DingoDatabase.GetMoveByID(dingoData["Move2ID"], playerDingoMoves),
            DingoDatabase.GetMoveByID(dingoData["Move3ID"], playerDingoMoves),
            DingoDatabase.GetMoveByID(dingoData["Move4ID"], playerDingoMoves)
        };

        // Create the Dingo object in the game world
        GameObject playerDingoObject = new GameObject(playerDingo.Name); // Example name; replace with actual logic to create the Dingo.
        // Set the Dingo’s stats, sprite, etc.

        return playerDingoObject;
    }
}
