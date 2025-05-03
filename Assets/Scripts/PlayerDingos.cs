using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DingoSystem;
using SimpleJSON;
using System.IO;
using System;
using System.Net.NetworkInformation;
using Unity.VisualScripting;

public class PlayerDingos : MonoBehaviour
{
    public static PlayerDingos Instance;
    public Transform PlayerDingoContent;
    public List<DingoID> DingoList = new List<DingoID>(); // Assuming DingoID is defined elsewhere.
    public GameObject DingoItem;
    public GameObject StatScreenPrefab; // Reference to the stat screen prefab
    public GameObject Bingo;
    public SpriteRenderer Dingo;
    public string InventoryID;
    public Text Success;
    private JSONArray jsonDingos;
    public int pagenumber;
#pragma warning disable CS0649 // Field 'PlayerDingos.filePath' is never assigned to, and will always have its default value null
    private string filePath;
#pragma warning restore CS0649 // Field 'PlayerDingos.filePath' is never assigned to, and will always have its default value null
    private string jsonData;
    public HealthBar healthBar;
    public HealthBar xpBar;
    public Text SetMoves;
    public Text Warning;
    public Text IDstat;
    public Text NAMEstat;
    public Text ATKstat;
    public Text DEFstat;
    public Text SPDstat;
    public Text LVLstat;
    public Text TYPEstat;
    public Text DESCstat;
    public Text HPstat;
    public Text XPstat;
    private int attack;
    private int defense;
    private int speed;
    public Image SPRITEstat;
    public Text ActiveMove1;
    public Text ActiveMove2;
    public Text ActiveMove3;
    public Text ActiveMove4;
    public bool move1toggle = false;

