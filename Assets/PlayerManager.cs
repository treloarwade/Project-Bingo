using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PlayerManager : NetworkBehaviour
{
    // Static variables to track player numbers and battle status
    private static Dictionary<ulong, int> playerMapping = new Dictionary<ulong, int>(); // Maps clientId to player number
    private static Dictionary<ulong, bool> playerInBattle = new Dictionary<ulong, bool>(); // Tracks if a player is in a battle
    private static int playerCount = 0;

    // NetworkVariable to store player number (synced across the network)
    public NetworkVariable<int> PlayerNumber = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            // Assign a unique player number when the player spawns
            playerCount++;
            PlayerNumber.Value = playerCount;

            // Store the player number and initialize battle status
            playerMapping[OwnerClientId] = PlayerNumber.Value;
            playerInBattle[OwnerClientId] = false; // Initialize battle status to false

            Debug.Log($"Assigned Player {PlayerNumber.Value} to client {OwnerClientId}");

            // Notify the client of their player number
            NotifyPlayerNumberClientRpc(PlayerNumber.Value, OwnerClientId);
        }
    }

    [ClientRpc]
    private void NotifyPlayerNumberClientRpc(int playerNumber, ulong clientId)
    {
        Debug.Log($"Client {clientId} is Player {playerNumber}");
        GameObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId).gameObject;
        playerObject.name = $"Player{playerNumber}";
    }

    // Method to get player number by clientId
    public static int GetPlayerNumberByClientId(ulong clientId)
    {
        if (playerMapping.ContainsKey(clientId))
        {
            return playerMapping[clientId];
        }
        return -1; // Return an invalid number if not found
    }

    // Method to check if a player is in a battle
    public static bool IsPlayerInBattle(ulong clientId)
    {
        if (playerInBattle.ContainsKey(clientId))
        {
            return playerInBattle[clientId];
        }
        return false; // Return false if player not found
    }

    // Method to set a player's battle status (call this on the server)
    public static void SetPlayerBattleStatus(ulong clientId, bool inBattle)
    {
        if (playerInBattle.ContainsKey(clientId))
        {
            playerInBattle[clientId] = inBattle;
        }
        else
        {
            Debug.LogWarning($"Player {clientId} not found in battle status dictionary.");
        }
    }
}