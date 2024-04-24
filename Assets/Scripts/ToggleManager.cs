using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DingoSystem;
using System.Collections;

public class ToggleManager : MonoBehaviour
{
    public List<Toggle> toggles = new List<Toggle>();
    private int checkedCount = 0;
    private List<int> checkedIndices = new List<int>();
    public Text PageNumber;
    private JSONArray jsonDingos;
    public string filePath;
    public string jsonData;
    public Text Success;
    public Text Move1Text;
    public Text Move2Text;
    public Text Move3Text;
    public Text Move4Text;

    void Start()
    {
        // Set up event listeners for each toggle
        for (int i = 0; i < toggles.Count; i++)
        {
            int index = i; // Capture the current index in the loop
            toggles[index].onValueChanged.AddListener(delegate { ToggleValueChanged(index); });
        }
    }

    private void ToggleValueChanged(int toggleIndex)
    {
        filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
        jsonData = File.ReadAllText(filePath);
        jsonDingos = JSON.Parse(jsonData) as JSONArray;
        if (toggles[toggleIndex].isOn)
        {
            checkedCount++;
            checkedIndices.Add(toggleIndex); // Store the index of the checked toggle

            if (checkedCount >= 4)
            {
                int pageNumber = GetComponent<PlayerDingos>().pagenumber;
                // Use pageNumber value
                Debug.Log("PageNumber: " + pageNumber);
                if (pageNumber >= 0 && pageNumber < jsonDingos.Count)
                {
                    JSONNode dingoData = jsonDingos[pageNumber];
                    JSONObject dingo = dingoData.AsObject;
                    Debug.Log(dingo["Name"]);

                    // Overwrite Move IDs with checked indices
                    // Overwrite Move IDs with checked indices
                    for (int i = 0; i < checkedIndices.Count; i++)
                    {
                        int moveIndex = i + 1; // Moves are indexed from 1 to 4
                        if (moveIndex <= 4)
                        {
                            int moveID = checkedIndices[i];
                            dingo["Move" + moveIndex + "ID"] = moveID;
                            Debug.Log("Move" + moveIndex + "ID set to: " + moveID);
                        }
                        else
                        {
                            Debug.LogWarning("Move index out of range: " + moveIndex);
                        }

                    }
                    string jsonString = jsonDingos.ToString();
                    File.WriteAllText(filePath, jsonString);
                    Debug.Log("JSON updated and saved to file: " + filePath);
                    //This doesn't work because it didn't write it to the json yet
                    int dingoID = dingo["DingoID"];
                    DingoID dingom = DingoDatabase.GetDingoByID(dingoID);
                    DingoMove activemove1 = DingoDatabase.GetMoveByID(checkedIndices[0], dingom);
                    DingoMove activemove2 = DingoDatabase.GetMoveByID(checkedIndices[1], dingom);
                    DingoMove activemove3 = DingoDatabase.GetMoveByID(checkedIndices[2], dingom);
                    DingoMove activemove4 = DingoDatabase.GetMoveByID(checkedIndices[3], dingom);
                    jsonData = File.ReadAllText(filePath);
                    jsonDingos = JSON.Parse(jsonData) as JSONArray;
                    Move1Text.text = activemove1.Name;
                    Move2Text.text = activemove2.Name;
                    Move3Text.text = activemove3.Name;
                    Move4Text.text = activemove4.Name;
                    //To fix this we have to add a listener maybe
                }
                else
                {
                    Debug.LogWarning("Invalid page number.");
                }


                // Make all toggles invisible
                SetTogglesVisible(false);

                // Output checked toggles to console for debugging
                Debug.Log("Checked Toggles:");
                foreach (int index in checkedIndices)
                {
                    Debug.Log("Toggle " + index + " is checked.");
                }

                // Turn off all toggles
                foreach (Toggle toggle in toggles)
                {
                    toggle.isOn = false;
                }

                // Reset checked count
                checkedCount = 0;
                checkedIndices.Clear(); // Clear checked indices list
            }
        }
        else
        {
            checkedCount--;
            checkedIndices.Remove(toggleIndex); // Remove the index if the toggle is unchecked
        }
    }
    private void SetTogglesVisible(bool isVisible)
    {
        foreach (Toggle toggle in toggles)
        {
            toggle.gameObject.SetActive(isVisible);
        }
    }
}