    public Text[] MoveNames;
    public Text[] MovePowers;
    public Text[] MoveAccuracies;
    public Text[] MoveDescriptions;
    public Text[] MoveTypes;
    public bool agentBingo = false;
    private void Awake()
    {
        Instance = this;
        DingoLoader.Start();
    }
    public void DeleteJson()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "dingos.json");

        // Check if the file exists before attempting to delete it
        if (File.Exists(filePath))
        {
            try
            {
                // Delete the file
                File.Delete(filePath);
                Debug.Log("Deleted dingos.json file.");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error deleting dingos.json file: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("dingos.json file not found at path: " + filePath);
        }
    }
    public void ListDingos()
    {
        // Clear existing Dingo items before populating the list
        foreach (Transform child in PlayerDingoContent)
        {
            Destroy(child.gameObject);
        }
        if (File.Exists(filePath))
        {
            try
            {

                if (jsonDingos != null)
                {
                    foreach (JSONNode dingoData in jsonDingos)
                    {
                        JSONObject dingo = dingoData.AsObject;
                        GameObject obj = Instantiate(DingoItem, PlayerDingoContent);
                        var dingoName = obj.transform.Find("ItemName").GetComponent<Text>();
                        var dingoIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
                        var dingoType = obj.transform.Find("ItemType").GetComponent<Text>();
                        var dingoID = obj.transform.Find("ItemID").GetComponent<Text>();

                        dingoName.text = dingo["Name"];
                        dingoType.text = dingo["Type"];
                        dingoID.text = dingo["ID"];
                        // Load Dingo icon sprite from Resources folder based on the icon path in JSON data
                        string iconPath = dingo["Sprite"];
                        Sprite iconSprite = Resources.Load<Sprite>(iconPath);
                        if (iconSprite != null)
                        {
                            dingoIcon.sprite = iconSprite;
                        }
                        else
                        {
                            Debug.LogWarning("Failed to load Dingo icon: " + iconPath);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to parse JSON data.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error reading JSON file: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Dingo JSON file not found at path: " + filePath);
        }
    }
    public void Heal()
    {
        filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
        if (File.Exists(filePath))
        {
            jsonData = File.ReadAllText(filePath);
            jsonDingos = JSON.Parse(jsonData) as JSONArray;
            if (jsonDingos != null && jsonDingos.Count > 0)
            {
                JSONObject dingo = jsonDingos[pagenumber].AsObject;
                attack = dingo["ATK"];
                defense = dingo["DEF"];
                speed = dingo["SPD"];
                attack++;
                while (jsonDingos.Count <= pagenumber)
                {
                    jsonDingos.Add(new JSONObject());
                }

                JSONObject jsonDingo = new JSONObject();
                jsonDingo.Add("ID", pagenumber);
                jsonDingo.Add("DingoID", dingo["DingoID"]);
                jsonDingo.Add("Name", dingo["Name"]);
                jsonDingo.Add("Type", dingo["Type"]);
                jsonDingo.Add("Description", dingo["Description"]);
                jsonDingo.Add("CurrentHealth", dingo["MaxHealth"]);
                jsonDingo.Add("ATK", attack);
                jsonDingo.Add("DEF", defense);
                jsonDingo.Add("SPD", speed);
                jsonDingo.Add("Sprite", dingo["Sprite"]);
                jsonDingo.Add("MaxHealth", dingo["MaxHealth"]);
                jsonDingo.Add("XP", dingo["XP"]);
                jsonDingo.Add("MaxXP", dingo["MaxXP"]);
                jsonDingo.Add("Level", dingo["Level"]);
                jsonDingo.Add("Move1ID", dingo["Move1ID"]);
                jsonDingo.Add("Move2ID", dingo["Move2ID"]);
                jsonDingo.Add("Move3ID", dingo["Move3ID"]);
                jsonDingo.Add("Move4ID", dingo["Move4ID"]);
                jsonDingos[pagenumber] = jsonDingo;
                HPstat.text = "HP " + dingo["MaxHealth"] + "/" + dingo["MaxHealth"];
                healthBar.SetMaxHealth(dingo["MaxHealth"]);
                healthBar.SetHealth(dingo["MaxHealth"]);
                // Save the updated data back to the file
                File.WriteAllText(filePath, jsonDingos.ToString());
            }
        }

    }
    public void AddHealth()
    {
        filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
        if (File.Exists(filePath))
        {
            jsonData = File.ReadAllText(filePath);
            jsonDingos = JSON.Parse(jsonData) as JSONArray;
            if (jsonDingos != null && jsonDingos.Count > 0)
            {
                JSONObject dingo = jsonDingos[pagenumber].AsObject;
                attack = dingo["ATK"];
                defense = dingo["DEF"];
                speed = dingo["SPD"];
                int currentHp = dingo["CurrentHealth"];
                int maxHp = dingo["MaxHealth"];
                currentHp = currentHp + 100;
                maxHp = maxHp + 100;
                while (jsonDingos.Count <= pagenumber)
                {
                    jsonDingos.Add(new JSONObject());
                }

                JSONObject jsonDingo = new JSONObject();
                jsonDingo.Add("ID", pagenumber);
                jsonDingo.Add("DingoID", dingo["DingoID"]);
                jsonDingo.Add("Name", dingo["Name"]);
                jsonDingo.Add("Type", dingo["Type"]);
                jsonDingo.Add("Description", dingo["Description"]);
                jsonDingo.Add("CurrentHealth", currentHp);
                jsonDingo.Add("ATK", attack);
                jsonDingo.Add("DEF", defense);
                jsonDingo.Add("SPD", speed);
                jsonDingo.Add("Sprite", dingo["Sprite"]);
                jsonDingo.Add("MaxHealth", maxHp);
                jsonDingo.Add("XP", dingo["XP"]);
                jsonDingo.Add("MaxXP", dingo["MaxXP"]);
                jsonDingo.Add("Level", dingo["Level"]);
                jsonDingo.Add("Move1ID", dingo["Move1ID"]);
                jsonDingo.Add("Move2ID", dingo["Move2ID"]);
                jsonDingo.Add("Move3ID", dingo["Move3ID"]);
                jsonDingo.Add("Move4ID", dingo["Move4ID"]);
                jsonDingos[pagenumber] = jsonDingo;
                HPstat.text = "HP " + currentHp + "/" + maxHp;
                healthBar.SetMaxHealth(maxHp);
                healthBar.SetHealth(currentHp);
                // Save the updated data back to the file
                File.WriteAllText(filePath, jsonDingos.ToString());
            }
        }

    }

    public void AddAttack()
    {
        filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
        if (File.Exists(filePath))
        {
            jsonData = File.ReadAllText(filePath);
            jsonDingos = JSON.Parse(jsonData) as JSONArray;
            if (jsonDingos != null && jsonDingos.Count > 0)
            {
                JSONObject dingo = jsonDingos[pagenumber].AsObject;
                attack = dingo["ATK"];
                defense = dingo["DEF"];
                speed = dingo["SPD"];
                attack++;
                while (jsonDingos.Count <= pagenumber)
                {
                    jsonDingos.Add(new JSONObject());
                }

                JSONObject jsonDingo = new JSONObject();
                jsonDingo.Add("ID", pagenumber);
                jsonDingo.Add("DingoID", dingo["DingoID"]);
                jsonDingo.Add("Name", dingo["Name"]);
                jsonDingo.Add("Type", dingo["Type"]);
                jsonDingo.Add("Description", dingo["Description"]);
                jsonDingo.Add("CurrentHealth", dingo["CurrentHealth"]);
                jsonDingo.Add("ATK", attack);
                jsonDingo.Add("DEF", defense);
                jsonDingo.Add("SPD", speed);
                jsonDingo.Add("Sprite", dingo["Sprite"]);
                jsonDingo.Add("MaxHealth", dingo["MaxHealth"]);
                jsonDingo.Add("XP", dingo["XP"]);
                jsonDingo.Add("MaxXP", dingo["MaxXP"]);
                jsonDingo.Add("Level", dingo["Level"]);
                jsonDingo.Add("Move1ID", dingo["Move1ID"]);
                jsonDingo.Add("Move2ID", dingo["Move2ID"]);
                jsonDingo.Add("Move3ID", dingo["Move3ID"]);
                jsonDingo.Add("Move4ID", dingo["Move4ID"]);
                jsonDingos[pagenumber] = jsonDingo;

                // Save the updated data back to the file
                File.WriteAllText(filePath, jsonDingos.ToString());
            }
        }
        ATKstat.text = "Attack " + attack;
    }
    public void AddDefense()
    {
        filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
        if (File.Exists(filePath))
        {
            jsonData = File.ReadAllText(filePath);
            jsonDingos = JSON.Parse(jsonData) as JSONArray;
            if (jsonDingos != null && jsonDingos.Count > 0)
            {
                JSONObject dingo = jsonDingos[pagenumber].AsObject;
                attack = dingo["ATK"];
                defense = dingo["DEF"];
                speed = dingo["SPD"];
                defense++;
                while (jsonDingos.Count <= pagenumber)
                {
                    jsonDingos.Add(new JSONObject());
                }

                JSONObject jsonDingo = new JSONObject();
                jsonDingo.Add("ID", pagenumber);
                jsonDingo.Add("DingoID", dingo["DingoID"]);
                jsonDingo.Add("Name", dingo["Name"]);
                jsonDingo.Add("Type", dingo["Type"]);
                jsonDingo.Add("Description", dingo["Description"]);
                jsonDingo.Add("CurrentHealth", dingo["CurrentHealth"]);
                jsonDingo.Add("ATK", attack);
                jsonDingo.Add("DEF", defense);
                jsonDingo.Add("SPD", speed);
                jsonDingo.Add("Sprite", dingo["Sprite"]);
                jsonDingo.Add("MaxHealth", dingo["MaxHealth"]);
                jsonDingo.Add("XP", dingo["XP"]);
                jsonDingo.Add("MaxXP", dingo["MaxXP"]);
                jsonDingo.Add("Level", dingo["Level"]);
                jsonDingo.Add("Move1ID", dingo["Move1ID"]);
                jsonDingo.Add("Move2ID", dingo["Move2ID"]);
                jsonDingo.Add("Move3ID", dingo["Move3ID"]);
                jsonDingo.Add("Move4ID", dingo["Move4ID"]);
                jsonDingos[pagenumber] = jsonDingo;

                // Save the updated data back to the file
                File.WriteAllText(filePath, jsonDingos.ToString());
            }
        }
        DEFstat.text = "Defense " + defense;
    }
    public void AddSpeed()
    {
        filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
        if (File.Exists(filePath))
        {
            jsonData = File.ReadAllText(filePath);
            jsonDingos = JSON.Parse(jsonData) as JSONArray;
            if (jsonDingos != null && jsonDingos.Count > 0)
            {
                JSONObject dingo = jsonDingos[pagenumber].AsObject;
                attack = dingo["ATK"];
                defense = dingo["DEF"];
                speed = dingo["SPD"];
                speed++;
                while (jsonDingos.Count <= pagenumber)
                {
                    jsonDingos.Add(new JSONObject());
                }

                JSONObject jsonDingo = new JSONObject();
                jsonDingo.Add("ID", pagenumber);
                jsonDingo.Add("DingoID", dingo["DingoID"]);
                jsonDingo.Add("Name", dingo["Name"]);
                jsonDingo.Add("Type", dingo["Type"]);
                jsonDingo.Add("Description", dingo["Description"]);
                jsonDingo.Add("CurrentHealth", dingo["CurrentHealth"]);
                jsonDingo.Add("ATK", attack);
                jsonDingo.Add("DEF", defense);
                jsonDingo.Add("SPD", speed);
                jsonDingo.Add("Sprite", dingo["Sprite"]);
                jsonDingo.Add("MaxHealth", dingo["MaxHealth"]);
                jsonDingo.Add("XP", dingo["XP"]);
                jsonDingo.Add("MaxXP", dingo["MaxXP"]);
                jsonDingo.Add("Level", dingo["Level"]);
                jsonDingo.Add("Move1ID", dingo["Move1ID"]);
                jsonDingo.Add("Move2ID", dingo["Move2ID"]);
                jsonDingo.Add("Move3ID", dingo["Move3ID"]);
                jsonDingo.Add("Move4ID", dingo["Move4ID"]);
                jsonDingos[pagenumber] = jsonDingo;

                // Save the updated data back to the file
                File.WriteAllText(filePath, jsonDingos.ToString());
            }
        }
        SPDstat.text = "Speed " + speed;
    }
    private void UpdateAllMoves(List<DingoMove> moves)
    {
        for (int i = 0; i < moves.Count && i < 10; i++)
        {
            DingoMove move = moves[i];
            MoveNames[i].text = move.Name;
            MovePowers[i].text = move.Power.ToString();
            MoveAccuracies[i].text = move.Accuracy.ToString();
            MoveTypes[i].text = move.Type.ToString();
            MoveDescriptions[i].text = move.Description;
        }

        // If there are fewer than 10 moves, clear the remaining slots
        for (int i = moves.Count; i < 10; i++)
        {
            MoveNames[i].text = "";
            MovePowers[i].text = "";
            MoveAccuracies[i].text = "";
            MoveTypes[i].text = "";
            MoveDescriptions[i].text = "";
        }
    }
    public void ShowAgentBingoStatScreens()
    {
        string filePath;
        filePath = Path.Combine(Application.persistentDataPath, "playerinfo.json");
        jsonData = File.ReadAllText(filePath);
        jsonDingos = JSON.Parse(jsonData) as JSONArray;

        Debug.Log("File path: " + filePath);

        // Check if file exists
        if (File.Exists(filePath))
        {
            try
            {
                // Read JSON data from file
                string jsonData = File.ReadAllText(filePath);
                Debug.Log("JSON data: " + jsonData);

                // Parse JSON data
                JSONArray jsonDingos = JSON.Parse(jsonData) as JSONArray;
                if (jsonDingos != null)
                {
                    // Instantiate the stat screen prefab for the first Dingo
                    JSONNode firstDingoData = jsonDingos[0];
                    JSONObject firstDingo = firstDingoData.AsObject;
                    pagenumber = 0;

                    xpBar.SetMaxHealth(firstDingo["MaxXP"]);
                    xpBar.SetHealth(firstDingo["XP"]);
                    healthBar.SetMaxHealth(firstDingo["MaxHealth"]);
                    healthBar.SetHealth(firstDingo["CurrentHealth"]);
                    // Create a new DingoID object based on the ID from the JSON data
                    int dingoID = firstDingo["DingoID"];
                    int move1id = firstDingo["Move1ID"];
                    int move2id = firstDingo["Move2ID"];
                    int move3id = firstDingo["Move3ID"];
                    int move4id = firstDingo["Move4ID"];
                    DingoID dingo = DingoDatabase.GetDingoByID(dingoID);
                    DingoMove activemove1 = DingoDatabase.GetMoveByID(move1id, dingo);
                    DingoMove activemove2 = DingoDatabase.GetMoveByID(move2id, dingo);
                    DingoMove activemove3 = DingoDatabase.GetMoveByID(move3id, dingo);
                    DingoMove activemove4 = DingoDatabase.GetMoveByID(move4id, dingo);
                    ActiveMove1.text = activemove1.Name;
                    ActiveMove2.text = activemove2.Name;
                    ActiveMove3.text = activemove3.Name;
                    ActiveMove4.text = activemove4.Name;
                    UpdateAllMoves(dingo.Moves);

                    // You can then use the dingo object as needed
                    Debug.Log("Loaded Dingo: " + dingo.Name);



                    // Populate the stat screen with the first Dingo's information
                    DESCstat.text = firstDingo["Description"];
                    NAMEstat.text = firstDingo["Name"];
                    TYPEstat.text = firstDingo["Type"];
                    IDstat.text = firstDingo["ID"];
                    ATKstat.text = "Attack " + firstDingo["ATK"];
                    DEFstat.text = "Defense " + firstDingo["DEF"];
                    SPDstat.text = "Speed " + firstDingo["SPD"];
                    LVLstat.text = "Level " + firstDingo["Level"];
                    HPstat.text = "HP " + firstDingo["CurrentHealth"] + "/" + firstDingo["MaxHealth"];
                    XPstat.text = "XP " + firstDingo["XP"] + "/" + firstDingo["MaxXP"];
                    // Load Dingo icon sprite from Resources folder based on the icon path in JSON data
                    Sprite dingoSprite = Resources.Load<Sprite>(firstDingo["Sprite"]);
                    SPRITEstat.sprite = dingoSprite;
                    agentBingo = true;
                }
                else
                {
                    Debug.LogWarning("Failed to parse JSON data.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error reading JSON file: " + e.Message);
                // Print full stack trace if available
                Debug.LogException(e);
            }
        }
        else
        {
            if (healthBar != null)
            {
                // Call the HideHealthBar method
                healthBar.HideHealthBar();
            }
            else
            {
                Debug.LogError("HealthBar reference is null.");
            }
            Warning.enabled = true;
            Debug.LogWarning("Dingo JSON file not found at path: " + filePath);
        }
    }
    public void ShowAllDingoStatScreens()
    {
        string filePath;
        filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
        jsonData = File.ReadAllText(filePath);
        jsonDingos = JSON.Parse(jsonData) as JSONArray;

        Debug.Log("File path: " + filePath);

        // Check if file exists
        if (File.Exists(filePath))
        {
            try
            {
                // Read JSON data from file
                string jsonData = File.ReadAllText(filePath);
                Debug.Log("JSON data: " + jsonData);

                // Parse JSON data
                JSONArray jsonDingos = JSON.Parse(jsonData) as JSONArray;
                if (jsonDingos != null)
                {
                    // Instantiate the stat screen prefab for the first Dingo
                    JSONNode firstDingoData = jsonDingos[0];
                    JSONObject firstDingo = firstDingoData.AsObject;
                    pagenumber = 0;

                    xpBar.SetMaxHealth(firstDingo["MaxXP"]);
                    xpBar.SetHealth(firstDingo["XP"]);
                    healthBar.SetMaxHealth(firstDingo["MaxHealth"]);
                    healthBar.SetHealth(firstDingo["CurrentHealth"]);
                    // Create a new DingoID object based on the ID from the JSON data
                    int dingoID = firstDingo["DingoID"];
                    int move1id = firstDingo["Move1ID"];
                    int move2id = firstDingo["Move2ID"];
                    int move3id = firstDingo["Move3ID"];
                    int move4id = firstDingo["Move4ID"];
                    DingoID dingo = DingoDatabase.GetDingoByID(dingoID);
                    DingoMove activemove1 = DingoDatabase.GetMoveByID(move1id, dingo);
                    DingoMove activemove2 = DingoDatabase.GetMoveByID(move2id, dingo);
                    DingoMove activemove3 = DingoDatabase.GetMoveByID(move3id, dingo);
                    DingoMove activemove4 = DingoDatabase.GetMoveByID(move4id, dingo);
                    ActiveMove1.text = activemove1.Name;
                    ActiveMove2.text = activemove2.Name;
                    ActiveMove3.text = activemove3.Name;
                    ActiveMove4.text = activemove4.Name;
                    UpdateAllMoves(dingo.Moves);


                    // You can then use the dingo object as needed
                    Debug.Log("Loaded Dingo: " + dingo.Name);



                    // Populate the stat screen with the first Dingo's information
                    DESCstat.text = firstDingo["Description"];
                    NAMEstat.text = firstDingo["Name"];
                    TYPEstat.text = firstDingo["Type"];
                    IDstat.text = firstDingo["ID"];
                    ATKstat.text = "Attack " + firstDingo["ATK"];
                    DEFstat.text = "Defense " + firstDingo["DEF"];
                    SPDstat.text = "Speed " + firstDingo["SPD"];
                    LVLstat.text = "Level " + firstDingo["Level"];
                    HPstat.text = "HP " + firstDingo["CurrentHealth"] + "/" + firstDingo["MaxHealth"];
                    XPstat.text = "XP " + firstDingo["XP"] + "/" + firstDingo["MaxXP"];
                    // Load Dingo icon sprite from Resources folder based on the icon path in JSON data
                    Sprite dingoSprite = Resources.Load<Sprite>(firstDingo["Sprite"]);
                    SPRITEstat.sprite = dingoSprite;
                    agentBingo = false;

                }
                else
                {
                    Debug.LogWarning("Failed to parse JSON data.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error reading JSON file: " + e.Message);
                // Print full stack trace if available
                Debug.LogException(e);
            }
        }
        else
        {
            if (healthBar != null)
            {
                // Call the HideHealthBar method
                healthBar.HideHealthBar();
            }
            else
            {
                Debug.LogError("HealthBar reference is null.");
            }
            Warning.enabled = true;
            Debug.LogWarning("Dingo JSON file not found at path: " + filePath);
        }
    }
    public void ShowNextDingoStatScreen()
    {
        string filePath;
        filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
        jsonData = File.ReadAllText(filePath);
        jsonDingos = JSON.Parse(jsonData) as JSONArray;
        // Ensure jsonDingos is not null
        if (jsonDingos == null)
        {
            Debug.LogError("JSON data is not loaded.");
            return;
        }

        // Check if pagenumber is within bounds
        if (pagenumber < 0 || pagenumber >= jsonDingos.Count)
        {
            Debug.LogWarning("Invalid pagenumber: " + pagenumber);
            return;
        }

        try
        {
            // Calculate the index of the Dingo based on the pagenumber
            int index = pagenumber + 1; // Adjust for zero-based indexing
            Debug.Log("Index:" + index);
            if (index >= 0 && index < jsonDingos.Count)
            {
                // Get data for the Dingo at the specified index
                JSONNode dingoData = jsonDingos[index];
                JSONObject dingo = dingoData.AsObject;


                DESCstat.text = dingo["Description"];
                NAMEstat.text = dingo["Name"];
                TYPEstat.text = dingo["Type"];
                IDstat.text = dingo["ID"];
                ATKstat.text = "Attack " + dingo["ATK"];
                DEFstat.text = "Defense " + dingo["DEF"];
                SPDstat.text = "Speed " + dingo["SPD"];
                LVLstat.text = "Level " + dingo["Level"];
                HPstat.text = "HP " + dingo["CurrentHealth"] + "/" + dingo["MaxHealth"];
                XPstat.text = "XP " + dingo["XP"] + "/" + dingo["MaxXP"];
                // Load Dingo icon sprite from Resources folder based on the icon path in JSON data
                Sprite dingoSprite = Resources.Load<Sprite>(dingo["Sprite"]);
                SPRITEstat.sprite = dingoSprite;

                xpBar.SetMaxHealth(dingo["MaxXP"]);
                xpBar.SetHealth(dingo["XP"]);
                healthBar.SetMaxHealth(dingo["MaxHealth"]);
                healthBar.SetHealth(dingo["CurrentHealth"]);
                int dingoID = dingo["DingoID"];
                int move1id = dingo["Move1ID"];
                int move2id = dingo["Move2ID"];
                int move3id = dingo["Move3ID"];
                int move4id = dingo["Move4ID"];
                DingoID nextdingo = DingoDatabase.GetDingoByID(dingoID);
                DingoMove activemove1 = DingoDatabase.GetMoveByID(move1id, nextdingo);
                DingoMove activemove2 = DingoDatabase.GetMoveByID(move2id, nextdingo);
                DingoMove activemove3 = DingoDatabase.GetMoveByID(move3id, nextdingo);
                DingoMove activemove4 = DingoDatabase.GetMoveByID(move4id, nextdingo);
                ActiveMove1.text = activemove1.Name;
                ActiveMove2.text = activemove2.Name;
                ActiveMove3.text = activemove3.Name;
                ActiveMove4.text = activemove4.Name;
                UpdateAllMoves(nextdingo.Moves);


                pagenumber++;
                agentBingo = false;

            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error showing next Dingo stat screen: " + e.Message);
            // Print full stack trace if available
            Debug.LogException(e);
        }
    }
    public void ShowPreviousDingoStatScreen()
    {
        string filePath;
        filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
        jsonData = File.ReadAllText(filePath);
        jsonDingos = JSON.Parse(jsonData) as JSONArray;
        // Ensure jsonDingos is not null
        if (jsonDingos == null)
        {
            Debug.LogError("JSON data is not loaded.");
            return;
        }

        // Check if pagenumber is within bounds
        if (pagenumber < 1 || pagenumber > jsonDingos.Count)
        {
            return;
        }

        try
        {
            // Calculate the index of the Dingo based on the pagenumber
            int index = pagenumber - 1; // Adjust for zero-based indexing
            Debug.Log("Index:" + index);

            // Get data for the Dingo at the specified index
            JSONNode dingoData = jsonDingos[index];
            JSONObject dingo = dingoData.AsObject;


            DESCstat.text = dingo["Description"];
            NAMEstat.text = dingo["Name"];
            TYPEstat.text = dingo["Type"];
            IDstat.text = dingo["ID"];
            ATKstat.text = "Attack " + dingo["ATK"];
            DEFstat.text = "Defense " + dingo["DEF"];
            SPDstat.text = "Speed " + dingo["SPD"];
            LVLstat.text = "Level " + dingo["Level"];
            HPstat.text = "HP " + dingo["CurrentHealth"] + "/" + dingo["MaxHealth"];
            XPstat.text = "XP " + dingo["XP"] + "/" + dingo["MaxXP"];
            // Load Dingo icon sprite from Resources folder based on the icon path in JSON data
            Sprite dingoSprite = Resources.Load<Sprite>(dingo["Sprite"]);
            SPRITEstat.sprite = dingoSprite;
            xpBar.SetMaxHealth(dingo["MaxXP"]);
            xpBar.SetHealth(dingo["XP"]);
            healthBar.SetMaxHealth(dingo["MaxHealth"]);
            healthBar.SetHealth(dingo["CurrentHealth"]);
            int dingoID = dingo["DingoID"];
            int move1id = dingo["Move1ID"];
            int move2id = dingo["Move2ID"];
            int move3id = dingo["Move3ID"];
            int move4id = dingo["Move4ID"];
            DingoID nextdingo = DingoDatabase.GetDingoByID(dingoID);
            DingoMove activemove1 = DingoDatabase.GetMoveByID(move1id, nextdingo);
            DingoMove activemove2 = DingoDatabase.GetMoveByID(move2id, nextdingo);
            DingoMove activemove3 = DingoDatabase.GetMoveByID(move3id, nextdingo);
            DingoMove activemove4 = DingoDatabase.GetMoveByID(move4id, nextdingo);
            ActiveMove1.text = activemove1.Name;
            ActiveMove2.text = activemove2.Name;
            ActiveMove3.text = activemove3.Name;
            ActiveMove4.text = activemove4.Name;
            UpdateAllMoves(nextdingo.Moves);

            agentBingo = false;

            pagenumber--;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error showing next Dingo stat screen: " + e.Message);
            // Print full stack trace if available
            Debug.LogException(e);
        }
    }
    public void SetActiveDingo()
    {
        if (agentBingo)
        {
            return;
        }
        string filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
        Debug.Log("File path: " + filePath);

        // Check if file exists
        if (File.Exists(filePath))
        {
            // Read JSON data from file
            string jsonData = File.ReadAllText(filePath);
            JSONArray jsonDingos = JSON.Parse(jsonData) as JSONArray;

            // Identify the active dingo (e.g., based on some condition)
            int activeDingoIndex = pagenumber; // Adjust this according to your logic

            // Make sure the active dingo index is within the valid range
            if (activeDingoIndex >= 0 && activeDingoIndex < jsonDingos.Count)
            {
                // Remove the active dingo from its current position
                JSONNode activeDingoData = jsonDingos[activeDingoIndex];
                jsonDingos.Remove(activeDingoData);

                // Add the active dingo at the beginning of the JSON array
                jsonDingos.Add(null); // Add a placeholder at the end
                for (int i = jsonDingos.Count - 1; i > 0; i--)
                {
                    jsonDingos[i] = jsonDingos[i - 1];
                }
                jsonDingos[0] = activeDingoData;

                // Write the modified JSON data back to the file
                File.WriteAllText(filePath, jsonDingos.ToString());

                Debug.Log("Active dingo moved to the front of the JSON file.");
            }
            else
            {
                Debug.LogError("Invalid active dingo index.");
            }

        }
        else
        {
            Debug.LogError("File 'dingos.json' does not exist.");
        }
        ShowAllDingoStatScreens();
    }
    public void ExitGame()
    {
        Bingo = LocalPlayerManager.Instance.LocalPlayer;
        SaveCoordinates();
        // Exit the application
        Application.Quit();
    }
    public void SaveCoordinates()
    {
        // Save the position and rotation of the object
        PlayerPrefs.SetFloat("PosX", Bingo.transform.localPosition.x);
        PlayerPrefs.SetFloat("PosY", Bingo.transform.localPosition.y);
        PlayerPrefs.SetFloat("PosZ", Bingo.transform.localPosition.z);
        // Save PlayerPrefs to disk
        PlayerPrefs.Save();

        Debug.Log("Coordinates saved.");
    }

}
