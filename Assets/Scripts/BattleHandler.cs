using DingoSystem;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Linq;
using SimpleJSON;
using System.IO;
using System.Collections;
using System;
using Random = UnityEngine.Random;
using Unity.Netcode.Components;
using UnityEditor;
using UnityEditor.PackageManager;


public static class BattleHandler
{
    public static Dictionary<ulong, Dictionary<int, BattleSlot>> BattleSlots = new Dictionary<ulong, Dictionary<int, BattleSlot>>();
    private static Dictionary<ulong, ulong> playerToHostMap = new Dictionary<ulong, ulong>();
    private static Dictionary<ulong, GameObject> _battlePrefabInstances = new Dictionary<ulong, GameObject>();
    private static List<DingoID> cachedList;
    private static Dictionary<ulong, Vector3> playerStartPositions = new Dictionary<ulong, Vector3>();
    private static Dictionary<ulong, bool> waitingForSwitch = new Dictionary<ulong, bool>();
    private static Dictionary<ulong, float> switchTimer = new Dictionary<ulong, float>();
    private static Dictionary<ulong, HashSet<ulong>> ongoingCatches = new Dictionary<ulong, HashSet<ulong>>();
    public static Dictionary<ulong, Dictionary<int, StatusEffect>> statusEffects = new Dictionary<ulong, Dictionary<int, StatusEffect>>();
    public static Dictionary<ulong, EnvironmentEffect> environmentEffects = new Dictionary<ulong, EnvironmentEffect>();
    public static Dictionary<ulong, NetworkTrainer> _trainerInstances = new Dictionary<ulong, NetworkTrainer>();
    private static Dictionary<ulong, bool> _battleOutcomes = new Dictionary<ulong, bool>();


    public static GameObject GetBattlePrefab(ulong clientId)
    {
        if (!_battlePrefabInstances.TryGetValue(clientId, out var prefab))
        {
            prefab = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/BattlePrefab"));
            _battlePrefabInstances[clientId] = prefab;
        }
        return prefab;
    }
    public static void StartBattle(ulong clientId, int dingoListInt, Vector3 spawnPosition, string filePath, string agentBingoPath, bool isTrainer, int trainerSprite)
    {
        BattleStarter.Instance.SetBattleUIVisibilityClientRPC(true, clientId);
        List<DingoID> dingoList = new List<DingoID>(DingoDatabase.GetDingoList(dingoListInt));

        // Check if the player is already in a battle
        if (IsPlayerInBattle(clientId))
        {
            Debug.LogError($"Client {clientId} is already in a battle. Cannot start a new battle.");
            return;
        }

        // Store player's original position
        int playerNumber = PlayerManager.GetPlayerNumberByClientId(clientId);
        GameObject playerObject = GameObject.Find($"Player{playerNumber}");

        if (playerObject != null)
        {
            playerStartPositions[clientId] = playerObject.transform.position;
            Movement movement = playerObject.GetComponent<Movement>();
            if (movement != null)
            {
                movement.SetRigidbodyStatic(true);
            }
        }
        else
        {
            Debug.LogError($"Player{playerNumber} not found in the scene!");
            return;
        }
        
        // Instantiate battle prefab for this client
        GameObject battlePrefab = Resources.Load<GameObject>("Prefabs/BattlePrefab");
        if (battlePrefab != null)
        {
            GameObject newPrefab = GameObject.Instantiate(battlePrefab);
            newPrefab.transform.position = spawnPosition;
            _battlePrefabInstances[clientId] = newPrefab;
            Debug.Log($"Battle prefab instantiated at position: {spawnPosition} for client {clientId}");
        }
        else
        {
            Debug.LogError("Battle prefab not found in Resources/Prefabs!");
            return;
        }

        GameObject battleInstance = GetBattlePrefab(clientId);
        Vector2 position = battleInstance.transform.position;

        // Update the camera follow target for this client
        BattleStarter.Instance.CameraPositionClientRPC(position, clientId);

        // Cache the Dingo list for this client
        cachedList = dingoList;
        // Load player's Dingo - modified to find first healthy one
        NetworkDingo playerDingo1 = FindFirstHealthyDingo(clientId, filePath, agentBingoPath);
        if (playerDingo1 == null)
        {
            Debug.LogError("No healthy Dingos available for battle!");
            EndBattle(clientId);
            return;
        }

        NetworkDingo playerDingo2 = null;

        List<NetworkDingo> playerDingos = new List<NetworkDingo> { playerDingo1 };
        if (playerDingo2 != null)
        {
            playerDingos.Add(playerDingo2);
        }
        NetworkDingo opponentDingo1 = null;
        NetworkDingo opponentDingo2 = null;
        if (isTrainer)
        {
            opponentDingo1 = DingoLoader.LoadAndRemoveDingoFromList(cachedList);
            opponentDingo2 = DingoLoader.LoadAndRemoveDingoFromList(cachedList);
        }
        else
        {
            opponentDingo1 = DingoLoader.LoadRandomDingoFromList(cachedList);
            opponentDingo2 = DingoLoader.LoadRandomDingoFromList(cachedList);
        }


        if (opponentDingo1 == null || opponentDingo2 == null)
        {
            Debug.LogError("Failed to load opponent Dingos.");
            return;
        }

        opponentDingo1.isFlipped.Value = true;
        opponentDingo2.isFlipped.Value = true;

        NetworkDingo[] opponentDingos = { opponentDingo1, opponentDingo2 };

        AssignDingos(clientId, playerDingos.ToArray(), opponentDingos);

        SetBattlePositions(clientId, isTrainer, trainerSprite);

        Debug.Log($"Battle started successfully for client {clientId}!");
    }
    public static bool IsTrainerBattle(ulong clientId)
    {
        // First determine if this is Player 2 checking in
        bool isPlayer2 = playerToHostMap.ContainsKey(clientId);
        ulong hostClientId = isPlayer2 ? GetHostForPlayer2(clientId) : clientId;

        return _trainerInstances.ContainsKey(hostClientId);
    }
    // New helper method to find first healthy Dingo
    private static NetworkDingo FindFirstHealthyDingo(ulong clientId, string filePath, string agentBingoPath)
    {
        int dingoCount = DingoLoader.GetPlayerDingoCount(filePath);
        NetworkDingo healthyDingo = null;
        List<NetworkDingo> loadedDingos = new List<NetworkDingo>();

        try
        {
            for (int i = 0; i < dingoCount; i++)
            {
                NetworkDingo dingo = DingoLoader.LoadPrefabWithStats(i);
                if (dingo == null) continue;

                loadedDingos.Add(dingo); // Track all loaded Dingos

                if (dingo.hp.Value > 0)
                {
                    Debug.Log($"Found healthy Dingo at slot {i} with HP: {dingo.hp.Value}");
                    BattleStarter.Instance.AssignMoveButtonsSlotClientRPC(clientId, i);
                    BattleStarter.Instance.StoreClientSlot(clientId, i);

                    healthyDingo = dingo;
                    break; // Found a healthy one, stop searching
                }
                else
                {
                    Debug.Log($"Skipping fainted Dingo at slot {i} with HP: {dingo.hp.Value}");
                }
            }

            // Clean up unused Dingos (all except the healthy one we found)
            foreach (var dingo in loadedDingos)
            {
                if (dingo != healthyDingo)
                {
                    CleanupDingo(dingo);
                }
            }

            // If no healthy player Dingo found, use Agent Bingo as fallback
            if (healthyDingo == null)
            {
                healthyDingo = DingoLoader.LoadNetworkDingoFromFileToReceive(agentBingoPath, 0);
                BattleStarter.Instance.AssignMoveButtonsSlotClientRPC(clientId, -1);
                Debug.Log("No healthy player Dingos found, using Agent Bingo");
            }

            return healthyDingo;
        }
        catch (System.Exception e)
        {
            // Ensure cleanup happens even if something goes wrong
            foreach (var dingo in loadedDingos)
            {
                if (dingo != healthyDingo)
                {
                    CleanupDingo(dingo);
                }
            }
            Debug.LogError($"Error finding healthy Dingo: {e.Message}");
            return DingoLoader.LoadRandomDingoFromList(DingoDatabase.agentBingo);
        }
    }

