using System;
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
    public event Action OnDialogueStart;
    public event Action OnDialogueEnd;
    public event Action OnShopOpened;
    public event Action OnShopClosed;
    private bool isDialogueActive = false;

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
        movementEnabled = false;
        UpdateMovement();

        if (!isDialogueActive)
        {
            isDialogueActive = true;
            OnDialogueStart?.Invoke();
        }
    }

    public void CloseDialogShopScreen()
    {
        dialogBox.SetActive(false);
        buyMenu.SetActive(false);
        buyMenuPlayerInventory.SetActive(false);
        movementEnabled = true;
        UpdateMovement();
        exitableDialog = true;

        if (isDialogueActive)
        {
            isDialogueActive = false;
            OnDialogueEnd?.Invoke();
        }
    }

    public void ShowShopScreen()
    {
        dialogBox.SetActive(false);
        buyMenu.SetActive(true);
        buyMenuPlayerInventory.SetActive(true);
        OnShopOpened?.Invoke(); // Trigger event
    }

    // Modified to trigger shop closed event
    public void CloseShopScreen()
    {
        buyMenu.SetActive(false);
        buyMenuPlayerInventory.SetActive(false);
        OnShopClosed?.Invoke(); // Trigger event
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
    // Modify the DisplayDialogButton method to accept an Action
    public void DisplayDialogButton(string text, UnityEngine.Events.UnityAction action)
    {
        GameObject dialogButton = Instantiate(dialogButtonPrefab, dialogButtonContent);

        var dialogtext = dialogButton.transform.Find("DialogText").GetComponent<Text>();
        dialogtext.text = text;

        Button button = dialogButton.GetComponent<Button>();
        button.onClick.RemoveAllListeners(); // Clear existing listeners
        button.onClick.AddListener(action);

        // Always add the close dialog listener too
        button.onClick.AddListener(() => {
            if (exitableDialog) CloseDialogShopScreen();
        });
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


    void Update()
    {
        if (exitableDialog)
        {
            if (Input.GetButtonDown("Fire1") && IsDialogActive())
            {
                dialogBox.SetActive(false);
                movementEnabled = true;
                UpdateMovement();

                if (isDialogueActive)
                {
                    isDialogueActive = false;
                    OnDialogueEnd?.Invoke();
                }
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
