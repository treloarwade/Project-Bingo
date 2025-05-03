using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    private GameObject dialogBox;
    private Text dialogBoxText;
    public GameObject dialogButtonPrefab;
    public GameObject buyMenu;
    public GameObject buyMenuPlayerInventory;
    public Transform dialogButtonContent;
    public bool exitableDialog = true;
    public bool movementEnabled = true;
    public static DialogManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void DisplayDialogIsExitable(bool isExitable, string text)
    {
        dialogBox.SetActive(true);
        dialogBoxText.text = text;
        exitableDialog = isExitable;
    }
    public void ClearDialogButtons()
    {
        foreach (Transform child in dialogButtonContent)
        {
            Destroy(child.gameObject);
        }
    }
    public void DisplayShopButton(List<int> itemIDs)
    {
        GameObject dialogButton = Instantiate(dialogButtonPrefab, dialogButtonContent);

        // Set item details
        var dialogtext = dialogButton.transform.Find("DialogText").GetComponent<Text>();

        dialogtext.text = "Shop";

        // Add click listener
        Button button = dialogButton.GetComponent<Button>();
        button.onClick.AddListener(() => OpenShop(itemIDs));
    }
    public void OpenShop(List<int> itemIDs)
    {
        ShowShopScreen();
        BuyMenuManager.Instance.DisplayItemsForSale(itemIDs);
    }
    public void DisplayDialogButton(string dialog)
    {
        GameObject dialogButton = Instantiate(dialogButtonPrefab, dialogButtonContent);

        // Set item details
        var dialogtext = dialogButton.transform.Find("DialogText").GetComponent<Text>();

        dialogtext.text = dialog;

        // Add click listener
        Button button = dialogButton.GetComponent<Button>();
        button.onClick.AddListener(() => CloseDialogShopScreen());
    }
    public bool IsDialogActive()
    {
        return dialogBox.activeSelf;
    }
    private void Start()
    {
        dialogBox = GameObject.Find("Canvas/NonBattle/DialogBox");
        dialogBoxText = dialogBox.transform.Find("Viewport1/Content/DialogBoxText").GetComponent<Text>();
        //buyMenu = GameObject.Find("Canvas/BuyMenu");
        //buyMenuPlayerInventory = GameObject.Find("Canvas/InventoryBuyMenu");
        dialogButtonContent = dialogBox.transform.Find("Viewport2/Content").GetComponent<Transform>();
    }
    public void ShowShopScreen()
    {
        dialogBox.SetActive(false);
        buyMenu.SetActive(true);
        buyMenuPlayerInventory.SetActive(true);
    }
    public void CloseDialogShopScreen()
    {
        dialogBox.SetActive(false);
        buyMenu.SetActive(false);
        buyMenuPlayerInventory.SetActive(false);
        movementEnabled = true;
        UpdateMovement();
        exitableDialog = true;
    }

    void Update()
    {
        if (exitableDialog)
        {
            if (Input.GetButtonDown("Fire1") && IsDialogActive())
            {
                dialogBox.SetActive(false);
                // Close dialog box and enable movement
                movementEnabled = true;
                UpdateMovement();
            }
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
