using DingoSystem;
using SimpleJSON;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BattleDingos : MonoBehaviour
{
    public Transform PlayerDingoContent;
    private JSONArray jsonDingos;
    public GameObject DingoItem;
    private string filePath;
    public GameObject dingosUI;
    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
    }
    public void ListDingos()
    {
        dingosUI.SetActive(true);

        // Clear existing Dingo items before populating the list
        foreach (Transform child in PlayerDingoContent)
        {
            Destroy(child.gameObject);
        }

        int dingoCount = 0;
        if (File.Exists(filePath))
        {
            try
            {
                string existingData = File.ReadAllText(filePath);
                jsonDingos = JSON.Parse(existingData) as JSONArray;

                if (jsonDingos != null)
                {
                    foreach (JSONNode dingoData in jsonDingos)
                    {
                        JSONObject dingo = dingoData.AsObject;
                        GameObject obj = Instantiate(DingoItem, PlayerDingoContent);

                        // Get UI components
                        var dingoName = obj.transform.Find("ItemName").GetComponent<Text>();
                        var dingoIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
                        var dingoType = obj.transform.Find("ItemType").GetComponent<Text>();
                        var dingoID = obj.transform.Find("ItemID").GetComponent<Text>();
                        var dingoHP = obj.transform.Find("ItemHP").GetComponent<Text>(); // Assuming HP display exists

                        // Set basic info
                        dingoName.text = dingo["Name"];
                        dingoType.text = dingo["Type"];
                        dingoID.text = dingoCount.ToString();

                        // Add HP display if available
                        int currentHP = dingo["CurrentHealth"].AsInt;
                        int maxHP = dingo["MaxHealth"].AsInt;
                        if (dingoHP != null)
                        {
                            dingoHP.text = $"{currentHP}/{maxHP}";
                        }

                        // Load Dingo icon
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

                        // Handle fainted Dingos (HP ? 0)
                        if (currentHP <= 0)
                        {
                            // Visual indication of fainted Dingo
                            obj.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Gray out

                            // Disable interaction but keep visible
                            var button = obj.GetComponent<Button>();
                            button.interactable = false;

                            // Add tooltip or explanation
                            if (dingoName != null)
                            {
                                dingoName.text += " (Fainted)";
                            }
                        }

                        // Add click handler only for healthy Dingos
                        if (currentHP > 0)
                        {
                            int dingoIndex = dingoCount;
                            obj.GetComponent<Button>().onClick.AddListener(() => OnDingoItemClick(dingoIndex));
                        }
                        else
                        {
                            // Optional: Add handler that explains why they can't select this Dingo
                            obj.GetComponent<Button>().onClick.AddListener(() => {
                                Debug.Log("This Dingo has fainted and cannot be selected");
                                // You could show a message to the player here
                            });
                        }

                        dingoCount++;
                    }
                    AdjustContentWindowSize(dingoCount);

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

    private void AdjustContentWindowSize(int dingoCount)
    {
        RectTransform contentRect = PlayerDingoContent.GetComponent<RectTransform>();
        float bottomValue = 0f;
        if (dingoCount >= 36)
        {
            bottomValue = -2000f;
        }
        else if (dingoCount >= 31 && dingoCount <= 35)
        {
            bottomValue = -1000f;
        }
        else if(dingoCount >= 26 && dingoCount <= 30)
        {
            bottomValue = -750f;
        }
        else if (dingoCount >= 21 && dingoCount <= 25)
        {
            bottomValue = -500f;
        }
        else if (dingoCount >= 16 && dingoCount <= 20)
        {
            bottomValue = -250f;
        }
        // 15 or under keeps the default 0 value

        contentRect.offsetMin = new Vector2(contentRect.offsetMin.x, bottomValue);

    }
    private void OnDingoItemClick(int dingoIndex)
    {
        Debug.Log("Dingo item clicked: " + dingoIndex);
        // Call Use2nd() script with the selected Dingo index
        dingosUI.gameObject.SetActive(false);
        BattleStarter.Instance.SwitchPlayerDingos(dingoIndex);
    }
    public void CatchSlot1()
    {
        SaveDingo(2);
    }
    public void CatchSlot2()
    {
        SaveDingo(3);
    }
    public void OnCatchButtonPressed()
    {
        BattleStarter.Instance.NewAssignCatchButtons();
    }
    public void SaveDingo(int slot)
    {
        int slotIndex = 1; // Initialize slotIndex to 1

        // Load existing Dingos data if it exists
        JSONArray jsonDingos;
        string filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
        if (File.Exists(filePath))
        {
            string existingData = File.ReadAllText(filePath);
            jsonDingos = JSON.Parse(existingData) as JSONArray;
            if (jsonDingos == null)
            {
                // If parsing fails, create a new JSONArray
                Debug.LogWarning("Failed to parse existing Dingos data. Creating a new JSONArray.");
                jsonDingos = new JSONArray();
            }
            else
            {
                // Iterate over existing Dingos to find the highest slot index
                foreach (JSONNode dingoData in jsonDingos)
                {
                    JSONObject dingoObj = dingoData.AsObject;
                    if (dingoObj.HasKey("ID"))
                    {
                        int id = dingoObj["ID"].AsInt;
                        slotIndex = Mathf.Max(slotIndex, id); // Update slotIndex if higher ID found
                    }
                }
                // Increment slotIndex by 1 to get the next available slot
                slotIndex++;
            }
        }
        else
        {
            // If file doesn't exist, create a new JSONArray
            Debug.LogWarning("Dingos data file not found. Creating a new JSONArray.");
            jsonDingos = new JSONArray();
        }

        // Debug information
        Debug.Log("Loaded existing Dingos data from file: " + filePath);
        Debug.Log("Number of existing Dingos: " + jsonDingos.Count);

        // Create JSON object for the new Dingo
        JSONObject jsonDingo = new JSONObject();
        NetworkDingo networkDingo = BattleHandler.GetPlayerNetworkDingo(NetworkManager.Singleton.LocalClientId, slot);
        // Debug information
        Debug.Log("New Dingo created with ID: " + slotIndex);

        // Add properties of the Dingo object to the JSON object
        jsonDingo.Add("ID", slotIndex);
        jsonDingo.Add("DingoID", networkDingo.id.Value);
        jsonDingo.Add("Name", networkDingo.name.Value.ToString());
        jsonDingo.Add("Type", networkDingo.type.Value.ToString());
        jsonDingo.Add("Description", DingoDatabase.GetDingoDescriptionByID(networkDingo.id.Value));
        jsonDingo.Add("CurrentHealth", networkDingo.hp.Value);
        jsonDingo.Add("ATK", networkDingo.attack.Value);
        jsonDingo.Add("DEF", networkDingo.defense.Value);
        jsonDingo.Add("SPD", networkDingo.speed.Value);
        jsonDingo.Add("Sprite", networkDingo.spritePath.Value.ToString());
        jsonDingo.Add("MaxHealth", networkDingo.maxHP.Value);
        jsonDingo.Add("XP", networkDingo.xp.Value);
        jsonDingo.Add("MaxXP", networkDingo.maxXP.Value);
        jsonDingo.Add("Level", networkDingo.level.Value);
        jsonDingo.Add("Move1ID", networkDingo.move1.Value);
        jsonDingo.Add("Move2ID", networkDingo.move2.Value);
        jsonDingo.Add("Move3ID", networkDingo.move3.Value);
        jsonDingo.Add("Move4ID", networkDingo.move4.Value);

        // Add the current Dingo object to the JSON array
        jsonDingos.Add(jsonDingo);

        // Debug information
        Debug.Log("Dingo added to JSON array.");

        // Convert the JSON array to a string
        string jsonString = jsonDingos.ToString();

        // Write the JSON data to the file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.Write(jsonString);
        }

        Debug.Log("Dingos data saved to: " + filePath);
    }

}
