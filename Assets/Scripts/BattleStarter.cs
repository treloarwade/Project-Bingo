using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using DingoSystem;
using UnityEngine.UI;

public class BattleStarter : NetworkBehaviour
{
    private static HashSet<Vector3> activeBattleSpots = new HashSet<Vector3>();
    private static Dictionary<ulong, Vector3> clientBattleSpots = new Dictionary<ulong, Vector3>();
    public static BattleStarter Instance { get; private set; }

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple BattleStarter instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void RequestStartBattle(ulong clientId, int dingoList, Vector3 triggerPosition)
    {
        Debug.Log($"[BattleStarter] RequestStartBattle called by client {clientId} at position {triggerPosition}.");

        if (IsServer)
        {
            Debug.Log("[BattleStarter] Running on server, handling directly.");
            HandleStartBattleServerRPC(clientId, dingoList, triggerPosition);
        }
        else
        {
            Debug.Log("[BattleStarter] Running on client, sending ServerRpc.");
            RequestStartBattleServerRpc(clientId, dingoList, triggerPosition);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestStartBattleServerRpc(ulong clientId, int dingoList, Vector3 triggerPosition)
    {
        Debug.Log($"[Server] Received battle start request from client {clientId} at position {triggerPosition}.");
        HandleStartBattleServerRPC(clientId, dingoList, triggerPosition);
    }

    // Server-side method to handle starting a battle
    [ServerRpc]
    private void HandleStartBattleServerRPC(ulong clientId, int dingoList, Vector3 triggerPosition)
    {
        // Find the closest battle spot
        GameObject closestSpot = FindClosestBattleSpot(triggerPosition);
        if (closestSpot == null)
        {
            Debug.LogError("[Server] No available battle spots found.");
            return;
        }

        // Check if the battle spot is available
        if (!IsBattleSpotAvailable(closestSpot.transform.position))
        {
            // Battle spot is occupied, so Player 2 is joining an existing battle
            Debug.Log($"[Server] Battle spot is occupied. Client {clientId} is joining as Player 2.");

            // Request the client's save file path
            string filePath = DingoLoader.LoadPlayerDingoFromFileToSend();
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogError($"[Server] Failed to get the save file path for client {clientId}.");
                return;
            }
            NetworkDingo networkDingo = DingoLoader.LoadPrefabWithStats(0);
            // Join the battle as Player 2
            BattleHandler.JoinBattleAsPlayer2(clientId, filePath);
            AssignMoveButtonsClientRPC(clientId);
            return;
        }

        // Reserve the battle spot for Player 1
        ReserveBattleSpot(closestSpot.transform.position);

        // Store the client-battle spot association
        clientBattleSpots[clientId] = closestSpot.transform.position;

        // Start the battle for Player 1
        BattleHandler.StartBattle(clientId, dingoList, closestSpot.transform.position);

        Debug.Log($"[Server] Battle started successfully for client {clientId}!");
    }
    [ClientRpc]
    public void AssignMoveButtonsClientRPC(ulong clientId)
    {
        if(!(clientId == NetworkManager.Singleton.LocalClientId)) { return; }
        NetworkDingo networkDingo = DingoLoader.LoadPrefabWithStats(0);

        //BattleHandler.AssignMoveButtons(networkDingo);
        NewAssignMoveButtons(networkDingo);
    }
    public void NewAssignMoveButtons(NetworkDingo networkDingo)
    {
        if (networkDingo == null)
        {
            Debug.LogError("AssignMoveButtons: No active network Dingo found!");
            return;
        }

        // Find move buttons in the scene
        GameObject moveButton1 = GameObject.Find("MoveButton1");
        GameObject moveButton2 = GameObject.Find("MoveButton2");
        GameObject moveButton3 = GameObject.Find("MoveButton3");
        GameObject moveButton4 = GameObject.Find("MoveButton4");

        if (moveButton1 == null || moveButton2 == null || moveButton3 == null || moveButton4 == null)
        {
            Debug.LogError("AssignMoveButtons: One or more move buttons not found in the scene!");
            return;
        }

        // Load Dingo's moves
        DingoMove[] moves = DingoLoader.LoadDingoMovesByID(networkDingo.id.Value);
        if (moves == null || moves.Length < 4)
        {
            Debug.LogError("AssignMoveButtons: Failed to load moves for Dingo.");
            return;
        }

        // Assign move names and functions to buttons
        NewAssignMoveToButton(moveButton1, networkDingo, moves[0]);
        NewAssignMoveToButton(moveButton2, networkDingo, moves[1]);
        NewAssignMoveToButton(moveButton3, networkDingo, moves[2]);
        NewAssignMoveToButton(moveButton4, networkDingo, moves[3]);

        Debug.Log("Assigned moves to buttons and updated text.");
    }
    private void NewAssignMoveToButton(GameObject buttonObj, NetworkDingo dingo, DingoMove move)
    {
        Button button = buttonObj.GetComponent<Button>();
        Text buttonText = buttonObj.GetComponentInChildren<Text>(); // Get button text

        if (button == null || buttonText == null)
        {
            Debug.LogError($"AssignMoveToButton: Button or Text component missing on {buttonObj.name}.");
            return;
        }

        buttonText.text = move.Name; // Set button text to move name
        button.onClick.RemoveAllListeners(); // Clear previous listeners
        button.onClick.AddListener(() => SelectMoveServerRpc(NetworkManager.Singleton.LocalClientId, move.MoveID)); // Assign new move function
        NewAssignTargetButtons();
    }
    [ServerRpc(RequireOwnership = false)]
    public void SelectMoveServerRpc(ulong clientId, int moveId, ServerRpcParams rpcParams = default)
    {
        // Handle the move selection on the host
        Debug.Log($"Client {clientId} selected move {moveId}");
        // You can now update the game state or perform other actions based on the move selection

    }
    public void NewAssignTargetButtons()
    {


        // Find target buttons
        GameObject targetButton1 = GameObject.Find("Target1");
        GameObject targetButton2 = GameObject.Find("Target2");
        GameObject targetButton3 = GameObject.Find("Target3");
        if (targetButton1 == null || targetButton2 == null || targetButton3 == null)
        {
            Debug.LogError("AssignTargetButtons: One or more target buttons not found!");
            return;
        }
        NewAssignTargetToButton(targetButton1, 0); // First opponent
        NewAssignTargetToButton(targetButton2, 1); // Second opponent
        NewAssignTargetToButton(targetButton3, 2); // Both opponents (AOE attack)

        Debug.Log("Target selection buttons assigned.");
    }
    private void NewAssignTargetToButton(GameObject buttonObj, int targetId)
    {
        Button button = buttonObj.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError($"AssignTargetToButton: Button component missing on {buttonObj.name}.");
            return;
        }
        buttonObj.SetActive(true);
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SelectTargetServerRpc(NetworkManager.Singleton.LocalClientId, targetId));
    }


    [ServerRpc(RequireOwnership = false)]
    public void SelectTargetServerRpc(ulong clientId, int targetId, ServerRpcParams rpcParams = default)
    {
        // Handle the target selection on the host
        Debug.Log($"Client {clientId} selected target {targetId}");
        // You can now update the game state or perform other actions based on the target selection
    }
    // Server-side method to end a battle
    public void EndBattle(ulong clientId)
    {
        if (clientBattleSpots.TryGetValue(clientId, out Vector3 battleSpotPosition))
        {
            // Release the battle spot
            ReleaseBattleSpot(battleSpotPosition);

            // Remove the client-battle spot association
            clientBattleSpots.Remove(clientId);

            Debug.Log($"[Server] Battle ended for client {clientId}. Battle spot at {battleSpotPosition} is now available.");
        }
        else
        {
            Debug.LogError($"[Server] No battle spot found for client {clientId}.");
        }
    }

    // Helper method to check if a battle spot is available
    private static bool IsBattleSpotAvailable(Vector3 position)
    {
        return !activeBattleSpots.Contains(position);
    }

    // Helper method to reserve a battle spot
    private static void ReserveBattleSpot(Vector3 position)
    {
        activeBattleSpots.Add(position);
    }

    // Helper method to release a battle spot
    private static void ReleaseBattleSpot(Vector3 position)
    {
        activeBattleSpots.Remove(position);
    }

    // Helper method to find the closest battle spot
    private static GameObject FindClosestBattleSpot(Vector3 position)
    {
        GameObject[] battleSpots = GameObject.FindGameObjectsWithTag("BattleSpot");
        if (battleSpots.Length == 0)
        {
            Debug.LogError("[Server] No BattleSpots found in the scene.");
            return null;
        }

        GameObject closestSpot = null;
        float minDistance = float.MaxValue;

        foreach (GameObject spot in battleSpots)
        {
            float distance = Vector3.Distance(spot.transform.position, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestSpot = spot;
            }
        }

        return closestSpot;
    }
}