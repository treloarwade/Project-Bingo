using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    private int playerNumber;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            // Subscribe to PlayerNumber changes
            GetComponent<PlayerManager>().PlayerNumber.OnValueChanged += OnPlayerNumberChanged;

            // Initialize playerNumber with the current value
            playerNumber = GetComponent<PlayerManager>().PlayerNumber.Value;
            Debug.Log($"I am Player {playerNumber}");

            // Rename the GameObject locally
            RenamePlayerObject();
        }
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