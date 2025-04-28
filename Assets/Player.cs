using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using System.Collections;


public class Player : NetworkBehaviour
{
    public int playerNumber;
    private GameObject buttonGameObject;
    public NetworkVariable<int> itemEquipped = new NetworkVariable<int>();
    private KnifeLoader knifeLoader;
    private bool inBattle = false;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {

            Debug.Log($"I am Player {playerNumber}");
            SetupKnife();

        }
        itemEquipped.Value = -1;
        GetComponent<PlayerManager>().PlayerNumber.OnValueChanged += OnPlayerNumberChanged;
        itemEquipped.OnValueChanged += OnItemEquippedChanged;
        playerNumber = GetComponent<PlayerManager>().PlayerNumber.Value;
        RenamePlayerObject();

    }
    private void SetupKnife()
    {
        buttonGameObject = GameObject.Find("Knife Animation Button");

        Button button = buttonGameObject.GetComponent<Button>();

        knifeLoader = GetComponent<KnifeLoader>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(InspectServerRPC);
    }
    [ServerRpc(RequireOwnership = false)]
    private void InspectServerRPC()
    {
        InspectClientRPC();
    }
    [ClientRpc]
    private void InspectClientRPC()
    {
        if (knifeLoader == null)
        {
            knifeLoader = GetComponent<KnifeLoader>();
        }
        knifeLoader.Inspect();
    }

    private void OnItemEquippedChanged(int oldValue, int newValue)
    {
        InventoryManager.Instance.SetItemClientRpc(newValue, playerNumber);
    }

    private void OnPlayerNumberChanged(int oldValue, int newValue)
    {
        playerNumber = newValue;
        Debug.Log($"Player number changed to: {playerNumber}");
        // Rename the GameObject locally
        RenamePlayerObject();
    }

    // Renames the GameObject to "Player" followed by the player number
    private void RenamePlayerObject()
    {
        if (transform != null)
        {
            string newName = $"Player{playerNumber}";
            transform.name = newName;
            Debug.Log($"Renamed GameObject to: {newName}");
        }
        else
        {
            Debug.LogWarning("No transform found to rename.");
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsOwner)
        {
            // Unsubscribe from PlayerNumber changes
            GetComponent<PlayerManager>().PlayerNumber.OnValueChanged -= OnPlayerNumberChanged;
        }
    }
}