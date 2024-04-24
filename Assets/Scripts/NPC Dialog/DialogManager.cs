using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    private bool movementEnabled = true;

    public bool IsDialogActive()
    {
        // Check if the dialog box is active
        return dialogBox.activeSelf;
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
