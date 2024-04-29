using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DingoSystem;
using SimpleJSON;
using System.IO;
using System;

public class PlayerDingos : MonoBehaviour
{
    public static PlayerDingos Instance;
    public Transform PlayerDingoContent;
    public Transform StatDingoContent;
    public List<DingoID> DingoList = new List<DingoID>(); // Assuming DingoID is defined elsewhere.
    public GameObject DingoItem;
    public GameObject StatScreenPrefab; // Reference to the stat screen prefab
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
    public Image SPRITEstat;
    public Text ActiveMove1;
    public Text ActiveMove2;
    public Text ActiveMove3;
    public Text ActiveMove4;
    public bool move1toggle = false;

    public Text MoveName1;
    public Text MovePower1;
    public Text MoveAccuracy1;
    public Text MoveDescription1;
    public Text MoveType1;

    public Text MoveName2;
    public Text MovePower2;
    public Text MoveAccuracy2;
    public Text MoveDescription2;
    public Text MoveType2;

    public Text MoveName3;
    public Text MovePower3;
    public Text MoveAccuracy3;
    public Text MoveDescription3;
    public Text MoveType3;

    public Text MoveName4;
    public Text MovePower4;
    public Text MoveAccuracy4;
    public Text MoveDescription4;
    public Text MoveType4;

    public Text MoveName5;
    public Text MovePower5;
    public Text MoveAccuracy5;
    public Text MoveDescription5;
    public Text MoveType5;

    public Text MoveName6;
    public Text MovePower6;
    public Text MoveAccuracy6;
    public Text MoveDescription6;
    public Text MoveType6;

    public Text MoveName7;
    public Text MovePower7;
    public Text MoveAccuracy7;
    public Text MoveDescription7;
    public Text MoveType7;

    public Text MoveName8;
    public Text MovePower8;
    public Text MoveAccuracy8;
    public Text MoveDescription8;
    public Text MoveType8;

    public Text MoveName9;
    public Text MovePower9;
    public Text MoveAccuracy9;
    public Text MoveDescription9;
    public Text MoveType9;

    public Text MoveName10;
    public Text MovePower10;
    public Text MoveAccuracy10;
    public Text MoveDescription10;
    public Text MoveType10;


