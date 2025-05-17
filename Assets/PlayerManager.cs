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
    // Day/Night synchronization
    public static PlayerManager Instance;
    private Dictionary<ulong, SpriteRenderer> playerSprites = new Dictionary<ulong, SpriteRenderer>();
    private NetworkList<ulong> connectedPlayers;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        connectedPlayers = new NetworkList<ulong>();

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            // Player number assignment
            playerCount++;
            PlayerNumber.Value = playerCount;

            playerMapping[OwnerClientId] = PlayerNumber.Value;
            playerInBattle[OwnerClientId] = false;

            connectedPlayers.Add(OwnerClientId);

            Debug.Log($"Assigned Player {PlayerNumber.Value} to client {OwnerClientId}");
            UpdatePlayerListServer(connectedPlayers);
        }

        // Register player for day/night sync
        RegisterPlayer(OwnerClientId, GetComponent<SpriteRenderer>());
        // Sync immediately with current state
        if (DayAndNight.Instance != null)
        {
            DayAndNight.Instance.SyncPlayer();
        }
    }

    public void RegisterPlayer(ulong clientId, SpriteRenderer spriteRenderer)
    {
        if (!playerSprites.ContainsKey(clientId))
        {
            playerSprites.Add(clientId, spriteRenderer);
        }
    }

    public void UnregisterPlayer(ulong clientId)
    {
        if (playerSprites.ContainsKey(clientId))
        {
            playerSprites.Remove(clientId);
        }
    }
    public void UpdateAllPlayerSprites(Color color)
    {
        foreach (var playerSprite in playerSprites.Values)
        {
            if (playerSprite != null)
            {
                playerSprite.color = color;
            }
        }
    }
    private void UpdatePlayerListServer(NetworkList<ulong> players)
    {
        foreach (ulong clientId in players)
        {
            int player = GetPlayerNumberByClientId(clientId);
            NotifyPlayerNumberClientRpc(player, clientId);
        }
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            connectedPlayers.Remove(OwnerClientId);
        }
        UnregisterPlayer(OwnerClientId);
        base.OnNetworkDespawn();
    }

    [ClientRpc]
    private void NotifyPlayerNumberClientRpc(int playerNumber, ulong clientId)
    {
        Debug.Log($"Client {clientId} is Player {playerNumber}");
        playerMapping[clientId] = playerNumber;
        if (DayAndNight.Instance != null)
        {
            DayAndNight.Instance.SyncPlayer();
        }
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