using DingoSystem;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Linq;
using SimpleJSON;
using System.IO;
using UnityEditor.PackageManager;
using System.Collections;

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
    public static GameObject GetBattlePrefab(ulong clientId)
    {
        if (!_battlePrefabInstances.TryGetValue(clientId, out var prefab))
        {
            prefab = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/BattlePrefab"));
            _battlePrefabInstances[clientId] = prefab;
        }
        return prefab;
    }

    public static void CleanupBattle(ulong clientId)
    {
        if (_battlePrefabInstances.TryGetValue(clientId, out var prefab))
        {
            GameObject.Destroy(prefab);
            _battlePrefabInstances.Remove(clientId);
        }
    }
    public static void StartBattle(ulong clientId, int dingoListInt, Vector3 spawnPosition)
    {
        BattleStarter.Instance.SetBattleUIVisibilityClientRPC(true, clientId);
        List<DingoID> dingoList = DingoDatabase.GetDingoList(dingoListInt);
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

        // Instantiate battle prefab for this client (MODIFIED)
        GameObject battlePrefab = Resources.Load<GameObject>("Prefabs/BattlePrefab");
        if (battlePrefab != null)
        {
            GameObject newPrefab = GameObject.Instantiate(battlePrefab);
            newPrefab.transform.position = spawnPosition;
            _battlePrefabInstances[clientId] = newPrefab; // Store in dictionary
            Debug.Log($"Battle prefab instantiated at position: {spawnPosition} for client {clientId}");
        }
        else
        {
            Debug.LogError("Battle prefab not found in Resources/Prefabs!");
            return;
        }

        // Get prefab from dictionary (MODIFIED)
        GameObject battleInstance = GetBattlePrefab(clientId);
        Vector2 position = battleInstance.transform.position;

        // Update the camera follow target for this client
        BattleStarter.Instance.CameraPositionClientRPC(position, clientId);

        // Cache the Dingo list for this client
        cachedList = dingoList;

        // Rest of the function remains the same...
        NetworkDingo playerDingo1 = DingoLoader.LoadPrefabWithStats(0);
        BattleStarter.Instance.StoreClientSlot(clientId, 0);
        NetworkDingo playerDingo2 = null;

        List<NetworkDingo> playerDingos = new List<NetworkDingo> { playerDingo1 };
        if (playerDingo2 != null)
        {
            playerDingos.Add(playerDingo2);
        }

        NetworkDingo opponentDingo1 = DingoLoader.LoadRandomDingoFromList(dingoList);
        NetworkDingo opponentDingo2 = DingoLoader.LoadRandomDingoFromList(dingoList);

        if (opponentDingo1 == null || opponentDingo2 == null)
        {
            Debug.LogError("Failed to load opponent Dingos.");
            return;
        }

        opponentDingo1.isFlipped.Value = true;
        opponentDingo2.isFlipped.Value = true;

        NetworkDingo[] opponentDingos = { opponentDingo1, opponentDingo2 };

        BattleStarter.Instance.AssignMoveButtonsClientRPC(clientId);
        AssignDingos(clientId, playerDingos.ToArray(), opponentDingos);
        SetBattlePositions(clientId);

        Debug.Log($"Battle started successfully for client {clientId}!");
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

        // Clean up existing Dingo in this slot
        if (BattleSlots[clientId].ContainsKey(slotNumber))
        {
            NetworkDingo existingDingo = BattleSlots[clientId][slotNumber].Dingo;
            if (existingDingo != null && existingDingo != dingo)
            {
                existingDingo.gameObject.SetActive(false);
                if (existingDingo.GetComponent<NetworkObject>().IsSpawned)
                {
                    existingDingo.GetComponent<NetworkObject>().Despawn();
                }
            }
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
    public static void AssignMoveButtons(NetworkDingo networkDingo)
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
        AssignMoveToButton(moveButton1, networkDingo, moves[0]);
        AssignMoveToButton(moveButton2, networkDingo, moves[1]);
        AssignMoveToButton(moveButton3, networkDingo, moves[2]);
        AssignMoveToButton(moveButton4, networkDingo, moves[3]);

        Debug.Log("Assigned moves to buttons and updated text.");
    }
    private static void AssignMoveToButton(GameObject buttonObj, NetworkDingo dingo, DingoMove move)
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
        button.onClick.AddListener(() => UseMove(dingo, move)); // Assign new move function
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
    // In BattleHandler.cs
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

    private static Vector3 BezierCurve(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        return Vector3.Lerp(Vector3.Lerp(start, control, t), Vector3.Lerp(control, end, t), t);
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
            Debug.LogError($"Failed to load Dingo from file {file}");
            return;
        }

        Debug.Log($"{clientId} is switching to {newDingo.name.Value} in battle slot {battleSlot} (loaded from save slot {saveSlot})");

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
    public static NetworkDingo GetDingoForClientId(ulong clientId)
    {
        Debug.Log($"GetDingoForClientId: Searching for Client {clientId}.");

        // Check if the client is Player 2 and not the host
        if (playerToHostMap.ContainsKey(clientId))
        {
            clientId = GetHostForPlayer2(clientId); // Use the host's clientId if Player 2
            Debug.Log($"Client {clientId} is Player 2. Using host clientId.");
        }

        // Proceed to find the Dingo for the (host or Player 2)
        if (!BattleSlots.TryGetValue(clientId, out var slotDictionary))
        {
            Debug.LogError($"GetDingoForClientId: Client {clientId} NOT FOUND in BattleSlots.");
            return null;
        }

        Debug.Log($"GetDingoForClientId: Found battle entry for Client {clientId}.");

        foreach (var slot in slotDictionary)
        {
            Debug.Log($"Checking Slot {slot.Key}: {slot.Value.Dingo}");

            if (slot.Value.Dingo != null)
            {
                Debug.Log($"GetDingoForClientId: Returning Dingo in Slot {slot.Key}.");
                return slot.Value.Dingo;
            }
        }

        Debug.LogWarning($"GetDingoForClientId: No Dingo found for Client {clientId}.");
        return null;
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
    public static void JoinBattleAsPlayer2(ulong clientId, string filePath)
    {
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
        NetworkDingo player2Dingo = DingoLoader.LoadNetworkDingoFromFileToReceive(filePath, 0);
        BattleStarter.Instance.StoreClientSlot(clientId, 0);
        if (player2Dingo == null)
        {
            Debug.LogError("Failed to load Player 2's Dingo from the file.");
            return;
        }

        // Assign Player 2 to Player 1’s battle slots
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

        // Sorting battle slots based on speed
        var orderedSlots = BattleSlots[clientId].Values
            .Where(slot => slot.Dingo != null)
            .OrderByDescending(slot => DingoDatabase.GetDingoByID(slot.Dingo.id.Value).Speed)
            .ToList();

        // Start processing turns in sequence
        BattleStarter.Instance.StartCoroutine(ProcessBattleTurns(clientId, orderedSlots));
    }
    private static IEnumerator ProcessBattleTurns(ulong clientId, List<BattleSlot> orderedSlots)
    {
        foreach (var battleSlot in orderedSlots)
        {
            NetworkDingo selectedDingo = battleSlot.Dingo;
            if (selectedDingo == null) continue;

            Debug.Log($"Processing battle slot: {battleSlot.SlotIndex} for Dingo: {selectedDingo.name.Value}");

            // Handle AI turns (Enemy's move)
            if (battleSlot == BattleSlots[clientId][2] || battleSlot == BattleSlots[clientId][3])
            {
                yield return BattleStarter.Instance.StartCoroutine(HandleAITurn(clientId, selectedDingo, battleSlot));
            }
            else // Player's turn
            {
                yield return BattleStarter.Instance.StartCoroutine(HandlePlayerTurn(clientId, selectedDingo));
            }
        }

        ResetSelections(clientId);
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

        // AI picks a random move
        int chosenMoveId = GetRandomMoveId(selectedDingo);
        DingoMove move = DingoDatabase.GetMoveByID(chosenMoveId, DingoDatabase.GetDingoByID(selectedDingo.id.Value));

        if (move == null)
        {
            Debug.LogError($"Move with ID {chosenMoveId} not found.");
            yield break;
        }

        Debug.Log($"{selectedDingo.name.Value} (Enemy) uses {move.Name} on {target.name.Value}!");

        // Play animation and wait for it to complete
        //yield return BattleStarter.Instance.StartCoroutine(PlayMoveAnimation(selectedDingo, move));

        ApplyMove(clientId, selectedDingo, target, move);
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
    private static IEnumerator HandlePlayerTurn(ulong clientId, NetworkDingo selectedDingo)
    {
        // Check for catch attempt first
        if (selectedDingo.battleMoveId.Value == -2)
        {
            Debug.Log($"{selectedDingo.name.Value} attempting to catch opponent!");

            NetworkDingo targetDingo = GetOpponentDingo(clientId, selectedDingo.battleTargetId.Value);
            if (targetDingo == null) yield break;

            // Play catch animation and wait for it to complete


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

        // Play animation and wait for it to complete
        //yield return BattleStarter.Instance.StartCoroutine(PlayMoveAnimation(selectedDingo, move));

        ApplyMoveBasedOnTarget(clientId, selectedDingo, move);
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
    private static void TriggerDingoAnimation(NetworkDingo selectedDingo, DingoMove move)
    {
        // Trigger animation based on the move
        Debug.Log($"Triggering {selectedDingo.name.Value}'s animation for move: {move.Name}");

        // Example: Use Unity's Animator to play animations
        Animator animator = selectedDingo.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(move.Name);
        }
        else
        {
            Debug.LogError("Animator component not found on Dingo.");
        }
    }
    private static void ApplyMove(ulong clientId, NetworkDingo attacker, NetworkDingo target, DingoMove move)
    {
        if (target == null)
        {
            Debug.LogError($"{attacker.name.Value} tried to attack a null target.");
            return;
        }

        int attackStat = attacker.attack.Value;
        int defenseStat = target.defense.Value;
        int baseDamage = move.Power;

        float damageMultiplier = 1f + ((float)(attackStat - defenseStat) / Mathf.Max(1, defenseStat));
        damageMultiplier = Mathf.Max(damageMultiplier, 0.01f);

        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * damageMultiplier));

        target.hp.Value -= finalDamage;

        Debug.Log($"{attacker.name.Value} used {move.Name} on {target.name.Value} for {finalDamage} damage!");

        if (target.hp.Value <= 0)
        {
            HandleDingoFaint(clientId, target);
        }
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
    private static void UseMove(NetworkDingo networkDingo, DingoMove move)
    {
        if (networkDingo == null || move == null)
        {
            Debug.LogError("UseMove: Invalid Dingo or Move!");
            return;
        }

        Debug.Log($"{networkDingo.name.Value} selected move {move.Name}!");

        // Set the move ID
        networkDingo.battleMoveId.Value = move.MoveID;

        // Get the host client ID for this battle
        ulong hostClientId = GetHostClientIdForDingo(networkDingo);

        // Get the correct battle prefab
        AssignTargetButtons(hostClientId, networkDingo);

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
    public static void RegisterClientInBattle(ulong battleHostClientId, NetworkDingo[] dingos)
    {
        if (!BattleSlots.ContainsKey(battleHostClientId))
        {
            Debug.LogError($"RegisterClientInBattle: No battle exists for host {battleHostClientId}.");
            return;
        }

        foreach (var dingo in dingos)
        {
            ulong dingoClientId = dingo.OwnerClientId;

            if (!BattleSlots.ContainsKey(dingoClientId))
            {
                BattleSlots[dingoClientId] = new Dictionary<int, BattleSlot>();
            }

            // Ensure we're not reassigning an already placed Dingo
            if (BattleSlots[dingoClientId].Values.Any(slot => slot.Dingo == dingo))
            {
                Debug.Log($"Dingo {dingo.name.Value} is already assigned to a slot.");
                continue;
            }

            // Find the first available slot within expected range (0-1 for players)
            int availableSlot = Enumerable.Range(0, 2).FirstOrDefault(slot => !BattleSlots[dingoClientId].ContainsKey(slot) || BattleSlots[dingoClientId][slot].Dingo == null);

            BattleSlots[dingoClientId][availableSlot] = new BattleSlot(dingo, availableSlot, true);
            dingo.slotNumber.Value = availableSlot;

            Debug.Log($"RegisterClientInBattle: Assigned {dingo.name.Value} to slot {availableSlot} under Client {dingoClientId}.");
        }
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
        }
    }
    private static void SetBattlePositions(ulong clientId)
    {
        if (!_battlePrefabInstances.TryGetValue(clientId, out var battlePrefabInstance) || battlePrefabInstance == null)
        {
            Debug.LogError("Battle prefab instance is null for client: " + clientId);
            return;
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

                    // Now, tell the client where to move to (if client, send RPC)
                    if (NetworkManager.Singleton.IsServer)
                    {
                        Vector3 position = playerPosition1.position;
                        // Send the position to the client
                        BattleStarter.Instance.BattlePositionClientRPC(position, clientId);
                    }
                    else
                    {
                        Debug.LogError("Movement script not found on the player.");
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
        BattleStarter.Instance.BattlePositionClientRPC(playerPosition2.position, joiningClientId);
        Debug.Log($"Position the camera at {battlePrefab.transform.position}");

        // Position the camera
        BattleStarter.Instance.CameraPositionClientRPC(battlePrefab.transform.position, joiningClientId);

        // Position Player 2's Dingo if it exists
        if (BattleSlots.ContainsKey(hostClientId) &&
            BattleSlots[hostClientId].ContainsKey(1) &&
            BattleSlots[hostClientId][1].Dingo != null)
        {
            BattleSlots[hostClientId][1].Dingo.transform.position = playerSlot2.position;
            Debug.Log($"Set Player 2 Dingo position in battle hosted by {hostClientId}");
        }

        Debug.Log($"Joined player positions set for client {joiningClientId} in battle {hostClientId}");
    }
    public static void HandleDingoFaint(ulong clientId, NetworkDingo dingo)
    {
        Debug.Log($"{dingo.name.Value} has fainted!");
        dingo.gameObject.SetActive(false);

        // Determine the actual player client ID (host or Player 2)
        ulong playerClientId = clientId;
        bool isPlayer2 = false;
        bool isPlayerDingo = false;

        if (BattleSlots.ContainsKey(clientId) && BattleSlots[clientId].ContainsKey(0) &&
            BattleSlots[clientId][0].Dingo == dingo)
        {
            Debug.Log($"{dingo.name.Value} has fainted!1234541354354321");

            isPlayerDingo = true;
        }
        // Check if this is Player 2's Dingo (slot 1)
        else if (BattleSlots.ContainsKey(clientId) && BattleSlots[clientId].ContainsKey(1) &&
                 BattleSlots[clientId][1].Dingo == dingo)
        {
            isPlayerDingo = true;
            // Get Player 2's actual client ID
            ulong? player2Id = GetPlayer2FromHost(clientId);
            if (player2Id.HasValue)
            {
                playerClientId = player2Id.Value;
            }
        }

        // Only save if this is a player's Dingo
        if (isPlayerDingo)
        {
            RequestSaveDingo(playerClientId, dingo, false);
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
                    Debug.Log($"{(isPlayer2 ? "Player 2" : "Player 1")}'s Dingo in slot {slotNumber} has fainted.");

                    // Check if all player Dingos have fainted
                    bool allPlayerDingosFainted = true;
                    foreach (var playerSlot in BattleSlots[clientId])
                    {
                        if ((playerSlot.Key == 0 || playerSlot.Key == 1) &&
                            playerSlot.Value.Dingo != null &&
                            playerSlot.Value.Dingo.hp.Value > 0)
                        {
                            allPlayerDingosFainted = false;
                            break;
                        }
                    }

                    if (allPlayerDingosFainted)
                    {
                        Debug.Log($"All {(isPlayer2 ? "Player 2" : "Player 1")}'s Dingos have fainted! Ending battle...");
                        EndBattle(clientId);
                        return;
                    }

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

                    // Check if all opponent Dingos are defeated
                    bool opponentsRemain = BattleSlots[clientId].Values
                        .Any(slot => !slot.IsPlayer && slot.Dingo.hp.Value > 0);

                    if (!opponentsRemain)
                    {
                        Debug.Log("All opponents defeated! Ending battle...");
                        EndBattle(clientId);
                        return;
                    }

                    // Randomly spawn a new opponent
                    if (UnityEngine.Random.value < 0.5f)
                    {
                        SpawnNewOpponent(clientId, dingo, cachedList);
                    }
                    return;
                }
            }
        }
    }
    public static void RequestSaveDingo(ulong clientId, NetworkDingo dingo, bool wild)
    {
        if (BattleStarter.Instance != null && dingo != null)
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

    private static void EndBattle(ulong hostClientId)
    {
        Debug.Log($"Ending battle hosted by client {hostClientId}...");

        // Clean up battle slots
        if (BattleSlots.ContainsKey(hostClientId))
        {
            foreach (var slot in BattleSlots[hostClientId])
            {
                if (slot.Value.Dingo != null)
                {
                    slot.Value.Dingo.gameObject.SetActive(false);
                }
            }
            BattleSlots.Remove(hostClientId);
        }

        // Clean up the battle prefab for this host
        if (_battlePrefabInstances.TryGetValue(hostClientId, out var prefab))
        {
            GameObject.Destroy(prefab);
            _battlePrefabInstances.Remove(hostClientId);
            Debug.Log($"Destroyed battle prefab for host {hostClientId}");
        }
        BattleStarter.Instance.SetBattleUIVisibilityClientRPC(false, hostClientId);

        // Clean up Player 2 association if exists
        var player2ClientId = GetPlayer2FromHost(hostClientId);
        if (player2ClientId.HasValue)
        {
            int playerNumber = PlayerManager.GetPlayerNumberByClientId(player2ClientId.Value);
            BattleStarter.Instance.SetPlayerPhysicsStateClientRPC(false, player2ClientId.Value, playerNumber);

            BattleStarter.Instance.SetBattleUIVisibilityClientRPC(false, player2ClientId.Value);
            playerToHostMap.Remove(player2ClientId.Value);
            Debug.Log($"Removed Player2 {player2ClientId.Value} from battle hosted by {hostClientId}");
        }

        // Restore positions for all participants
        RestorePlayerPositionAndCamera(hostClientId);
        if (player2ClientId.HasValue)
        {
            RestorePlayerPositionAndCamera(player2ClientId.Value);
        }
        Debug.Log($"Battle ended for host {hostClientId}");
    }
    private static void RestorePlayerPositionAndCamera(ulong clientId)
    {
        BattleStarter.Instance.ReturnCameraPositionClientRPC(clientId);
        ulong? player2Id = GetPlayer2FromHost(clientId);
        Vector3 originalPosition = playerStartPositions[clientId];
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
        // Check if the player's start position exists
        if (playerStartPositions.ContainsKey(clientId))
        {
            BattleStarter.Instance.BattlePositionClientRPC(originalPosition, clientId);

            // Remove player's position record
            playerStartPositions.Remove(clientId);


        }
        if (player2Id.HasValue) // Ensure it's not null before passing
        {
            BattleStarter.Instance.ReturnCameraPositionClientRPC(player2Id.Value);
            originalPosition.y = originalPosition.y - 1.5f;
            BattleStarter.Instance.BattlePositionClientRPC(originalPosition, player2Id.Value);

        }


        // Check if the player's start position exists
        BattleStarter.Instance.SetBattleUIVisibilityClientRPC(false, clientId);

        // Call BattleStarter's EndBattle method
        if (BattleStarter.Instance != null)
        {
            BattleStarter.Instance.EndBattle(clientId);
        }
    }
    public static void SaveNetworkDingo(NetworkDingo networkDingo)
    {
        int slotIndex = 1; // Initialize slotIndex to 1

        // Load existing Dingos data if it exists
        JSONArray jsonDingos;
        string filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
        if (File.Exists(filePath))
        {
            string existingData = File.ReadAllText(filePath);
            jsonDingos = JSON.Parse(existingData) as JSONArray;
            if (jsonDingos == null)
            {
                // If parsing fails, create a new JSONArray
                Debug.LogWarning("Failed to parse existing Dingos data. Creating a new JSONArray.");
                jsonDingos = new JSONArray();
            }
            else
            {
                // Iterate over existing Dingos to find the highest slot index
                foreach (JSONNode dingoData in jsonDingos)
                {
                    JSONObject dingoObj = dingoData.AsObject;
                    if (dingoObj.HasKey("ID"))
                    {
                        int id = dingoObj["ID"].AsInt;
                        slotIndex = Mathf.Max(slotIndex, id); // Update slotIndex if higher ID found
                    }
                }
                // Increment slotIndex by 1 to get the next available slot
                slotIndex++;
            }
        }
        else
        {
            // If file doesn't exist, create a new JSONArray
            Debug.LogWarning("Dingos data file not found. Creating a new JSONArray.");
            jsonDingos = new JSONArray();
        }

        // Debug information
        Debug.Log("Loaded existing Dingos data from file: " + filePath);
        Debug.Log("Number of existing Dingos: " + jsonDingos.Count);

        // Create JSON object for the new Dingo
        JSONObject jsonDingo = new JSONObject();

        // Debug information
        Debug.Log("New NetworkDingo being saved with ID: " + slotIndex);

        // Add properties of the NetworkDingo to the JSON object
        jsonDingo.Add("ID", slotIndex);
        jsonDingo.Add("DingoID", networkDingo.id.Value);
        jsonDingo.Add("Name", networkDingo.name.Value.ToString());
        jsonDingo.Add("Type", networkDingo.type.Value.ToString());
        jsonDingo.Add("Description", DingoDatabase.GetDingoDescriptionByID(networkDingo.id.Value));
        jsonDingo.Add("CurrentHealth", networkDingo.hp.Value);
        jsonDingo.Add("ATK", networkDingo.attack.Value);
        jsonDingo.Add("DEF", networkDingo.defense.Value);
        jsonDingo.Add("SPD", networkDingo.speed.Value);
        jsonDingo.Add("Sprite", networkDingo.spritePath.Value.ToString());
        jsonDingo.Add("MaxHealth", networkDingo.maxHP.Value);
        jsonDingo.Add("XP", networkDingo.xp.Value);
        jsonDingo.Add("MaxXP", networkDingo.maxXP.Value);
        jsonDingo.Add("Level", networkDingo.level.Value);
        jsonDingo.Add("Move1ID", networkDingo.move1.Value);
        jsonDingo.Add("Move2ID", networkDingo.move2.Value);
        jsonDingo.Add("Move3ID", networkDingo.move3.Value);
        jsonDingo.Add("Move4ID", networkDingo.move4.Value);

        // Add the current Dingo object to the JSON array
        jsonDingos.Add(jsonDingo);

        // Debug information
        Debug.Log("NetworkDingo added to JSON array.");

        // Convert the JSON array to a string
        string jsonString = jsonDingos.ToString();

        // Write the JSON data to the file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.Write(jsonString);
        }

        Debug.Log("NetworkDingo data saved to: " + filePath);
    }
    public static void SaveSlotPlayerDingoData()
    {
        
    }

}
