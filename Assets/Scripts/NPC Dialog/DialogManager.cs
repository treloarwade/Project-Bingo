using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    private GameObject dialogBox;
    public bool movementEnabled = true;

    public bool IsDialogActive()
    {
        return dialogBox.activeSelf;
    }
    private void Start()
    {
        dialogBox = GameObject.Find("Canvas/NonBattle/DialogBox");
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && IsDialogActive())
        {
            // Close dialog box and enable movement
            dialogBox.SetActive(false);
            movementEnabled = true;
            UpdateMovement();
        }
    }

    void UpdateMovement()
    {
        // Enable or disable player movement based on movementEnabled flag
        Movement movementScript = GetComponent<Movement>();
        if (movementScript != null)
        {
            movementScript.enabled = movementEnabled;
        }
    }

}
