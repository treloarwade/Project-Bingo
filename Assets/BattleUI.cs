using DingoSystem;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    private static Button[] moveButtons = new Button[4];

    private void Awake()
    {
        InitializeButtons();
    }

    private static void InitializeButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            moveButtons[i] = GameObject.Find($"MoveButton{i + 1}")?.GetComponent<Button>();

            if (moveButtons[i] == null)
            {
                Debug.LogError($"MoveButton{i + 1} could not be found! Make sure it exists in the scene and is named correctly.");
            }
        }
    }

    public static Button[] GetMoveButtons()
    {
        if (moveButtons[0] == null) // Ensure buttons are initialized before returning
        {
            InitializeButtons();
        }
        return moveButtons;
    }

}