    private void Awake()
    {
        Instance = this;
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
    public void ShowAllDingoStatScreens()
    {
        string filePath;
        filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
        jsonData = File.ReadAllText(filePath);
        jsonDingos = JSON.Parse(jsonData) as JSONArray;
        // Clear existing stat screen items before populating the list
        foreach (Transform child in StatDingoContent)
        {
            Destroy(child.gameObject);
        }

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
                    DingoMove dingomove1 = dingo.Moves[0];
                    DingoMove dingomove2 = dingo.Moves[1];
                    DingoMove dingomove3 = dingo.Moves[2];
                    DingoMove dingomove4 = dingo.Moves[3];
                    DingoMove dingomove5 = dingo.Moves[4];
                    DingoMove dingomove6 = dingo.Moves[5];
                    DingoMove dingomove7 = dingo.Moves[6];
                    DingoMove dingomove8 = dingo.Moves[7];
                    DingoMove dingomove9 = dingo.Moves[8];
                    DingoMove dingomove10 = dingo.Moves[9];


                    // For Move 1
                    MoveName1.text = dingomove1.Name;
                    MovePower1.text = dingomove1.Power.ToString();
                    MoveAccuracy1.text = dingomove1.Accuracy.ToString();
                    MoveType1.text = dingomove1.Type.ToString();
                    MoveDescription1.text = dingomove1.Description;

                    // For Move 2
                    MoveName2.text = dingomove2.Name;
                    MovePower2.text = dingomove2.Power.ToString();
                    MoveAccuracy2.text = dingomove2.Accuracy.ToString();
                    MoveType2.text = dingomove2.Type.ToString();
                    MoveDescription2.text = dingomove2.Description;

                    // For Move 3
                    MoveName3.text = dingomove3.Name;
                    MovePower3.text = dingomove3.Power.ToString();
                    MoveAccuracy3.text = dingomove3.Accuracy.ToString();
                    MoveType3.text = dingomove3.Type.ToString();
                    MoveDescription3.text = dingomove3.Description;

                    // For Move 4
                    MoveName4.text = dingomove4.Name;
                    MovePower4.text = dingomove4.Power.ToString();
                    MoveAccuracy4.text = dingomove4.Accuracy.ToString();
                    MoveType4.text = dingomove4.Type.ToString();
                    MoveDescription4.text = dingomove4.Description;

                    // For Move 5
                    MoveName5.text = dingomove5.Name;
                    MovePower5.text = dingomove5.Power.ToString();
                    MoveAccuracy5.text = dingomove5.Accuracy.ToString();
                    MoveType5.text = dingomove5.Type.ToString();
                    MoveDescription5.text = dingomove5.Description;

                    // For Move 6
                    MoveName6.text = dingomove6.Name;
                    MovePower6.text = dingomove6.Power.ToString();
                    MoveAccuracy6.text = dingomove6.Accuracy.ToString();
                    MoveType6.text = dingomove6.Type.ToString();
                    MoveDescription6.text = dingomove6.Description;

                    // For Move 7
                    MoveName7.text = dingomove7.Name;
                    MovePower7.text = dingomove7.Power.ToString();
                    MoveAccuracy7.text = dingomove7.Accuracy.ToString();
                    MoveType7.text = dingomove7.Type.ToString();
                    MoveDescription7.text = dingomove7.Description;

                    // For Move 8
                    MoveName8.text = dingomove8.Name;
                    MovePower8.text = dingomove8.Power.ToString();
                    MoveAccuracy8.text = dingomove8.Accuracy.ToString();
                    MoveType8.text = dingomove8.Type.ToString();
                    MoveDescription8.text = dingomove8.Description;

                    // For Move 9
                    MoveName9.text = dingomove9.Name;
                    MovePower9.text = dingomove9.Power.ToString();
                    MoveAccuracy9.text = dingomove9.Accuracy.ToString();
                    MoveType9.text = dingomove9.Type.ToString();
                    MoveDescription9.text = dingomove9.Description;

                    // For Move 10
                    MoveName10.text = dingomove10.Name;
                    MovePower10.text = dingomove10.Power.ToString();
                    MoveAccuracy10.text = dingomove10.Accuracy.ToString();
                    MoveType10.text = dingomove10.Type.ToString();
                    MoveDescription10.text = dingomove10.Description;



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

                // Clear existing stat screen items before populating the list
                foreach (Transform child in StatDingoContent)
                {
                    Destroy(child.gameObject);
                }

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
                DingoMove dingomove1 = nextdingo.Moves[0];
                DingoMove dingomove2 = nextdingo.Moves[1];
                DingoMove dingomove3 = nextdingo.Moves[2];
                DingoMove dingomove4 = nextdingo.Moves[3];
                DingoMove dingomove5 = nextdingo.Moves[4];
                DingoMove dingomove6 = nextdingo.Moves[5];
                DingoMove dingomove7 = nextdingo.Moves[6];
                DingoMove dingomove8 = nextdingo.Moves[7];
                DingoMove dingomove9 = nextdingo.Moves[8];
                DingoMove dingomove10 = nextdingo.Moves[9];



                // For Move 1
                MoveName1.text = dingomove1.Name;
                MovePower1.text = dingomove1.Power.ToString();
                MoveAccuracy1.text = dingomove1.Accuracy.ToString();
                MoveType1.text = dingomove1.Type.ToString();
                MoveDescription1.text = dingomove1.Description;

                // For Move 2
                MoveName2.text = dingomove2.Name;
                MovePower2.text = dingomove2.Power.ToString();
                MoveAccuracy2.text = dingomove2.Accuracy.ToString();
                MoveType2.text = dingomove2.Type.ToString();
                MoveDescription2.text = dingomove2.Description;

                // For Move 3
                MoveName3.text = dingomove3.Name;
                MovePower3.text = dingomove3.Power.ToString();
                MoveAccuracy3.text = dingomove3.Accuracy.ToString();
                MoveType3.text = dingomove3.Type.ToString();
                MoveDescription3.text = dingomove3.Description;

                // For Move 4
                MoveName4.text = dingomove4.Name;
                MovePower4.text = dingomove4.Power.ToString();
                MoveAccuracy4.text = dingomove4.Accuracy.ToString();
                MoveType4.text = dingomove4.Type.ToString();
                MoveDescription4.text = dingomove4.Description;

                // For Move 5
                MoveName5.text = dingomove5.Name;
                MovePower5.text = dingomove5.Power.ToString();
                MoveAccuracy5.text = dingomove5.Accuracy.ToString();
                MoveType5.text = dingomove5.Type.ToString();
                MoveDescription5.text = dingomove5.Description;

                // For Move 6
                MoveName6.text = dingomove6.Name;
                MovePower6.text = dingomove6.Power.ToString();
                MoveAccuracy6.text = dingomove6.Accuracy.ToString();
                MoveType6.text = dingomove6.Type.ToString();
                MoveDescription6.text = dingomove6.Description;

                // For Move 7
                MoveName7.text = dingomove7.Name;
                MovePower7.text = dingomove7.Power.ToString();
                MoveAccuracy7.text = dingomove7.Accuracy.ToString();
                MoveType7.text = dingomove7.Type.ToString();
                MoveDescription7.text = dingomove7.Description;

                // For Move 8
                MoveName8.text = dingomove8.Name;
                MovePower8.text = dingomove8.Power.ToString();
                MoveAccuracy8.text = dingomove8.Accuracy.ToString();
                MoveType8.text = dingomove8.Type.ToString();
                MoveDescription8.text = dingomove8.Description;

                // For Move 9
                MoveName9.text = dingomove9.Name;
                MovePower9.text = dingomove9.Power.ToString();
                MoveAccuracy9.text = dingomove9.Accuracy.ToString();
                MoveType9.text = dingomove9.Type.ToString();
                MoveDescription9.text = dingomove9.Description;

                // For Move 10
                MoveName10.text = dingomove10.Name;
                MovePower10.text = dingomove10.Power.ToString();
                MoveAccuracy10.text = dingomove10.Accuracy.ToString();
                MoveType10.text = dingomove10.Type.ToString();
                MoveDescription10.text = dingomove10.Description;




                pagenumber++;
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

            // Clear existing stat screen items before populating the list
            foreach (Transform child in StatDingoContent)
            {
                Destroy(child.gameObject);
            }

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
            DingoMove dingomove1 = nextdingo.Moves[0];
            DingoMove dingomove2 = nextdingo.Moves[1];
            DingoMove dingomove3 = nextdingo.Moves[2];
            DingoMove dingomove4 = nextdingo.Moves[3];
            DingoMove dingomove5 = nextdingo.Moves[4];
            DingoMove dingomove6 = nextdingo.Moves[5];
            DingoMove dingomove7 = nextdingo.Moves[6];
            DingoMove dingomove8 = nextdingo.Moves[7];
            DingoMove dingomove9 = nextdingo.Moves[8];
            DingoMove dingomove10 = nextdingo.Moves[9];



            // For Move 1
            MoveName1.text = dingomove1.Name;
            MovePower1.text = dingomove1.Power.ToString();
            MoveAccuracy1.text = dingomove1.Accuracy.ToString();
            MoveType1.text = dingomove1.Type.ToString();
            MoveDescription1.text = dingomove1.Description;

            // For Move 2
            MoveName2.text = dingomove2.Name;
            MovePower2.text = dingomove2.Power.ToString();
            MoveAccuracy2.text = dingomove2.Accuracy.ToString();
            MoveType2.text = dingomove2.Type.ToString();
            MoveDescription2.text = dingomove2.Description;

            // For Move 3
            MoveName3.text = dingomove3.Name;
            MovePower3.text = dingomove3.Power.ToString();
            MoveAccuracy3.text = dingomove3.Accuracy.ToString();
            MoveType3.text = dingomove3.Type.ToString();
            MoveDescription3.text = dingomove3.Description;

            // For Move 4
            MoveName4.text = dingomove4.Name;
            MovePower4.text = dingomove4.Power.ToString();
            MoveAccuracy4.text = dingomove4.Accuracy.ToString();
            MoveType4.text = dingomove4.Type.ToString();
            MoveDescription4.text = dingomove4.Description;

            // For Move 5
            MoveName5.text = dingomove5.Name;
            MovePower5.text = dingomove5.Power.ToString();
            MoveAccuracy5.text = dingomove5.Accuracy.ToString();
            MoveType5.text = dingomove5.Type.ToString();
            MoveDescription5.text = dingomove5.Description;

            // For Move 6
            MoveName6.text = dingomove6.Name;
            MovePower6.text = dingomove6.Power.ToString();
            MoveAccuracy6.text = dingomove6.Accuracy.ToString();
            MoveType6.text = dingomove6.Type.ToString();
            MoveDescription6.text = dingomove6.Description;

            // For Move 7
            MoveName7.text = dingomove7.Name;
            MovePower7.text = dingomove7.Power.ToString();
            MoveAccuracy7.text = dingomove7.Accuracy.ToString();
            MoveType7.text = dingomove7.Type.ToString();
            MoveDescription7.text = dingomove7.Description;

            // For Move 8
            MoveName8.text = dingomove8.Name;
            MovePower8.text = dingomove8.Power.ToString();
            MoveAccuracy8.text = dingomove8.Accuracy.ToString();
            MoveType8.text = dingomove8.Type.ToString();
            MoveDescription8.text = dingomove8.Description;

            // For Move 9
            MoveName9.text = dingomove9.Name;
            MovePower9.text = dingomove9.Power.ToString();
            MoveAccuracy9.text = dingomove9.Accuracy.ToString();
            MoveType9.text = dingomove9.Type.ToString();
            MoveDescription9.text = dingomove9.Description;

            // For Move 10
            MoveName10.text = dingomove10.Name;
            MovePower10.text = dingomove10.Power.ToString();
            MoveAccuracy10.text = dingomove10.Accuracy.ToString();
            MoveType10.text = dingomove10.Type.ToString();
            MoveDescription10.text = dingomove10.Description;


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
        // Exit the application
        Application.Quit();
    }

}