    private static void CleanupDingo(NetworkDingo dingo)
    {
        if (dingo == null) return;

        // Reset any battle-specific properties
        dingo.battleMoveId.Value = -1;
        dingo.battleTargetId.Value = -1;
        dingo.gameObject.SetActive(false);

        // Proper network cleanup
        if (dingo.TryGetComponent<NetworkObject>(out var netObj))
        {
            if (netObj.IsSpawned)
            {
                netObj.Despawn(true);
            }
        }

        // Destroy the game object
        GameObject.Destroy(dingo.gameObject);
        Debug.Log($"Cleaned up Dingo: {dingo.name.Value}");
    }
    public static void AssignDingos(ulong clientId, NetworkDingo[] playerDingos, NetworkDingo[] opponentDingos)
    {
        // Validate input
        if (playerDingos == null || opponentDingos == null)
        {
            Debug.LogError("Dingo arrays cannot be null.");
            return;
        }

        if (playerDingos.Length < 1 || playerDingos.Length > 2 || opponentDingos.Length != 2)
        {
            Debug.LogError("Invalid number of Dingos for battle.");
            return;
        }

        // Ensure BattleSlots is initialized for the client
        if (!BattleSlots.ContainsKey(clientId))
        {
            BattleSlots[clientId] = new Dictionary<int, BattleSlot>(); // Create new battle slot dictionary for this player
        }

        // Assign the player Dingos
        AssignDingoToSlot(clientId, playerDingos[0], 0, true);
        if (playerDingos.Length > 1)
        {
            AssignDingoToSlot(clientId, playerDingos[1], 1, true);
        }

        // Assign the opponent Dingos
        AssignDingoToSlot(clientId, opponentDingos[0], 2, false);
        AssignDingoToSlot(clientId, opponentDingos[1], 3, false);

        // Flip opponent Dingos
        opponentDingos[0].isFlipped.Value = true;
        opponentDingos[1].isFlipped.Value = true;

        Debug.Log($"All Dingos assigned to battle slots for client {clientId}.");
    }
    public static void ApplyFoodEffects(ulong clientId, NetworkDingo dingo, int foodItemId)
    {
        // Implement food effects based on foodItemId
        // Example:
        switch (foodItemId)
        {
            case 0: // Health food
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                dingo.hp.Value = Mathf.Min(dingo.hp.Value + 100, dingo.maxHP.Value);

                break;
            case 4:
                dingo.hp.Value = Mathf.Min(dingo.hp.Value + 200, dingo.maxHP.Value);

                break;
            case 5:
                dingo.hp.Value = Mathf.Min(dingo.hp.Value + 500, dingo.maxHP.Value);

                break;
            case 6:
                dingo.hp.Value = Mathf.Min(dingo.hp.Value + 999, dingo.maxHP.Value);

                break;
        }
    }
    private static void AssignDingoToSlot(ulong clientId, NetworkDingo dingo, int slotNumber, bool isPlayer)
    {
        if (dingo == null)
        {
            Debug.LogError($"Dingo is null for client {clientId} and slot {slotNumber}");
            return;
        }

        // Initialize slot dictionary if needed
        if (!BattleSlots.ContainsKey(clientId))
        {
            BattleSlots[clientId] = new Dictionary<int, BattleSlot>();
        }


        // Assign the new Dingo
        BattleSlots[clientId][slotNumber] = new BattleSlot(dingo, slotNumber, isPlayer);
        dingo.slotNumber.Value = slotNumber;

        // Spawn and activate the new Dingo
        dingo.gameObject.SetActive(true);
        if (!dingo.GetComponent<NetworkObject>().IsSpawned)
        {
            dingo.GetComponent<NetworkObject>().Spawn();
        }
        Debug.Log($"Assigned {dingo.name.Value} to slot {slotNumber} (Player: {isPlayer})");
    }
    public static void AssignTargetButtons(ulong clientId, NetworkDingo dingo)
    {
        // MODIFIED: Get the battle prefab for this client
        if (!_battlePrefabInstances.TryGetValue(clientId, out var battlePrefab) || battlePrefab == null)
        {
            Debug.LogError($"AssignTargetButtons: No battle prefab found for client {clientId}!");
            return;
        }

        if (dingo == null)
        {
            Debug.LogError("AssignTargetButtons: Dingo is null!");
            return;
        }

        // Find target buttons - MODIFIED to use client-specific prefab
        GameObject targetButton1 = battlePrefab.transform.Find("Opponents/Canvas/TargetButton1")?.gameObject;
        GameObject targetButton2 = battlePrefab.transform.Find("Opponents/Canvas/TargetButton2")?.gameObject;
        GameObject targetButton3 = battlePrefab.transform.Find("Opponents/Canvas/TargetButton3")?.gameObject;

        if (targetButton1 == null || targetButton2 == null || targetButton3 == null)
        {
            Debug.LogError("AssignTargetButtons: One or more target buttons not found!");
            return;
        }

        AssignTargetToButton(targetButton1, 0, dingo); // First opponent
        AssignTargetToButton(targetButton2, 1, dingo); // Second opponent
        AssignTargetToButton(targetButton3, 2, dingo); // Both opponents (AOE attack)

        Debug.Log($"Target buttons assigned for client {clientId}'s battle");
    }
    private static void AssignTargetToButton(GameObject buttonObj, int targetId, NetworkDingo selectedDingo)
    {
        buttonObj.SetActive(true);
        Button button = buttonObj.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError($"AssignTargetToButton: Button component missing on {buttonObj.name}.");
            return;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SelectTarget(targetId, selectedDingo));
    }
    public static NetworkDingo GetPlayerDingo(ulong clientId)
    {
        // First determine if this is Player 2 (joined player)
        bool isPlayer2 = playerToHostMap.ContainsKey(clientId);
        ulong hostClientId = isPlayer2 ? playerToHostMap[clientId] : clientId;

        if (!BattleSlots.ContainsKey(hostClientId))
        {
            Debug.LogError($"No battle slots found for host {hostClientId}");
            return null;
        }

        // Determine the correct slot:
        // - Host (battle starter) gets slot 0
        // - Player 2 (joined player) gets slot 1
        int slotNumber = isPlayer2 ? 1 : 0;

        if (BattleSlots[hostClientId].TryGetValue(slotNumber, out var slot))
        {
            return slot.Dingo;
        }

        Debug.LogError($"No Dingo found in slot {slotNumber} for client {clientId}");
        return null;
    }
    public static NetworkDingo GetOpponentDingo(ulong clientId, int targetId)
    {
        // First determine if this is Player 2 checking in
        bool isPlayer2 = playerToHostMap.ContainsKey(clientId);
        ulong hostClientId = isPlayer2 ? GetHostForPlayer2(clientId) : clientId;

        if (!BattleSlots.ContainsKey(hostClientId))
        {
            Debug.LogError($"No battle slots found for host client {hostClientId}");
            return null;
        }

        // For opponent slots (2 and 3)
        int opponentSlot = targetId == 0 ? 2 : 3;

        if (BattleSlots[hostClientId].ContainsKey(opponentSlot))
        {
            return BattleSlots[hostClientId][opponentSlot].Dingo;
        }

        return null;
    }

    public static void AttemptCatchDingo(ulong clientId, NetworkDingo targetDingo)
    {
        if (targetDingo == null)
        {
            Debug.LogError("AttemptCatchDingo: Target Dingo is null");
            return;
        }

        // Disable the caught Dingo
        targetDingo.gameObject.SetActive(false);

        // Save the caught Dingo
        RequestSaveDingo(clientId, targetDingo, true);

        // Check if this was the last opponent Dingo
        bool opponentsRemain = false;
        ulong hostClientId = clientId;
        if (targetDingo.GetComponent<NetworkObject>().IsSpawned)
        {
            targetDingo.GetComponent<NetworkObject>().Despawn();
        }
        GameObject.Destroy(targetDingo);
        // Handle Player 2 case
        if (playerToHostMap.ContainsKey(clientId))
        {
            hostClientId = GetHostForPlayer2(clientId);
        }

        if (BattleSlots.ContainsKey(hostClientId))
        {
            foreach (var slot in BattleSlots[hostClientId])
            {
                // Check opponent slots (2 and 3)
                if ((slot.Key == 2 || slot.Key == 3) &&
                    slot.Value.Dingo != null &&
                    slot.Value.Dingo.gameObject.activeSelf)
                {
                    opponentsRemain = true;
                    break;
                }
            }
        }

        if (!opponentsRemain)
        {
            Debug.Log("All opponents caught or defeated! Ending battle...");
            EndBattle(hostClientId);
        }
    }
    public static void SwitchAgentBingo(string agentBingoPath, ulong clientId)
    {
        bool isConverted;

        ulong adjustedClientId = GetHostClientIdIfApplicable(clientId, out isConverted);

        // Determine battle slot (0 for host, 1 for joining player)
        int battleSlot = playerToHostMap.ContainsKey(clientId) ? 1 : 0;

        NetworkDingo newDingo = DingoLoader.LoadNetworkDingoFromFileToReceive(agentBingoPath, 0);
        BattleStarter.Instance.AssignMoveButtonsSlotClientRPC(clientId, -1);
        if (newDingo == null)
        {
            Debug.LogError($"Failed to find a healthy Dingo from file");
            return;
        }

        Debug.Log($"{clientId} is switching to {newDingo.name.Value} in battle slot {battleSlot}");

        // Remove the old Dingo if it exists in this battle slot
        if (BattleSlots.ContainsKey(adjustedClientId) &&
            BattleSlots[adjustedClientId].ContainsKey(battleSlot))
        {
            NetworkDingo oldDingo = BattleSlots[adjustedClientId][battleSlot].Dingo;
            if (oldDingo != null)
            {
                // Properly clean up the old Dingo
                oldDingo.gameObject.SetActive(false);
                if (oldDingo.GetComponent<NetworkObject>().IsSpawned)
                {
                    oldDingo.GetComponent<NetworkObject>().Despawn();
                }
            }
        }

        // Assign the new Dingo to the fixed battle slot
        AssignDingoToSlot(adjustedClientId, newDingo, battleSlot, true);

        // Update battle position
        SetBattlePosition(adjustedClientId, battleSlot);

        // Reset move/target selection for the new Dingo
        newDingo.battleMoveId.Value = -1;
        newDingo.battleTargetId.Value = -1;
        Debug.Log($"Successfully switched to {newDingo.name.Value} in fixed battle slot {battleSlot}");
        BattleStarter.Instance.PauseBattle(clientId, false);
    }
    public static void SwitchDingos(int saveSlot, string file, ulong clientId)
    {
        // Validate inputs
        if (string.IsNullOrEmpty(file))
        {
            Debug.LogError("Invalid file path for Dingo data.");
            return;
        }

        // Get the correct client ID (handles host/non-host cases)
        bool isConverted;
        ulong adjustedClientId = GetHostClientIdIfApplicable(clientId, out isConverted);

        // Determine battle slot (0 for host, 1 for joining player)
        int battleSlot = playerToHostMap.ContainsKey(clientId) ? 1 : 0;

        Debug.Log($"Client {clientId} is {(battleSlot == 0 ? "host" : "joining player")} using battle slot {battleSlot}");

        // Load the new Dingo using the save slot
        NetworkDingo newDingo = DingoLoader.LoadNetworkDingoFromFileToReceive(file, saveSlot); 
        if (newDingo == null)
        {
            Debug.LogError($"Failed to find a healthy Dingo from file {file}");
            return;
        }

        Debug.Log($"{clientId} is switching to {newDingo.name.Value} in battle slot {battleSlot}");

        // Remove the old Dingo if it exists in this battle slot
        if (BattleSlots.ContainsKey(adjustedClientId) &&
            BattleSlots[adjustedClientId].ContainsKey(battleSlot))
        {
            NetworkDingo oldDingo = BattleSlots[adjustedClientId][battleSlot].Dingo;
            if (oldDingo != null)
            {
                // Properly clean up the old Dingo
                oldDingo.gameObject.SetActive(false);
                if (oldDingo.GetComponent<NetworkObject>().IsSpawned)
                {
                    oldDingo.GetComponent<NetworkObject>().Despawn();
                }
            }
        }

        // Assign the new Dingo to the fixed battle slot
        AssignDingoToSlot(adjustedClientId, newDingo, battleSlot, false);

        // Update battle position
        SetBattlePosition(adjustedClientId, battleSlot);

        // Reset move/target selection for the new Dingo
        newDingo.battleMoveId.Value = -1;
        newDingo.battleTargetId.Value = -1;
        Debug.Log($"Successfully switched to {newDingo.name.Value} in fixed battle slot {battleSlot}");
        BattleStarter.Instance.PauseBattle(clientId, false);
    }
    public static ulong GetHostClientIdIfApplicable(ulong clientId, out bool isConverted)
    {
        if (playerToHostMap.ContainsKey(clientId))
        {
            isConverted = true;
            return GetHostForPlayer2(clientId); // Convert to host's clientId
        }

        isConverted = false;
        return clientId; // No change needed
    }
    private static void SelectTarget(int targetId, NetworkDingo selectedDingo)
    {
        if (selectedDingo == null)
        {
            Debug.LogError("SelectTarget: No move selected or Dingo is null.");
            return;
        }

        selectedDingo.battleTargetId.Value = targetId; // Set target choice
        Debug.Log($"Target selected: {targetId}");

        // Find the correct client ID for the selected Dingo
        ulong clientId = GetClientIdForDingo(selectedDingo);

        if (clientId == ulong.MaxValue)
        {
            Debug.LogError("SelectTarget: Could not determine client ID for selected Dingo.");
            return;
        }

        CheckIfPlayersReady(clientId);
    }
    private static ulong GetClientIdForDingo(NetworkDingo dingo)
    {
        foreach (var entry in BattleSlots)
        {
            ulong clientId = entry.Key; // Get clientId from dictionary
            foreach (var slot in entry.Value)
            {
                if (slot.Value.Dingo == dingo)
                {
                    return clientId; // Return the matching client ID
                }
            }
        }
        return ulong.MaxValue; // Return an invalid ID if not found
    }
    public static void CheckIfPlayersReady(ulong clientId)
    {

        // First determine if this is Player 2 checking in
        bool isPlayer2 = playerToHostMap.ContainsKey(clientId);
        ulong hostClientId = isPlayer2 ? GetHostForPlayer2(clientId) : clientId;

        // Check if there are any ongoing catch attempts in this battle
        if (IsCatchInProgress(hostClientId))
        {
            Debug.Log($"Waiting for catch attempts to complete in battle {hostClientId}...");
            return;
        }
        if (!BattleSlots.ContainsKey(hostClientId))
        {
            Debug.LogError($"CheckIfPlayersReady: No battle slots found for host {hostClientId}.");
            return;
        }

        // Check if Player 2 exists in this battle
        bool player2Exists = GetPlayer2FromHost(hostClientId).HasValue;
        Debug.Log($"Checking readiness - Host: {hostClientId}, Player2 exists: {player2Exists}");

        // Check Player 1 (host) readiness
        bool isPlayer1Ready = IsPlayerReady(BattleSlots[hostClientId], 0);
        Debug.Log($"Player 1 (host) readiness: {isPlayer1Ready}");

        // Check Player 2 readiness if they exist
        bool isPlayer2Ready = false;
        if (player2Exists)
        {
            isPlayer2Ready = IsPlayerReady(BattleSlots[hostClientId], 1);
            Debug.Log($"Player 2 readiness: {isPlayer2Ready}");
        }

        // Determine if we should proceed
        bool shouldProceed = isPlayer1Ready && (!player2Exists || isPlayer2Ready);

        Debug.Log($"Readiness check result - Proceed: {shouldProceed} " +
                 $"(P1: {isPlayer1Ready}, P2: {(player2Exists ? isPlayer2Ready.ToString() : "N/A")})");

        if (shouldProceed)
        {
            Debug.Log("All required players ready - proceeding with battle");
            ProceedWithBattle(hostClientId);
        }
        else
        {
            Debug.Log("Waiting for players to be ready...");
        }
    }
    private static bool IsPlayerReady(Dictionary<int, BattleSlot> battleSlots, int slotIndex)
    {
        if (!battleSlots.ContainsKey(slotIndex))
        {
            Debug.Log($"Slot {slotIndex} doesn't exist");
            return false;
        }

        var slot = battleSlots[slotIndex];
        if (slot.Dingo == null)
        {
            Debug.Log($"Slot {slotIndex} has no Dingo");
            return false;
        }

        bool moveReady = slot.Dingo.battleMoveId.Value != -1;
        bool targetReady = slot.Dingo.battleTargetId.Value != -1;

        Debug.Log($"Slot {slotIndex} ready check - Move: {moveReady}, Target: {targetReady}");

        return moveReady && targetReady;
    }
    public static void JoinBattleAsPlayer2(ulong clientId, string filePath, string agentBingoPath)
    {
        Debug.Log("Successfully joined as Player 2");
        // Ensure at least one battle exists
        if (BattleSlots.Count == 0)
        {
            Debug.LogError("No battle is currently in progress. Player 2 cannot join.");
            return;
        }
        // Find a battle that does not yet have a Player 2
        ulong? existingClientId = null;

        foreach (var battle in BattleSlots)
        {
            // Check if slot 1 is empty, meaning there's an available spot for Player 2
            if (!battle.Value.ContainsKey(1) || battle.Value[1].Dingo == null)
            {
                existingClientId = battle.Key; // Found an open battle
                break;
            }
        }

        // If no battle was found with an open Player 2 slot, return an error
        if (existingClientId == null)
        {
            Debug.LogError($"No available battles for Player 2 (client {clientId}) to join.");
            return;
        }

        ulong battleHostClientId = existingClientId.Value; // Convert nullable ulong to actual ulong

        // Ensure Player 2 isn't already in the battle
        if (BattleSlots.ContainsKey(clientId))
        {
            Debug.LogError($"Client {clientId} is already assigned to a battle.");
            return;
        }

        // Validate the file path
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("Invalid file path for Player 2's Dingo data.");
            return;
        }

        // Load Player 2's Dingo
        NetworkDingo player2Dingo = FindFirstHealthyDingo(clientId, filePath, agentBingoPath);
        BattleStarter.Instance.StoreClientSlot(clientId, 0);
        if (player2Dingo == null)
        {
            Debug.LogError("Failed to load Player 2's Dingo from the file.");
            return;
        }

        // Assign Player 2 to Player 1?s battle slots
        if (!BattleSlots.ContainsKey(battleHostClientId))
        {
            Debug.LogError($"Player 1's client ID {battleHostClientId} not found in BattleSlots.");
            return;
        }

        // Assign Player 2's Dingo to slot 1 under Player 1's client ID
        AssignDingoToSlot(battleHostClientId, player2Dingo, 1, true);

        // Log the assignment
        Debug.Log($"Player 2's Dingo assigned to slot 1 for battle hosted by client {battleHostClientId}: {player2Dingo}");

        // Set Player 2's position in the battle
        SetJoinedPlayerPositions(battleHostClientId, clientId);
        JoinBattleAndAddHostAsPlayer2(clientId, battleHostClientId);
        BattleStarter.Instance.SetBattleUIVisibilityClientRPC(true, clientId);
        int playerNumber = PlayerManager.GetPlayerNumberByClientId(clientId);
        BattleStarter.Instance.SetPlayerPhysicsStateClientRPC(true, clientId, playerNumber);
        Debug.Log($"Player 2 (client {clientId}) has joined the battle hosted by client {battleHostClientId}!");
    }
    public static void SpawnDingoInSlot(ulong clientId, int battleSlotIndex, NetworkDingo playerDingo)
    {
        // Assuming BattleSlots is a dictionary that maps clientId to individual slot dictionaries
        if (BattleSlots.ContainsKey(clientId))
        {
            var clientSlots = BattleSlots[clientId]; // Get the slots for the specific client

            if (clientSlots.ContainsKey(battleSlotIndex))
            {
                BattleSlot targetSlot = clientSlots[battleSlotIndex];
                if (targetSlot != null)
                {
                    // Assign the Dingo to the requested battle slot
                    targetSlot.Dingo = playerDingo;
                    Debug.Log($"Dingo spawned for client {clientId} in slot {battleSlotIndex}: {playerDingo.name.Value}");
                }
                else
                {
                    Debug.LogError($"Battle slot {battleSlotIndex} for client {clientId} is null!");
                }
            }
            else
            {
                Debug.LogError($"Battle slot index {battleSlotIndex} for client {clientId} not found!");
            }
        }
        else
        {
            Debug.LogError($"Client ID {clientId} not found in BattleSlots!");
        }
    }
    public static void SetPlayer2HostAssociation(ulong player2ClientId, ulong hostClientId)
    {
        if (!playerToHostMap.ContainsKey(player2ClientId))
        {
            playerToHostMap.Add(player2ClientId, hostClientId);
        }
        else
        {
            // Update the association if necessary
            playerToHostMap[player2ClientId] = hostClientId;
        }
    }
    public static ulong GetHostForPlayer2(ulong player2ClientId)
    {
        if (playerToHostMap.ContainsKey(player2ClientId))
        {
            return playerToHostMap[player2ClientId];
        }
        else
        {
            Debug.LogError($"No host found for Player 2 with clientId {player2ClientId}.");
            return ulong.MaxValue; // Indicating no host is found.
        }
    }
    public static ulong? GetPlayer2FromHost(ulong hostClientId)
    {
        // Search through the dictionary to find the player2 associated with the hostClientId
        foreach (var entry in playerToHostMap)
        {
            if (entry.Value == hostClientId)
            {
                // Return the player2ClientId associated with the hostClientId
                return entry.Key;
            }
        }

        // If no association is found, return null
        return null;
    }
    public static void JoinBattleAndAddHostAsPlayer2(ulong player2ClientId, ulong hostClientId)
    {
        // When Player 2 joins, we associate them with the host.
        SetPlayer2HostAssociation(player2ClientId, hostClientId);

        // Additional logic for joining the battle (e.g., assigning BattleSlot, loading Dingo, etc.)
        // You can use `hostClientId` when needed later to reference the host.

        Debug.Log($"Player 2 with clientId {player2ClientId} has joined the battle hosted by {hostClientId}.");
    }

    public static void ProceedWithBattle(ulong clientId)
    {
        Debug.Log($"Proceeding with battle logic for client {clientId}...");

        if (IsCatchInProgress(clientId))
        {
            Debug.Log("Battle progression paused - catch attempt in progress");
            return;
        }

        if (!BattleSlots.ContainsKey(clientId))
        {
            Debug.LogError($"No battle slots found for client {clientId}!");
            return;
        }

        // Get turn order based on speed
        var turnOrder = GetTurnOrder(clientId);

        // Start processing turns
        BattleStarter.Instance.StartCoroutine(ProcessBattleTurns(clientId, turnOrder));
    }
    private static IEnumerator PlayStatusEffectAnimation(NetworkDingo target, string statusName)
    {
        if (target == null)
        {
            Debug.LogError("Cannot play status animation - Target is null");
            yield break;
        }

        SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();
        Vector3 originalPosition = target.transform.localPosition;
        Vector3 originalScale = target.transform.localScale;
        Quaternion originalRotation = target.transform.localRotation;
        Color originalColor = renderer != null ? renderer.color : Color.white;

        try
        {
            switch (statusName.ToLower())
            {
                case "goo":
                    yield return GooAnimation(target.transform);
                    break;
                // Add more status effects as needed
                default:
                    // Default animation for unknown status effects
                    yield return new WaitForSeconds(0.5f);
                    break;
            }
        }
        finally
        {
            if (renderer != null)
            {
                renderer.color = originalColor;
            }
            target.transform.localPosition = originalPosition;
            target.transform.localScale = originalScale;
            target.transform.localRotation = originalRotation;
        }
    }
    public static IEnumerator GooAnimation(Transform target)
    {
        GameObject goo = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GooPrefab"), target.position, Quaternion.identity);
        goo.transform.SetParent(target); // Optional: parent to target for positioning

        Vector3 originalScale = goo.transform.localScale;
        Vector3 squishedScale = new Vector3(originalScale.x, 0.7f * originalScale.y, originalScale.z);

        float duration = 0.2f;
        float elapsed = 0f;

        // Squish
        while (elapsed < duration)
        {
            goo.transform.localScale = Vector3.Lerp(originalScale, squishedScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        goo.transform.localScale = squishedScale;

        // Unsquish
        elapsed = 0f;
        while (elapsed < duration)
        {
            goo.transform.localScale = Vector3.Lerp(squishedScale, originalScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        goo.transform.localScale = originalScale;

        GameObject.Destroy(goo, 0.5f); // Clean up
    }
    public static IEnumerator LocalAnimationFunction(ulong clientID, NetworkDingo attacker0, NetworkDingo target2, string movename, int animationtype)
    {

        switch (animationtype)
        {
            case 0:
                if (attacker0 == null)
                {
                    Debug.LogError("Cannot play animation - Attacker is null");
                    yield break;
                }
                yield return PlayMoveAnimation(attacker0, target2, movename);
                break;
            case 1:
                yield return PlayStatusEffectAnimation(target2, movename);
                break;
            case 2:
                yield return new WaitForSeconds(0.5f);
                break;
            default:
                yield return new WaitForSeconds(0.5f);
                break;
        }

    }
    private static IEnumerator PlayMoveAnimation(NetworkDingo attacker, NetworkDingo target, string move)
    {
        if (attacker == null)
        {
            Debug.LogError("Cannot play animation - Attacker is null");
            yield return new WaitForSeconds(0.5f);
            yield break;
        }
        Unsync(attacker);
        SpriteRenderer renderer = attacker.GetComponent<SpriteRenderer>();
        Vector3 originalPosition = attacker.transform.localPosition;
        Vector3 originalScale = attacker.transform.localScale;
        Quaternion originalRotation = attacker.transform.localRotation;

        try
        {
            switch (move)
            {
                case "Squishy Frenzy":
                    if (target != null)
                    {
                        yield return SquishyFrenzyAnimation(attacker, target);
                    }
                    break;

                // Other move cases...

                default:
                    yield return new WaitForSeconds(0.5f);
                    break;
            }
        }
        finally
        {
            attacker.transform.localPosition = originalPosition;
            attacker.transform.localScale = originalScale;
            attacker.transform.localRotation = originalRotation;
            if (renderer != null) renderer.color = Color.white;
        }
        Resync(attacker);
    }
    private static void Unsync(NetworkDingo dingo)
    {
        if (NetworkManager.Singleton.IsHost) return;
        NetworkTransform netObj = dingo.GetComponent<NetworkTransform>();
        netObj.enabled = false;
    }
    private static void Resync(NetworkDingo dingo)
    {
        if (NetworkManager.Singleton.IsHost) return;
        NetworkTransform netObj = dingo.GetComponent<NetworkTransform>();
        netObj.enabled = true;
    }
    private static IEnumerator SquishyFrenzyAnimation(NetworkDingo attacker, NetworkDingo target)
    {
        if (attacker == null || target == null) yield break;

        const float moveSpeed = 1.15f; // Faster than default for snappier animation
        Vector3 originalPosition = attacker.transform.position;
        Vector3 originalScale = attacker.transform.localScale;
        Quaternion originalRotation = attacker.transform.rotation;
        try
        {
            // 1. Calculate key positions relative to combatants
            Vector3 targetPosition = target.transform.position;
            Vector3 attackDirection = (targetPosition - originalPosition).normalized;

            // Calculate bounce points as percentages between combatants
            Vector3 bouncePoint1 = originalPosition + (targetPosition - originalPosition) * 0.3f + Vector3.up * 2f;
            Vector3 bouncePoint2 = originalPosition + (targetPosition - originalPosition) * 0.7f + Vector3.up * 3f;

            // 2. Initial squish preparation
            Vector3 squishScale = new Vector3(1.3f, 0.4f, 1f);
            yield return LerpTransform(attacker.transform,
                originalScale, squishScale,
                originalPosition, originalPosition,
                originalRotation, moveSpeed * 0.8f);

            // 3. Launch toward first bounce point
            Vector3 launchScale = new Vector3(0.8f, 1.2f, 1f);
            yield return LerpTransform(attacker.transform,
                squishScale, launchScale,
                originalPosition, bouncePoint1,
                Quaternion.Euler(0, 0, -25f), moveSpeed * 1.2f);

            // 4. Bounce toward second point
            yield return LerpTransform(attacker.transform,
                launchScale, squishScale,
                bouncePoint1, bouncePoint2,
                Quaternion.Euler(0, 0, 15f), moveSpeed * 1f);

            // 5. Crashing down onto target
            Vector3 impactScale = new Vector3(1.5f, 0.3f, 1f);
            yield return LerpTransform(attacker.transform,
                squishScale, impactScale,
                bouncePoint2, targetPosition,
                Quaternion.Euler(0, 0, -45f), moveSpeed * 0.8f);

            // 6. Quick bounce back
            Vector3 reboundPos = originalPosition + (originalPosition - targetPosition).normalized * 0.5f;
            yield return LerpTransform(attacker.transform,
                impactScale, originalScale,
                targetPosition, reboundPos,
                Quaternion.Euler(0, 0, 10f), moveSpeed * 0.5f);

            // 7. Final return home with small bounce
            yield return LerpTransform(attacker.transform,
                originalScale, originalScale * 1.1f,
                reboundPos, originalPosition,
                originalRotation, moveSpeed * 0.3f);

            yield return LerpScale(attacker.transform, originalScale * 1.1f, originalScale, moveSpeed * 0.2f);
        }
        finally
        {
            // Ensure reset even if interrupted
            attacker.transform.position = originalPosition;
            attacker.transform.localScale = originalScale;
            attacker.transform.rotation = originalRotation;
        }
    }
    private static IEnumerator LerpTransform(Transform target, Vector3 fromScale, Vector3 toScale,
                                       Vector3 fromPos, Vector3 toPos,
                                       Quaternion toRot, float duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            target.localScale = Vector3.Lerp(fromScale, toScale, t);
            target.localPosition = Vector3.Lerp(fromPos, toPos, t);
            target.localRotation = Quaternion.Lerp(target.localRotation, toRot, t);
            yield return null;
        }
    }

    private static IEnumerator LerpScale(Transform target, Vector3 fromScale, Vector3 toScale, float duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            target.localScale = Vector3.Lerp(fromScale, toScale, t);
            yield return null;
        }
    }

    private static IEnumerator LerpRotation(Transform target, Quaternion fromRot, Quaternion toRot, float duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            target.localRotation = Quaternion.Slerp(fromRot, toRot, t);
            yield return null;
        }
    }

    // Bezier curve helper
    private static Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }
    private static List<BattleSlot> GetTurnOrder(ulong clientId)
    {
        if (!BattleSlots.ContainsKey(clientId))
        {
            Debug.LogError($"No battle slots found for client {clientId}!");
            return new List<BattleSlot>();
        }

        // Get all active participants with their speed and selected move
        var participants = new List<(BattleSlot slot, int priority, int speed)>();

        foreach (var slot in BattleSlots[clientId].Values)
        {
            if (slot.Dingo == null || slot.Dingo.hp.Value <= 0)
                continue;

            // Get move priority (default to 0 if no move selected)
            int movePriority = 0;
            if (slot.Dingo.battleMoveId.Value != -1)
            {
                var move = DingoDatabase.GetMoveByID(
                    slot.Dingo.battleMoveId.Value,
                    DingoDatabase.GetDingoByID(slot.Dingo.id.Value)
                );
                movePriority = move?.Priority ?? 0;
            }

            // Get modified speed from BattleSlot (which accounts for speed modifiers and status effects)
            int modifiedSpeed = slot.GetModifiedSpeed();
            Debug.Log($"[GetTurnOrder] Slot: {slot}, MovePriority: {movePriority}, ModifiedSpeed: {modifiedSpeed}!");

            participants.Add((slot, movePriority, modifiedSpeed));
        }

        // Sort by:
        // 1. Move priority (higher priority goes first)
        // 2. Modified speed (higher speed goes first if priorities are equal)
        // 3. Random if both priority and speed are equal
        return participants
            .OrderByDescending(p => p.priority)
            .ThenByDescending(p => p.speed)
            .ThenBy(_ => Random.value)
            .Select(p => p.slot)
            .ToList();
    }
    public static void AddStatus(ulong clientId, int slotIndex, string status, int duration = -1)
    {
        if (!BattleSlots.TryGetValue(clientId, out var slots) || !slots.TryGetValue(slotIndex, out var slot))
            return;

        status = status.ToLower();

        // Check if status already exists (by name)
        if (slot.StatusEffects.Any(se => se.Name == status))
            return;

        // Create appropriate status effect
        StatusEffect effect = status switch
        {
            "goo" => StatusEffect.CreateGooEffect(duration > 0 ? duration : 3),
            "sadlook" => StatusEffect.CreateSadLookEffect(duration > 0 ? duration : 3),
            "throwball" => StatusEffect.CreateSadLookEffect(duration > 0 ? duration : 3),
            "marketanalysis" => StatusEffect.CreateMarketAnalysisEffect(),
            _ => new StatusEffect(999, duration, status) // Default fallback
        };

        slot.StatusEffects.Add(effect);
        Debug.Log($"{slot.Dingo.name.Value} gained {status} status");
        BattleStarter.Instance.StartCoroutine(PlayStatusEffectAnimation(slot.Dingo, status));
        BattleStarter.Instance.MoveAnimationClientRPC(clientId, 0, 0, slot.Dingo.NetworkObjectId, 0, status, 1);

    }

    public static void RemoveStatus(ulong clientId, int slotIndex, string status)
    {
        if (BattleSlots.TryGetValue(clientId, out var slots) && slots.TryGetValue(slotIndex, out var slot))
        {
            status = status.ToLower();
            slot.StatusEffects.RemoveAll(se => se.Name == status);
            Debug.Log($"{slot.Dingo?.name.Value ?? "Dingo"} had {status} removed");
        }
    }

    public static bool HasStatus(ulong clientId, int slotIndex, string status)
    {
        if (BattleSlots.TryGetValue(clientId, out var slots) && slots.TryGetValue(slotIndex, out var slot))
        {
            return slot.StatusEffects.Any(se => se.Name == status.ToLower());
        }
        return false;
    }

    private static IEnumerator ProcessBattleTurns(ulong clientId, List<BattleSlot> turnOrder)
    {
        foreach (var battleSlot in turnOrder)
        {
            NetworkDingo currentDingo = battleSlot.Dingo;
            if (currentDingo == null || currentDingo.hp.Value <= 0) continue;

            Debug.Log($"[ProcessBattleTurns] Processing turn for {currentDingo.name.Value}");

            bool skipTurn = ProcessSkipTurnStatusEffects(clientId, battleSlot.SlotIndex);
            if (skipTurn) continue;
            if (!battleSlot.IsPlayer) // AI turn
            {
                yield return BattleStarter.Instance.StartCoroutine(HandleAITurn(clientId, currentDingo, battleSlot));
            }
            else // Player turn
            {
                // For players, we just need to wait for their selection
                while (currentDingo.battleMoveId.Value == -1 || currentDingo.battleTargetId.Value == -1)
                {
                    yield return null;
                }

                yield return BattleStarter.Instance.StartCoroutine(HandlePlayerTurn(clientId, currentDingo));
            }

            // Check if battle should end after each turn
            if (CheckBattleEndCondition(clientId))
                yield break;
        }

        ResetSelections(clientId);
    }

    private static IEnumerator HandleAITurn(ulong clientId, NetworkDingo selectedDingo, BattleSlot battleSlot)
    {
        Debug.Log($"BattleSlot {battleSlot.SlotIndex} is AI turn.");

        List<NetworkDingo> possibleTargets = GetAITargets(clientId);
        if (possibleTargets.Count == 0)
        {
            Debug.LogWarning($"No possible targets found for AI turn.");
            yield break;
        }

        // AI picks a random target
        NetworkDingo target = possibleTargets[UnityEngine.Random.Range(0, possibleTargets.Count)];
        Debug.Log($"AI selected target: {target.name.Value}");
        int chosenMoveId = GetRandomMoveId(selectedDingo);
        DingoMove move = DingoDatabase.GetMoveByID(chosenMoveId, DingoDatabase.GetDingoByID(selectedDingo.id.Value));

        if (move == null)
        {
            Debug.LogError($"Move with ID {chosenMoveId} not found.");
            yield break;
        }

        Debug.Log($"{selectedDingo.name.Value} (Enemy) uses {move.Name} on {target.name.Value}!");
        BattleStarter.Instance.MoveAnimationClientRPC(clientId, selectedDingo.NetworkObjectId, 0, target.NetworkObjectId, 0, move.Name, 0);
        // Play animation and wait for it to complete
        yield return BattleStarter.Instance.StartCoroutine(PlayMoveAnimation(selectedDingo, target, move.Name));

        ApplyMove(clientId, selectedDingo, target, move);
    }

    private static IEnumerator HandlePlayerTurn(ulong clientId, NetworkDingo selectedDingo)
    {
        // Check for catch attempt first
        if (selectedDingo.battleMoveId.Value == -2)
        {
            Debug.Log($"{selectedDingo.name.Value} attempting to catch opponent!");
            NetworkDingo targetDingo = GetOpponentDingo(clientId, selectedDingo.battleTargetId.Value);
            if (targetDingo == null) yield break;

            // Play catch animation and wait
            yield break;
        }

        if (selectedDingo.battleMoveId.Value == -1 || selectedDingo.battleTargetId.Value == -1)
        {
            Debug.LogError($"{selectedDingo.name.Value} has not selected a move or target.");
            yield break;
        }

        DingoMove move = DingoDatabase.GetMoveByID(
            selectedDingo.battleMoveId.Value,
            DingoDatabase.GetDingoByID(selectedDingo.id.Value)
        );

        if (move == null)
        {
            Debug.LogError($"Move with ID {selectedDingo.battleMoveId.Value} not found.");
            yield break;
        }

        Debug.Log($"{selectedDingo.name.Value} is using {move.Name}!");
        NetworkDingo targetDingo2 = GetOpponentDingo(clientId, selectedDingo.battleTargetId.Value);
        BattleStarter.Instance.MoveAnimationClientRPC(clientId, selectedDingo.NetworkObjectId, 0, targetDingo2.NetworkObjectId, 0, move.Name, 0);
        // Play animation and wait for it to complete
        yield return BattleStarter.Instance.StartCoroutine(PlayMoveAnimation(selectedDingo, targetDingo2, move.Name));

        EvaluateMoveEffectsAndPerform(clientId, selectedDingo, move);
    }
    private static void EvaluateMoveEffectsAndPerform(ulong clientId, NetworkDingo selectedDingo, DingoMove move)
    {
        switch (move.Name)
        {
            case "Market Analysis":
                SelfBuff(clientId, selectedDingo, move);
                break;
            case "ATM Withdrawal":
                CalculateCash(clientId, selectedDingo, move);
                break;
            default:
                ApplyMoveBasedOnTarget(clientId, selectedDingo, move);
                break;
        }
    }
    private static void SelfBuff(ulong clientId, NetworkDingo selectedDingo, DingoMove move)
    {
        AddStatus(clientId, selectedDingo.slotNumber.Value, move.StatusEffect);
        SendBattleMessage(clientId, $"{selectedDingo.name.Value} used {move.Name}");
    }
    public static float ProcessMoneyMultiplierStatusEffects(ulong clientId, int slotIndex)
    {
        if (!BattleSlots.TryGetValue(clientId, out var slots) ||
        !slots.TryGetValue(slotIndex, out var slot) ||
        slot.Dingo == null)
            return 1f;

        var dingo = slot.Dingo;
        float multiplier = 1f;
        foreach (var status in slot.StatusEffects.ToList()) // ToList() to avoid modification during iteration
        {
            switch (status.Name)
            {
                case "marketanalysis":
                    multiplier = multiplier * 2;
                    break;
                default:
                    break;
            }
        }
        return multiplier;
    }

    private static void CalculateCash(ulong clientId, NetworkDingo selectedDingo, DingoMove move)
    {
        int attackStat = BattleSlots[clientId][selectedDingo.slotNumber.Value].GetModifiedAttack();
        float multiplier = ProcessMoneyMultiplierStatusEffects(clientId, selectedDingo.slotNumber.Value);
        int cashgained = Mathf.RoundToInt((selectedDingo.level.Value + attackStat) * ProcessMoneyMultiplierStatusEffects(clientId, selectedDingo.slotNumber.Value));
        InventoryManager.Instance.AwardBattleEarningsClientRpc(cashgained, clientId);
        SendBattleMessage(clientId, $"{selectedDingo.name.Value} used {move.Name} and gained ${cashgained} multiplier was {multiplier}");
    }
    public static bool ProcessSkipTurnStatusEffects(ulong clientId, int slotIndex)
    {
        if (!BattleSlots.TryGetValue(clientId, out var slots) ||
            !slots.TryGetValue(slotIndex, out var slot) ||
            slot.Dingo == null)
            return false;

        var dingo = slot.Dingo;
        bool skipTurn = false;

        // Process all status effects from the slot
        foreach (var status in slot.StatusEffects.ToList()) // ToList() to avoid modification during iteration
        {
            BattleStarter.Instance.StartCoroutine(PlayStatusEffectAnimation(dingo, status.Name));
            BattleStarter.Instance.MoveAnimationClientRPC(clientId, 0, 0, dingo.NetworkObjectId, 0, status.Name, 1);
            // Skip turn chance (like paralysis or sleep)
            if (status.SkipTurnChance > 0 && Random.value < status.SkipTurnChance)
            {
                SendBattleMessage(clientId, $"{dingo.name.Value} is affected by {status.Name} and can't move!");
                Debug.Log($"{dingo.name.Value} is affected by {status.Name} and can't move!");
                skipTurn = true;
            }

            // Damage over time
            if (status.DamagePerTurn > 0)
            {
                int damage = Math.Max(1, (int)(dingo.maxHP.Value * (status.DamagePerTurn / 100f)));
                dingo.hp.Value -= damage;
                Debug.Log($"{dingo.name.Value} is hurt by {status.Name}!");
            }

            // Reduce duration and remove if expired
            status.Duration--;
            if (status.Duration <= 0)
            {
                slot.StatusEffects.Remove(status);
                Debug.Log($"{dingo.name.Value} is no longer affected by {status.Name}!");
            }
        }

        return skipTurn;
    }


    private static void ApplyMoveBasedOnTarget(ulong clientId, NetworkDingo selectedDingo, DingoMove move)
    {
        switch (selectedDingo.battleTargetId.Value)
        {
            case 0:
                ApplyMove(clientId, selectedDingo, BattleSlots[clientId][2].Dingo, move);
                break;
            case 1:
                ApplyMove(clientId, selectedDingo, BattleSlots[clientId][3].Dingo, move);
                break;
            case 2:
                ApplyMove(clientId, selectedDingo, BattleSlots[clientId][2].Dingo, move);
                ApplyMove(clientId, selectedDingo, BattleSlots[clientId][3].Dingo, move);
                break;
            default:
                Debug.LogError($"Invalid target ID {selectedDingo.battleTargetId.Value} for {selectedDingo.name}.");
                break;
        }
    }

    private static bool CheckBattleEndCondition(ulong clientId)
    {
        if (!BattleSlots.ContainsKey(clientId))
        {
            Debug.LogError($"No battle slots found for client {clientId}!");
            return false;
        }

        // Check if all player Dingos have fainted
        bool allPlayerDingosFainted = true;
        foreach (var slot in BattleSlots[clientId].Values)
        {
            if (slot.IsPlayer && slot.Dingo != null && slot.Dingo.hp.Value > 0)
            {
                allPlayerDingosFainted = false;
                break;
            }
        }

        // Check if all opponent Dingos have fainted
        bool allOpponentsFainted = true;
        foreach (var slot in BattleSlots[clientId].Values)
        {
            if (!slot.IsPlayer && slot.Dingo != null && slot.Dingo.hp.Value > 0)
            {
                allOpponentsFainted = false;
                break;
            }
        }

        if (allPlayerDingosFainted)
        {
            Debug.Log("All player Dingos have fainted! Ending battle...");

            // First check if player has any healthy Dingos left in their collection
            bool hasHealthyDingos = CheckForHealthyDingos(clientId);

            if (!hasHealthyDingos)
            {
                Debug.Log("Player has no healthy Dingos left in collection! Using Agent Bingo...");
                BattleStarter.Instance.UseAgentBingo();
                return false; // Don't end battle yet
            }
            else
            {
                Debug.Log("Player has healthy Dingos available - forcing switch");
                // The HandleDingoFaint method will handle forcing the player to switch
                return false; // Don't end battle yet
            }
        }

        if (allOpponentsFainted)
        {
            Debug.Log("All opponents defeated! Ending battle...");
            EndBattle(clientId);
            return true;
        }

        return false;
    }

    private static bool CheckForHealthyDingos(ulong clientId)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
        if (!File.Exists(filePath)) return false;

        try
        {
            string jsonData = File.ReadAllText(filePath);
            JSONArray jsonDingos = JSON.Parse(jsonData) as JSONArray;

            if (jsonDingos != null)
            {
                foreach (JSONNode dingoData in jsonDingos)
                {
                    JSONObject dingo = dingoData.AsObject;
                    if (dingo["CurrentHealth"].AsInt > 0)
                    {
                        return true; // Found at least one healthy Dingo
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error checking for healthy Dingos: " + e.Message);
        }

        return false;
    }
    private static void ResetSelections(ulong clientId)
    {
        Debug.Log($"Resetting move/target selections for client {clientId}");

        // Reset host player (slot 0)
        if (BattleSlots[clientId].ContainsKey(0) && BattleSlots[clientId][0].Dingo != null)
        {
            BattleSlots[clientId][0].Dingo.battleMoveId.Value = -1;
            BattleSlots[clientId][0].Dingo.battleTargetId.Value = -1;
            Debug.Log($"Reset selections for host player's Dingo");
        }

        // Reset Player 2 (slot 1) if they exist
        if (BattleSlots[clientId].ContainsKey(1) && BattleSlots[clientId][1].Dingo != null)
        {
            BattleSlots[clientId][1].Dingo.battleMoveId.Value = -1;
            BattleSlots[clientId][1].Dingo.battleTargetId.Value = -1;
            Debug.Log($"Reset selections for Player 2's Dingo");
        }

        // Don't reset opponent selections (slots 2-3) as they're AI-controlled
    }
    private static List<NetworkDingo> GetAITargets(ulong clientId)
    {
        List<NetworkDingo> possibleTargets = new List<NetworkDingo>();

        if (BattleSlots[clientId].ContainsKey(1) && BattleSlots[clientId][1].Dingo != null)
        {
            possibleTargets.Add(BattleSlots[clientId][1].Dingo);
            Debug.Log($"Added Dingo from BattleSlot[1] as possible target.");
        }

        if (BattleSlots[clientId].ContainsKey(0) && BattleSlots[clientId][0].Dingo != null)
        {
            possibleTargets.Add(BattleSlots[clientId][0].Dingo);
            Debug.Log($"Added Dingo from BattleSlot[0] as possible target.");
        }

        return possibleTargets;
    }
    private static int GetRandomMoveId(NetworkDingo selectedDingo)
    {
        int[] moves = { selectedDingo.move1.Value, selectedDingo.move2.Value, selectedDingo.move3.Value, selectedDingo.move4.Value };
        return moves[UnityEngine.Random.Range(0, moves.Length)];
    }
    private static void ApplyMove(ulong clientId, NetworkDingo attacker, NetworkDingo target, DingoMove move)
    {
        if (target == null)
        {
            Debug.LogError($"{attacker.name.Value} tried to attack a null target.");
            return;
        }
        BattleSlot attackerSlot = GetBattleSlotForDingo(clientId, attacker);
        BattleSlot targetSlot = GetBattleSlotForDingo(clientId, target);

        try
        {
            // Convert string types to DingoType enums
            DingoType attackerType = (DingoType)Enum.Parse(typeof(DingoType), attacker.type.Value.ToString());
            DingoType defenderType = (DingoType)Enum.Parse(typeof(DingoType), target.type.Value.ToString());

            // Calculate type effectiveness
            float typeEffectiveness = DingoTypeEffectivenessCalculator.GetEffectiveness(attackerType, defenderType);

            if (typeEffectiveness == 0f)
            {
                Debug.Log($"{move.Name} has no effect on {target.name.Value}!");
                return;
            }

            int attackStat = BattleSlots[clientId][attacker.slotNumber.Value].GetModifiedAttack();
            int defenseStat = BattleSlots[clientId][target.slotNumber.Value].GetModifiedDefense();
            int baseDamage = move.Power;

            // Apply type effectiveness to damage calculation
            float damageMultiplier = 1f + ((float)(attackStat - defenseStat) / Mathf.Max(1, defenseStat));
            damageMultiplier = Mathf.Max(damageMultiplier, 0.01f) * typeEffectiveness;

            int finalDamage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * damageMultiplier));

            target.hp.Value -= finalDamage;

            // Add type effectiveness message
            string effectivenessMessage = typeEffectiveness switch
            {
                > 1f => "It's super effective!",
                < 1f and > 0f => "It's not very effective...",
                0f => "It had no effect!",
                _ => ""
            };
            SendBattleMessage(clientId, $"{attacker.name.Value} used {move.Name} on {target.name.Value} for {finalDamage} damage! {effectivenessMessage}");

            Debug.Log($"{attacker.name.Value} used {move.Name} on {target.name.Value} for {finalDamage} damage! {effectivenessMessage}");
            if (!string.IsNullOrEmpty(move.StatusEffect))
            {
                AddStatus(clientId, targetSlot.SlotIndex, move.StatusEffect);
            }

            if (target.hp.Value <= 0)
            {
                HandleDingoFaint(clientId, target);
            }
        }
        catch (ArgumentException e)
        {
            Debug.LogError($"Error calculating type effectiveness: {e.Message}");
            // Default to neutral effectiveness if there's an error
            int finalDamage = Mathf.Max(1, Mathf.RoundToInt(move.Power * (1f + ((float)(attacker.attack.Value - target.defense.Value) / Mathf.Max(1, target.defense.Value)))));
            target.hp.Value -= finalDamage;
            Debug.Log($"{attacker.name.Value} used {move.Name} on {target.name.Value} for {finalDamage} damage!");
        }
    }
    public static void SendBattleMessage(ulong clientId, string message)
    {
        // First determine if this is Player 2 checking in
        bool isPlayer2 = playerToHostMap.ContainsKey(clientId);
        ulong hostClientId = isPlayer2 ? GetHostForPlayer2(clientId) : clientId;

        // Send to host player
        BattleDialog.SendDialogToClient(hostClientId, message);

        // Check if there's a Player 2 in this battle and send to them too
        ulong? player2Id = GetPlayer2FromHost(hostClientId);
        if (player2Id.HasValue)
        {
            BattleDialog.SendDialogToClient(player2Id.Value, message);
        }

        Debug.Log($"Battle message sent to participants: {message}");
    }
    private static BattleSlot GetBattleSlotForDingo(ulong clientId, NetworkDingo dingo)
    {
        if (!BattleSlots.ContainsKey(clientId)) return null;

        foreach (var slot in BattleSlots[clientId].Values)
        {
            if (slot.Dingo == dingo)
            {
                return slot;
            }
        }
        return null;
    }
    private static void SpawnNewOpponent(ulong clientId, NetworkDingo oldOpponent, List<DingoID> dingoList)
    {
        // Validate BattleSlots and clientId
        if (BattleSlots == null || !BattleSlots.ContainsKey(clientId))
        {
            Debug.LogError($"SpawnNewOpponent: No battle slots found for client {clientId}.");
            return;
        }

        // Validate dingoList
        if (dingoList == null || dingoList.Count == 0)
        {
            Debug.LogError("No available Dingos to spawn!");
            return;
        }

        // Load a new random Dingo
        NetworkDingo newOpponent = DingoLoader.LoadRandomDingoFromList(dingoList);
        if (newOpponent == null)
        {
            Debug.LogError("Failed to load a new opponent Dingo.");
            return;
        }

        // Set opponent properties
        newOpponent.isFlipped.Value = true;

        // Find the correct battle slot for replacement
        bool foundSlot = false;
        foreach (var slot in BattleSlots[clientId])
        {
            if (slot.Value.Dingo == oldOpponent)
            {
                // Assign the new opponent to the slot
                slot.Value.Dingo = newOpponent;

                // Move new opponent into the correct battle position
                Vector3 oldPosition = oldOpponent.transform.position;
                newOpponent.transform.position = oldPosition;

                // Make new opponent visible
                newOpponent.gameObject.SetActive(true);

                Debug.Log($"New opponent {newOpponent.name} has moved into battle slot {slot.Key} for client {clientId}.");
                foundSlot = true;
                break;
            }
        }

        if (!foundSlot)
        {
            Debug.LogError($"Failed to assign the new opponent to a valid battle slot for client {clientId}.");
            Debug.Log($"Old opponent: {oldOpponent?.name}, New opponent: {newOpponent.name}");
            Debug.Log($"BattleSlots for client {clientId}:");
            foreach (var slot in BattleSlots[clientId])
            {
                Debug.Log($"Slot {slot.Key}: Dingo = {slot.Value.Dingo?.name}");
            }
        }
    }
    public static void MarkCatchInProgress(ulong clientId, bool inProgress)
    {
        ulong hostClientId = GetHostClientIdIfApplicable(clientId, out _);

        if (!ongoingCatches.ContainsKey(hostClientId))
        {
            ongoingCatches[hostClientId] = new HashSet<ulong>();
        }

        if (inProgress)
        {
            ongoingCatches[hostClientId].Add(clientId);
        }
        else
        {
            ongoingCatches[hostClientId].Remove(clientId);

            if (ongoingCatches[hostClientId].Count == 0)
            {
                ongoingCatches.Remove(hostClientId);
            }
        }
    }
    public static bool IsCatchInProgress(ulong hostClientId)
    {
        return ongoingCatches.ContainsKey(hostClientId) && ongoingCatches[hostClientId].Count > 0;
    }
    // Helper method to find which battle a Dingo belongs to
    public static ulong GetHostClientIdForDingo(NetworkDingo dingo)
    {
        // First check if this is Player 2's Dingo
        if (playerToHostMap.TryGetValue(dingo.OwnerClientId, out ulong hostId))
        {
            return hostId;
        }

        // Otherwise assume it's the host's Dingo
        return dingo.OwnerClientId;
    }
    private static bool IsPlayerInBattle(ulong clientId)
    {
        // Check if the client is already registered in BattleSlots
        return BattleSlots.ContainsKey(clientId);
    }
    public static NetworkDingo GetPlayer2Dingo(ulong clientId)
    {
        // First, check if Player 2 is the host. If so, switch the clientId to the host for correct retrieval.
        if (playerToHostMap.ContainsKey(clientId))
        {
            ulong hostClientId = GetHostForPlayer2(clientId); // Use the host's clientId if Player 2
            Debug.Log($"Client {clientId} is Player 2. Using host clientId.");

            // Check if the battle slots exist for this host client
            if (BattleSlots.ContainsKey(hostClientId))
            {
                Dictionary<int, BattleSlot> clientBattleSlots = BattleSlots[hostClientId];

                // If Player 2 is the host, look at slot 1 for Player 2's Dingo
                if (clientBattleSlots.ContainsKey(1))
                {
                    BattleSlot player2Slot = clientBattleSlots[1];
                    if (player2Slot != null && player2Slot.Dingo != null)
                    {
                        return player2Slot.Dingo;
                    }
                }

                // If Player 1 is the host, check slot 0 for Player 1's Dingo
                if (clientBattleSlots.ContainsKey(0))
                {
                    BattleSlot player1Slot = clientBattleSlots[0];
                    if (player1Slot != null && player1Slot.Dingo != null)
                    {
                        return player1Slot.Dingo;
                    }
                }
            }
        }
        else
        {
            // If Player 2 is not the host, look directly at slot 0 for their Dingo
            if (BattleSlots.ContainsKey(clientId))
            {
                Dictionary<int, BattleSlot> clientBattleSlots = BattleSlots[clientId];

                // Look for the Dingo in slot 0 (for the non-host player)
                if (clientBattleSlots.ContainsKey(0))
                {
                    BattleSlot playerSlot = clientBattleSlots[0];
                    if (playerSlot != null && playerSlot.Dingo != null)
                    {
                        return playerSlot.Dingo;
                    }
                }
            }
        }

        // If no Dingo was found, log an error
        Debug.LogError($"Player's Dingo not found for client {clientId}.");
        return null;
    }
    public static NetworkDingo GetPlayerNetworkDingo(ulong clientId, int slot)
    {
        // First, check if Player 2 is the host. If so, switch the clientId to the host for correct retrieval.
        if (playerToHostMap.ContainsKey(clientId))
        {
            ulong hostClientId = GetHostForPlayer2(clientId); // Use the host's clientId if Player 2
            Debug.Log($"Client {clientId} is Player 2. Using host clientId.");

            // Check if the battle slots exist for this host client
            if (BattleSlots.ContainsKey(hostClientId))
            {
                Dictionary<int, BattleSlot> clientBattleSlots = BattleSlots[hostClientId];

                // If Player 2 is the host, look at slot 1 for Player 2's Dingo
                if (clientBattleSlots.ContainsKey(slot))
                {
                    BattleSlot player2Slot = clientBattleSlots[slot];
                    if (player2Slot != null && player2Slot.Dingo != null)
                    {
                        return player2Slot.Dingo;
                    }
                }

                // If Player 1 is the host, check slot 0 for Player 1's Dingo
                if (clientBattleSlots.ContainsKey(0))
                {
                    BattleSlot player1Slot = clientBattleSlots[0];
                    if (player1Slot != null && player1Slot.Dingo != null)
                    {
                        return player1Slot.Dingo;
                    }
                }
            }
        }
        else
        {
            // If Player 2 is not the host, look directly at slot 0 for their Dingo
            if (BattleSlots.ContainsKey(clientId))
            {
                Dictionary<int, BattleSlot> clientBattleSlots = BattleSlots[clientId];

                // Look for the Dingo in slot 0 (for the non-host player)
                if (clientBattleSlots.ContainsKey(slot))
                {
                    BattleSlot playerSlot = clientBattleSlots[slot];
                    if (playerSlot != null && playerSlot.Dingo != null)
                    {
                        return playerSlot.Dingo;
                    }
                }
            }
        }

        // If no Dingo was found, log an error
        Debug.LogError($"Player's Dingo not found for client {clientId}.");
        return null;
    }
    private static void SetBattlePosition(ulong clientId, int slotIndex)
    {
        // MODIFIED: Get the battle prefab for this client
        if (!_battlePrefabInstances.TryGetValue(clientId, out var battlePrefab) || battlePrefab == null)
        {
            Debug.LogError($"Battle prefab instance is null for client {clientId}");
            return;
        }

        string[] slotPaths = {
        "Players/Slot1",
        "Players/Slot2",
        "Opponents/Opponent1",
        "Opponents/Opponent2"
        };

        if (slotIndex < 0 || slotIndex >= slotPaths.Length)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return;
        }

        // MODIFIED: Use client-specific prefab
        Transform slotTransform = battlePrefab.transform.Find(slotPaths[slotIndex]);
        if (slotTransform == null)
        {
            Debug.LogError($"Slot path not found: {slotPaths[slotIndex]} in client {clientId}'s battle");
            return;
        }

        if (!BattleSlots.ContainsKey(clientId) || !BattleSlots[clientId].ContainsKey(slotIndex))
        {
            Debug.LogError($"No Dingo found in slot {slotIndex} for client {clientId}");
            return;
        }

        NetworkDingo dingo = BattleSlots[clientId][slotIndex].Dingo;
        if (dingo != null)
        {
            dingo.transform.position = slotTransform.position;
            dingo.transform.rotation = slotTransform.rotation;

            // Flip opponents if needed
            dingo.isFlipped.Value = (slotIndex >= 2); // Simplified flip logic

            // Handle Agent Bingo (ID 0) for both player slots
            if (dingo.id.Value == 0 && (slotIndex == 0 || slotIndex == 1))
            {
                Debug.Log($"Value was {dingo.id.Value}");
                SpriteRenderer dingoRenderer = dingo.GetComponent<SpriteRenderer>();
                if (dingoRenderer != null)
                {
                    dingoRenderer.enabled = false;
                }

                // For slot 0 (host player)
                if (slotIndex == 0)
                {
                    Transform playerPosition1 = battlePrefab.transform.Find("Players/Slot1");
                    if (playerPosition1 != null)
                    {
                        BattleStarter.Instance.BattlePositionClientRPC(playerPosition1.position, clientId);
                    }
                }
                // For slot 1 (player 2)
                else if (slotIndex == 1)
                {
                    Transform playerPosition2 = battlePrefab.transform.Find("Players/Slot2");
                    if (playerPosition2 != null)
                    {
                        // Get player 2's actual client ID
                        ulong? player2Id = GetPlayer2FromHost(clientId);
                        if (player2Id.HasValue)
                        {
                            BattleStarter.Instance.BattlePositionClientRPC(playerPosition2.position, player2Id.Value);
                        }
                    }
                }
            }
            else if (slotIndex == 0 || slotIndex == 1)
            {
                // Regular player Dingo positioning
                Transform playerPosition = battlePrefab.transform.Find(
                    slotIndex == 0 ? "Players/TrainerPosition1" : "Players/TrainerPosition2");

                if (playerPosition != null)
                {
                    ulong targetClientId = clientId;
                    if (slotIndex == 1)
                    {
                        // For player 2, use their actual client ID
                        ulong? player2Id = GetPlayer2FromHost(clientId);
                        if (player2Id.HasValue) targetClientId = player2Id.Value;
                    }

                    BattleStarter.Instance.BattlePositionClientRPC(playerPosition.position, targetClientId);
                }
            }
        }

    }
    private static void SetBattlePositions(ulong clientId, bool isTrainer, int trainerSprite)
    {
        if (!_battlePrefabInstances.TryGetValue(clientId, out var battlePrefabInstance) || battlePrefabInstance == null)
        {
            Debug.LogError("Battle prefab instance is null for client: " + clientId);
            return;
        }
        if (isTrainer)
        {
            NetworkTrainer trainer = DingoLoader.TrainerSprite(trainerSprite);
            if (trainer != null)
            {
                NetworkObject netObj = trainer.GetComponent<NetworkObject>();
                if (netObj != null && !netObj.IsSpawned)
                {
                    netObj.Spawn(true); // Spawn and assign ownership if needed
                }

                _trainerInstances[clientId] = trainer; // Store the trainer instance

                Transform opponentTrainer1 = battlePrefabInstance.transform.Find("Opponents/OpponentTrainerPosition1");
                if (opponentTrainer1 != null)
                {
                    trainer.transform.localPosition = opponentTrainer1.position;
                }
                else
                {
                    Debug.LogError("OpponentTrainerPosition1 not found in battle prefab!");
                }
            }
            else
            {
                Debug.LogError("Failed to load trainer sprite!");
            }
        }

        // Find the battle slots (child objects)
        Transform playerSlot1 = battlePrefabInstance.transform.Find("Players/Slot1");
        Transform playerSlot2 = battlePrefabInstance.transform.Find("Players/Slot2");
        Transform opponentSlot1 = battlePrefabInstance.transform.Find("Opponents/Opponent1");
        Transform opponentSlot2 = battlePrefabInstance.transform.Find("Opponents/Opponent2");
        Transform playerPosition1 = battlePrefabInstance.transform.Find("Players/TrainerPosition1");

        if (playerPosition1 != null)
        {
            int playerNumber = PlayerManager.GetPlayerNumberByClientId(clientId);
            GameObject playerObject = GameObject.Find($"Player{playerNumber}");

            if (playerObject != null)
            {
                Movement playerMovement = playerObject.GetComponent<Movement>();
                if (playerMovement != null)
                {
                    playerMovement.movementEnabled = false;
                    playerObject.transform.position = playerPosition1.position;
                    if (BattleSlots[clientId][0].Dingo.id.Value == 0)
                    {
                        // Disable the Dingo's sprite renderer
                        SpriteRenderer dingoRenderer = BattleSlots[clientId][0].Dingo.GetComponent<SpriteRenderer>();
                        if (dingoRenderer != null)
                        {
                            dingoRenderer.enabled = false;
                        }
                        BattleStarter.Instance.BattlePositionClientRPC(playerSlot1.position, clientId);
                    }
                    else
                    {
                        BattleStarter.Instance.BattlePositionClientRPC(playerPosition1.position, clientId);
                    }
                }
                else
                {
                    Debug.LogError($"Player{playerNumber} not found in the BattlePrefab.");
                }
            }
            else
            {
                Debug.LogError("Player position 1 not found in the BattlePrefab.");
            }

            if (playerSlot1 == null || playerSlot2 == null || opponentSlot1 == null || opponentSlot2 == null)
            {
                Debug.LogError("One or more battle slot transforms not found in BattlePrefab.");
                return;
            }

            // Set the Dingos' positions to the respective slots
            BattleSlots[clientId][0].Dingo.transform.position = playerSlot1.position;
            if (playerSlot2 != null && BattleSlots[clientId].ContainsKey(1) && BattleSlots[clientId][1].Dingo != null)
            {

                BattleSlots[clientId][1].Dingo.transform.position = playerSlot2.position;
            }
            else
            {
                Debug.Log("playerDingo2 is not present, skipping slot assignment.");
            }
            BattleSlots[clientId][2].Dingo.transform.position = opponentSlot1.position;
            BattleSlots[clientId][3].Dingo.transform.position = opponentSlot2.position;

            Debug.Log("Battle positions set successfully.");
        }
    }
    private static void SetJoinedPlayerPositions(ulong hostClientId, ulong joiningClientId)
    {
        // Get the correct battle prefab for this host
        if (!_battlePrefabInstances.TryGetValue(hostClientId, out var battlePrefab) || battlePrefab == null)
        {
            Debug.LogError($"No battle prefab found for host client {hostClientId}");
            return;
        }

        // Find the required transforms
        Transform playerSlot2 = battlePrefab.transform.Find("Players/Slot2");
        Transform playerPosition2 = battlePrefab.transform.Find("Players/TrainerPosition2");

        if (playerPosition2 == null || playerSlot2 == null)
        {
            Debug.LogError($"Required battle positions not found in host {hostClientId}'s battle prefab");
            return;
        }

        // Position the joining player's character
        Debug.Log($"Position the camera at {battlePrefab.transform.position}");

        // Position the camera
        BattleStarter.Instance.CameraPositionClientRPC(battlePrefab.transform.position, joiningClientId);

        // Position Player 2's Dingo if it exists
        if (BattleSlots.ContainsKey(hostClientId) &&
            BattleSlots[hostClientId].ContainsKey(1) &&
            BattleSlots[hostClientId][1].Dingo != null)
        {
            BattleSlots[hostClientId][1].Dingo.transform.position = playerSlot2.position;

            if (BattleSlots[hostClientId][1].Dingo.id.Value == 0)
            {
                BattleStarter.Instance.BattlePositionClientRPC(playerSlot2.position, joiningClientId);
                SpriteRenderer dingoRenderer = BattleSlots[hostClientId][1].Dingo.GetComponent<SpriteRenderer>();
                if (dingoRenderer != null)
                {
                    dingoRenderer.enabled = false;
                }
            }
            else
            {
                BattleStarter.Instance.BattlePositionClientRPC(playerPosition2.position, joiningClientId);
            }
            Debug.Log($"Set Player 2 Dingo position in battle hosted by {hostClientId}");
        }

        Debug.Log($"Joined player positions set for client {joiningClientId} in battle {hostClientId}");
    }
    private static void HandleDingoFaint(ulong clientId, NetworkDingo dingo)
    {
        Debug.Log($"{dingo.name.Value} has fainted!");

        // Reset temp stats before despawning
        foreach (var slot in BattleSlots[clientId].Values)
        {
            if (slot.Dingo == dingo)
            {
                slot.ResetTempStats();
                break;
            }
        }

        if (dingo.TryGetComponent<NetworkObject>(out var netObj) && netObj.IsSpawned)
        {
            netObj.Despawn(true); // Destroy on all clients
        }
        else
        {
            // Fallback if NetworkObject isn't available
            dingo.gameObject.SetActive(false);
        }

        // Find which slot the fainted Dingo was in
        foreach (var slot in BattleSlots[clientId])
        {
            if (slot.Value.Dingo == dingo)
            {
                int slotNumber = slot.Key;

                // Handle player Dingos (slots 0 and 1)
                if (slotNumber == 0 || slotNumber == 1)
                {
                    ulong playerClientId = clientId;
                    bool isPlayer2 = false;

                    // If this is Player 2's Dingo (slot 1), get their actual client ID
                    if (slotNumber == 1)
                    {
                        ulong? player2Id = GetPlayer2FromHost(clientId);
                        if (player2Id.HasValue)
                        {
                            playerClientId = player2Id.Value;
                            isPlayer2 = true;
                        }
                    }
                    RequestSaveDingo(playerClientId, dingo, false);

                    Debug.Log($"{(isPlayer2 ? "Player 2" : "Player 1")}'s Dingo in slot {slotNumber} has fainted.");

                    // Force player to switch
                    waitingForSwitch[playerClientId] = true;
                    switchTimer[playerClientId] = 5f; // 5 second timer to switch

                    // Show switch UI to the correct player
                    BattleStarter.Instance.ShowSwitchUI(playerClientId, slotNumber);

                    // Pause battle for this player
                    BattleStarter.Instance.PauseBattle(playerClientId, true);

                    Debug.Log($"Waiting for {(isPlayer2 ? "Player 2" : "Player 1")} (client {playerClientId}) to switch Dingo in slot {slotNumber}");
                    return;
                }
                // Handle opponent Dingos (existing logic)
                else if (slotNumber == 2 || slotNumber == 3)
                {
                    Debug.Log($"Opponent Dingo in slot {slotNumber} has fainted.");
                    return;
                }
            }
        }
    }
    public static void RequestSaveDingo(ulong clientId, NetworkDingo dingo, bool wild)
    {
        if (BattleStarter.Instance != null && dingo != null)
        {
            if (dingo.id.Value == 0)
            {
                BattleStarter.Instance.RequestSavePlayerServerRPC(
    clientId,
    dingo.id.Value,
    dingo.name.Value,
    dingo.type.Value.ToString(),
    dingo.hp.Value,
    dingo.maxHP.Value,
    dingo.attack.Value,
    dingo.defense.Value,
    dingo.speed.Value,
    dingo.spritePath.Value,
    dingo.xp.Value,
    dingo.maxXP.Value,
    dingo.level.Value,
    dingo.move1.Value,
    dingo.move2.Value,
    dingo.move3.Value,
    dingo.move4.Value
);
            }
            else
            {
                BattleStarter.Instance.RequestSaveDingoServerRPC(
    clientId,
    dingo.id.Value,
    dingo.name.Value,
    dingo.type.Value.ToString(),
    dingo.hp.Value,
    dingo.maxHP.Value,
    dingo.attack.Value,
    dingo.defense.Value,
    dingo.speed.Value,
    dingo.spritePath.Value,
    dingo.xp.Value,
    dingo.maxXP.Value,
    dingo.level.Value,
    dingo.move1.Value,
    dingo.move2.Value,
    dingo.move3.Value,
    dingo.move4.Value,
    wild
);
            }
        }
    }
    public static void SavePlayerDingoData(
        int slotIndex,
        int dingoId,
        string dingoName,
        string dingoType,
        int currentHp,
        int maxHp,
        int atk,
        int def,
        int spd,
        string spritePath,
        int xp,
        int maxXp,
        int level,
        int move1Id,
        int move2Id,
        int move3Id,
        int move4Id
    )
    {
        // If slotIndex is -1, it means we're adding a new Dingo
        if (slotIndex == -1)
        {
            // Get the client's Dingo count from BattleStarter
            ulong clientId = NetworkManager.Singleton.LocalClientId;
            if (BattleStarter.clientDingoCount.TryGetValue(clientId, out int count))
            {
                slotIndex = count; // Use the count as the next available slot
                BattleStarter.clientDingoCount[clientId] = count + 1; // Increment count for next time
            }
            else
            {
                // First Dingo for this client
                slotIndex = 0;
                BattleStarter.clientDingoCount[clientId] = 1; // Set count to 1 for next time
            }
        }
        // Load or create JSON data
        JSONArray jsonDingos;
        string filePath = Path.Combine(Application.persistentDataPath, "dingos.json");

        if (File.Exists(filePath))
        {
            jsonDingos = JSON.Parse(File.ReadAllText(filePath)) as JSONArray ?? new JSONArray();
        }
        else
        {
            jsonDingos = new JSONArray();
        }

        // Create/update Dingo entry
        JSONObject jsonDingo = new JSONObject();
        jsonDingo.Add("ID", slotIndex);
        jsonDingo.Add("DingoID", dingoId);
        jsonDingo.Add("Name", dingoName);
        jsonDingo.Add("Type", dingoType);
        jsonDingo.Add("Description", DingoDatabase.GetDingoDescriptionByID(dingoId));
        jsonDingo.Add("CurrentHealth", currentHp);
        jsonDingo.Add("ATK", atk);
        jsonDingo.Add("DEF", def);
        jsonDingo.Add("SPD", spd);
        jsonDingo.Add("Sprite", spritePath);
        jsonDingo.Add("MaxHealth", maxHp);
        jsonDingo.Add("XP", xp);
        jsonDingo.Add("MaxXP", maxXp);
        jsonDingo.Add("Level", level);
        jsonDingo.Add("Move1ID", move1Id);
        jsonDingo.Add("Move2ID", move2Id);
        jsonDingo.Add("Move3ID", move3Id);
        jsonDingo.Add("Move4ID", move4Id);


        // Update or add slot
        if (jsonDingos.Count > slotIndex)
        {
            jsonDingos[slotIndex] = jsonDingo;
        }
        else
        {
            jsonDingos.Add(jsonDingo);
        }

        // Save file
        File.WriteAllText(filePath, jsonDingos.ToString());
        Debug.Log($"Saved Dingo data to slot {slotIndex}");
    }
    public static void SavePlayerCharacterData(
    int slotIndex,
    int dingoId,
    string dingoName,
    string dingoType,
    int currentHp,
    int maxHp,
    int atk,
    int def,
    int spd,
    string spritePath,
    int xp,
    int maxXp,
    int level,
    int move1Id,
    int move2Id,
    int move3Id,
    int move4Id
)
    {
        // Load or create JSON data
        JSONArray jsonDingos;
        string filePath = Path.Combine(Application.persistentDataPath, "playerinfo.json");

        if (File.Exists(filePath))
        {
            jsonDingos = JSON.Parse(File.ReadAllText(filePath)) as JSONArray ?? new JSONArray();
        }
        else
        {
            jsonDingos = new JSONArray();
        }

        // Create/update Dingo entry
        JSONObject jsonDingo = new JSONObject();
        jsonDingo.Add("ID", slotIndex);
        jsonDingo.Add("DingoID", dingoId);
        jsonDingo.Add("Name", dingoName);
        jsonDingo.Add("Type", dingoType);
        jsonDingo.Add("Description", DingoDatabase.GetDingoDescriptionByID(dingoId));
        jsonDingo.Add("CurrentHealth", currentHp);
        jsonDingo.Add("ATK", atk);
        jsonDingo.Add("DEF", def);
        jsonDingo.Add("SPD", spd);
        jsonDingo.Add("Sprite", spritePath);
        jsonDingo.Add("MaxHealth", maxHp);
        jsonDingo.Add("XP", xp);
        jsonDingo.Add("MaxXP", maxXp);
        jsonDingo.Add("Level", level);
        jsonDingo.Add("Move1ID", move1Id);
        jsonDingo.Add("Move2ID", move2Id);
        jsonDingo.Add("Move3ID", move3Id);
        jsonDingo.Add("Move4ID", move4Id);


        // Update or add slot
        if (jsonDingos.Count > slotIndex)
        {
            jsonDingos[slotIndex] = jsonDingo;
        }
        else
        {
            jsonDingos.Add(jsonDingo);
        }

        // Save file
        File.WriteAllText(filePath, jsonDingos.ToString());
        Debug.Log($"Saved Dingo data to slot {slotIndex}");
    }


    public static void EndBattle(ulong hostClientId)
    {
        bool playerWon = CheckPlayerWinCondition(hostClientId);
        _battleOutcomes[hostClientId] = playerWon;

        // Get trainer info if this was a trainer battle
        int trainerId = -1;
        if (_trainerInstances.TryGetValue(hostClientId, out var trainer2))
        {
            trainerId = trainer2.trainerId.Value;
        }

        // Notify all clients about battle end with trainer info
        BattleStarter.Instance.NotifyBattleEndClientRpc(
            hostClientId,
            playerWon,
            trainerId
        );
        // Clean up all Dingos in this battle
        if (BattleSlots.TryGetValue(hostClientId, out var slots))
        {
            foreach (var slot in slots.Values)
            {
                if (slot.Dingo != null)
                {
                    CleanupDingo(slot.Dingo);
                }
            }
            BattleSlots.Remove(hostClientId);
        }

        // Clean up trainer if exists
        if (_trainerInstances.TryGetValue(hostClientId, out var trainer))
        {
            if (trainer != null)
            {
                if (trainer.TryGetComponent<NetworkObject>(out var netObj) && netObj.IsSpawned)
                {
                    netObj.Despawn(true);
                }
                GameObject.Destroy(trainer.gameObject);
            }
            _trainerInstances.Remove(hostClientId);
        }

        SavePlayerDingosAtBattleEnd(hostClientId);
        CleanupBattlePrefab(hostClientId);
        HandlePlayer2Cleanup(hostClientId);
        RestorePlayerPositions(hostClientId);
    }
    private static bool CheckPlayerWinCondition(ulong hostClientId)
    {
        if (!BattleSlots.ContainsKey(hostClientId)) return false;

        // Check if all opponent Dingos are fainted
        bool allOpponentsFainted = true;
        foreach (var slot in BattleSlots[hostClientId].Values)
        {
            if (!slot.IsPlayer && slot.Dingo != null && slot.Dingo.hp.Value > 0)
            {
                allOpponentsFainted = false;
                break;
            }
        }
        return allOpponentsFainted;
    }

    public static bool GetBattleOutcome(ulong clientId)
    {
        if (_battleOutcomes.TryGetValue(clientId, out bool outcome))
        {
            _battleOutcomes.Remove(clientId); // Clean up after retrieval
            return outcome;
        }
        return false;
    }
    private static void SavePlayerDingosAtBattleEnd(ulong hostClientId)
    {
        // Save host player's Dingo (slot 0)
        if (BattleSlots.TryGetValue(hostClientId, out var hostSlots) &&
            hostSlots.TryGetValue(0, out var hostSlot) &&
            hostSlot.Dingo != null)
        {
            Debug.Log($"Saving host player's Dingo: {hostSlot.Dingo.name.Value}");
            RequestSaveDingo(hostClientId, hostSlot.Dingo, false);
        }

        // Check if Player 2 exists and save their Dingo (slot 1)
        var player2Id = GetPlayer2FromHost(hostClientId);
        if (player2Id.HasValue &&
            hostSlots.TryGetValue(1, out var player2Slot) &&
            player2Slot.Dingo != null)
        {
            Debug.Log($"Saving Player 2's Dingo: {player2Slot.Dingo.name.Value}");
            RequestSaveDingo(player2Id.Value, player2Slot.Dingo, false);
        }
    }

    private static void CleanupBattlePrefab(ulong hostClientId)
    {
        if (_battlePrefabInstances.TryGetValue(hostClientId, out var prefab))
        {
            // Also clean up trainer if it exists
            if (_trainerInstances.TryGetValue(hostClientId, out var trainer))
            {
                if (trainer != null)
                {
                    if (trainer.TryGetComponent<NetworkObject>(out var netObj) && netObj.IsSpawned)
                    {
                        netObj.Despawn(true);
                    }
                    else
                    {
                        GameObject.Destroy(trainer.gameObject);
                    }
                }
                _trainerInstances.Remove(hostClientId);
            }

            GameObject.Destroy(prefab);
            _battlePrefabInstances.Remove(hostClientId);
            Debug.Log($"Destroyed battle prefab and trainer for host {hostClientId}");
        }
    }
    private static void HandlePlayer2Cleanup(ulong hostClientId)
    {
        var player2ClientId = GetPlayer2FromHost(hostClientId);
        if (player2ClientId.HasValue)
        {
            int playerNumber = PlayerManager.GetPlayerNumberByClientId(player2ClientId.Value);
            BattleStarter.Instance.SetPlayerPhysicsStateClientRPC(false, player2ClientId.Value, playerNumber);
            BattleStarter.Instance.SetBattleUIVisibilityClientRPC(false, player2ClientId.Value);
            playerToHostMap.Remove(player2ClientId.Value);
            Debug.Log($"Removed Player2 {player2ClientId.Value} from battle hosted by {hostClientId}");
        }
    }

    private static void RestorePlayerPositions(ulong hostClientId)
    {
        RestorePlayerPositionAndCamera(hostClientId);
        var player2Id = GetPlayer2FromHost(hostClientId);
        if (player2Id.HasValue)
        {
            RestorePlayerPositionAndCamera(player2Id.Value);
        }
        Debug.Log($"Battle ended for host {hostClientId}");
    }
    private static void RestorePlayerPositionAndCamera(ulong clientId)
    {
        BattleStarter.Instance.ReturnCameraPositionClientRPC(clientId);

        ulong? player2Id = GetPlayer2FromHost(clientId);

        // Safely get the player's start position if it exists
        Vector3 originalPosition = Vector3.zero;
        if (playerStartPositions.TryGetValue(clientId, out originalPosition))
        {
            BattleStarter.Instance.BattlePositionClientRPC(originalPosition, clientId);

            // Remove player's position record only if it exists
            playerStartPositions.Remove(clientId);
        }
        else
        {
            Debug.LogWarning($"No start position recorded for client {clientId}");
        }

        int playerNumber = PlayerManager.GetPlayerNumberByClientId(clientId);
        GameObject playerObject = GameObject.Find($"Player{playerNumber}");
        if (playerObject != null)
        {
            Movement movement = playerObject.GetComponent<Movement>();
            if (movement != null)
            {
                movement.SetRigidbodyStatic(false);
            }
        }

        // Handle Player 2 position if exists
        if (player2Id.HasValue)
        {
            BattleStarter.Instance.ReturnCameraPositionClientRPC(player2Id.Value);
            Vector3 player2OriginalPosition = originalPosition;

            player2OriginalPosition.y = player2OriginalPosition.y - 1.5f;
            BattleStarter.Instance.BattlePositionClientRPC(player2OriginalPosition, player2Id.Value);

            // Remove Player 2's position record
            playerStartPositions.Remove(player2Id.Value);

            // Restore physics for Player 2
            int player2Number = PlayerManager.GetPlayerNumberByClientId(player2Id.Value);
            GameObject player2Object = GameObject.Find($"Player{player2Number}");
            if (player2Object != null)
            {
                Movement player2Movement = player2Object.GetComponent<Movement>();
                if (player2Movement != null)
                {
                    player2Movement.SetRigidbodyStatic(false);
                }
            }
        }

        BattleStarter.Instance.SetBattleUIVisibilityClientRPC(false, clientId);

        // Call BattleStarter's EndBattle method
        if (BattleStarter.Instance != null)
        {
            BattleStarter.Instance.EndBattle(clientId);
        }
    }
}