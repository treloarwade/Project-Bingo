using DingoSystem;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Linq;
using System;
public static class BattleHandler
{
    public static Dictionary<ulong, Dictionary<int, BattleSlot>> BattleSlots = new Dictionary<ulong, Dictionary<int, BattleSlot>>(); 
    private static GameObject battlePrefabInstance;
    private static int selectedMoveId = -1;
    private static Button moveButton1, moveButton2, moveButton3, moveButton4;
    public static Dictionary<ulong, List<NetworkDingo>> BattleClients = new Dictionary<ulong, List<NetworkDingo>>();
    private static List<DingoID> cachedList;
    private static Dictionary<ulong, Vector3> playerStartPositions = new Dictionary<ulong, Vector3>();
    public static void StartBattle(ulong clientId, int dingoListInt, Vector3 spawnPosition)
    {
        List<DingoID> dingoList = DingoDatabase.GetDingoList(dingoListInt);
        // Check if the player is already in a battle
        if (IsPlayerInBattle(clientId))
        {
            Debug.LogError($"Client {clientId} is already in a battle. Cannot start a new battle.");
            return;
        }

        // Store player's original position
        int playerNumber = PlayerManager.GetPlayerNumberByClientId(clientId); // Use the passed clientId
        GameObject playerObject = GameObject.Find($"Player{playerNumber}");
        if (playerObject != null)
        {
            playerStartPositions[clientId] = playerObject.transform.position;
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
            battlePrefabInstance = GameObject.Instantiate(battlePrefab);
            battlePrefabInstance.transform.position = spawnPosition;
            Debug.Log($"Battle prefab instantiated at position: {spawnPosition} for client {clientId}");
        }
        else
        {
            Debug.LogError("Battle prefab not found in Resources/Prefabs!");
            return;
        }

        // Update the camera follow target for this client
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.target = battlePrefabInstance.transform; // Set the camera focus to the battle area
        }
        else
        {
            Debug.LogError("CameraFollow script not found on the main camera!");
        }

        // Cache the Dingo list for this client
        cachedList = dingoList;

        // Load player Dingos from saved data (using slot 0 and 1 for the player)
        NetworkDingo playerDingo1 = DingoLoader.LoadPrefabWithStats(0);
        NetworkDingo playerDingo2 = null; // Optional second Dingo

        // Assign Dingos to battle slots
        List<NetworkDingo> playerDingos = new List<NetworkDingo> { playerDingo1 };
        if (playerDingo2 != null)
        {
            playerDingos.Add(playerDingo2);
        }

        // Load random opponent Dingos (using random selection from dingoList)
        NetworkDingo opponentDingo1 = DingoLoader.LoadRandomDingoFromList(dingoList);
        NetworkDingo opponentDingo2 = DingoLoader.LoadRandomDingoFromList(dingoList);

        if (opponentDingo1 == null || opponentDingo2 == null)
        {
            Debug.LogError("Failed to load opponent Dingos.");
            return;
        }

        // Flip opponent sprites
        opponentDingo1.isFlipped.Value = true;
        opponentDingo2.isFlipped.Value = true;

        // Assign Dingos to battle slots for this client
        NetworkDingo[] opponentDingos = { opponentDingo1, opponentDingo2 };
        AssignMoveButtons(playerDingo1);
        AssignDingos(clientId, playerDingos.ToArray(), opponentDingos); // Pass clientId to AssignDingos

        // Set positions in the battle prefab for this client
        SetBattlePositions(clientId); // Pass clientId to SetBattlePositions

        // Register client in the battle
        RegisterClientInBattle(clientId, playerDingos.ToArray());
        //BattleStarter.Instance.AssignMoveButtonsClientRPC(clientId);
        Debug.Log($"Battle started successfully for client {clientId}!");
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
    public static void AssignTargetButtons(GameObject battlePrefab, NetworkDingo dingo)
    {
        if (battlePrefab == null || dingo == null)
        {
            Debug.LogError("AssignTargetButtons: Battle prefab or Dingo is null!");
            return;
        }

        // Store the prefab reference
        battlePrefabInstance = battlePrefab;

        // Find target buttons
        GameObject targetButton1 = battlePrefabInstance.transform.Find("Opponents/Canvas/TargetButton1")?.gameObject;
        GameObject targetButton2 = battlePrefabInstance.transform.Find("Opponents/Canvas/TargetButton2")?.gameObject;
        GameObject targetButton3 = battlePrefabInstance.transform.Find("Opponents/Canvas/TargetButton3")?.gameObject;

        if (targetButton1 == null || targetButton2 == null || targetButton3 == null)
        {
            Debug.LogError("AssignTargetButtons: One or more target buttons not found!");
            return;
        }
        AssignTargetToButton(targetButton1, 0, dingo); // First opponent
        AssignTargetToButton(targetButton2, 1, dingo); // Second opponent
        AssignTargetToButton(targetButton3, 2, dingo); // Both opponents (AOE attack)

        Debug.Log("Target selection buttons assigned.");
    }
    private static void AssignTargetToButton(GameObject buttonObj, int targetId, NetworkDingo selectedDingo)
    {
        Button button = buttonObj.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError($"AssignTargetToButton: Button component missing on {buttonObj.name}.");
            return;
        }
        buttonObj.SetActive(true);
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SelectTarget(targetId, selectedDingo));
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
    private static NetworkDingo GetDingoForClientId(ulong clientId)
    {
        // Check if the clientId exists in the BattleSlots dictionary
        if (BattleSlots.TryGetValue(clientId, out var slotDictionary))
        {
            // Iterate through the values of the inner dictionary to find the first valid Dingo
            foreach (var slot in slotDictionary.Values)
            {
                if (slot.Dingo != null) // Access the NetworkDingo inside the BattleSlot
                {
                    return slot.Dingo; // Return the first valid NetworkDingo found
                }
            }
        }

        // If no Dingo is found for the clientId, log a warning and return null
        Debug.LogWarning($"GetDingoForClientId: No Dingo found for client ID {clientId}.");
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
        if (!BattleSlots.ContainsKey(clientId))
        {
            Debug.LogError($"CheckIfPlayersReady: No battle slots found for client {clientId}.");
            return;
        }

        // Check if both players (for this client) are ready
        bool isPlayer1Ready = BattleSlots[clientId].ContainsKey(0) &&
                              BattleSlots[clientId][0].Dingo.battleMoveId.Value != -1 &&
                              BattleSlots[clientId][0].Dingo.battleTargetId.Value != -1;

        bool isPlayer2Ready = BattleSlots[clientId].ContainsKey(1) &&
                              BattleSlots[clientId][1].Dingo.battleMoveId.Value != -1 &&
                              BattleSlots[clientId][1].Dingo.battleTargetId.Value != -1;

        // Proceed if both players for this client are ready
        if (isPlayer1Ready && isPlayer2Ready)
        {
            Debug.Log($"Both players for client {clientId} are ready. Proceeding with the battle.");
            ProceedWithBattle(clientId);
            return;
        }

        Debug.Log($"One or both players for client {clientId} are not ready.");

        // If there's only 1 client, check just the first slot
        if (NetworkManager.Singleton.ConnectedClients.Count == 1 && isPlayer1Ready)
        {
            Debug.Log("Only one client. Proceeding with Player 1's turn.");
            ProceedWithBattle(clientId);
        }
    }
    public static void ProceedWithBattle(ulong clientId)
    {
        Debug.Log($"Proceeding with battle logic for client {clientId}...");

        if (!BattleSlots.ContainsKey(clientId))
        {
            Debug.LogError($"No battle slots found for client {clientId}!");
            return;
        }

        var orderedSlots = BattleSlots[clientId].Values
            .Where(slot => slot.Dingo != null)
            .OrderByDescending(slot => DingoDatabase.GetDingoByID(slot.Dingo.id.Value).Speed)
            .ToList();

        foreach (var battleSlot in orderedSlots)
        {
            NetworkDingo selectedDingo = battleSlot.Dingo;
            if (selectedDingo == null) continue;

            if (battleSlot == BattleSlots[clientId][2] || battleSlot == BattleSlots[clientId][3]) // Enemy AI turn
            {
                List<NetworkDingo> possibleTargets = new List<NetworkDingo>();

                if (BattleSlots[clientId].ContainsKey(1) && BattleSlots[clientId][1].Dingo != null)
                {
                    possibleTargets.Add(BattleSlots[clientId][1].Dingo);
                }

                if (BattleSlots[clientId][0].Dingo != null)
                {
                    possibleTargets.Add(BattleSlots[clientId][0].Dingo);
                }

                if (possibleTargets.Count == 0) continue;

                NetworkDingo target = possibleTargets[UnityEngine.Random.Range(0, possibleTargets.Count)];

                int[] moves = { selectedDingo.move1.Value, selectedDingo.move2.Value, selectedDingo.move3.Value, selectedDingo.move4.Value };
                int chosenMoveId = moves[UnityEngine.Random.Range(0, moves.Length)];

                DingoMove move = DingoDatabase.GetMoveByID(chosenMoveId, DingoDatabase.GetDingoByID(selectedDingo.id.Value));
                if (move == null) continue;

                Debug.Log($"{selectedDingo.name} (Enemy) uses {move.Name} on {target.name}!");
                ApplyMove(clientId, selectedDingo, target, move);
            }
            else // Player turn
            {
                if (selectedDingo.battleMoveId.Value == -1 || selectedDingo.battleTargetId.Value == -1)
                {
                    Debug.LogError($"{selectedDingo.name} has not selected a move or target.");
                    continue;
                }

                DingoMove move = DingoDatabase.GetMoveByID(
                    selectedDingo.battleMoveId.Value,
                    DingoDatabase.GetDingoByID(selectedDingo.id.Value)
                );

                if (move == null)
                {
                    Debug.LogError($"Move with ID {selectedDingo.battleMoveId.Value} not found.");
                    continue;
                }

                Debug.Log($"{selectedDingo.name} is using {move.Name}!");

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
        }
    }
    private static void ApplyMove(ulong clientId, NetworkDingo attacker, NetworkDingo target, DingoMove move)
    {
        if (target == null)
        {
            Debug.LogError($"{attacker.name} tried to attack a null target.");
            return;
        }

        int attackStat = attacker.attack.Value;
        int defenseStat = target.defense.Value;
        int baseDamage = move.Power;

        float damageMultiplier = 1f + ((float)(attackStat - defenseStat) / Mathf.Max(1, defenseStat));
        damageMultiplier = Mathf.Max(damageMultiplier, 0.01f);

        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * damageMultiplier));

        target.hp.Value -= finalDamage;

        Debug.Log($"{attacker.name} used {move.Name} on {target.name} for {finalDamage} damage!");

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
        Debug.Log($"{networkDingo.name} selected move {move.Name}!");

        // Set the move ID on the networked Dingo
        networkDingo.battleMoveId.Value = move.MoveID;

        // Now prompt the player to select a target instead of auto-assigning
        AssignTargetButtons(battlePrefabInstance, networkDingo);

        Debug.Log($"Move {move.Name} selected. Waiting for target choice...");
    }
    private static bool IsPlayerInBattle(ulong clientId)
    {
        // Check if the client is already registered in BattleSlots
        return BattleSlots.ContainsKey(clientId);
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
        // Assign the Dingo to the slot
        BattleSlots[clientId][slotNumber] = new BattleSlot(dingo, slotNumber, isPlayer);

        // Update the Dingo's slotNumber.Value to match the slot
        dingo.slotNumber.Value = slotNumber;
        dingo.GetComponent<NetworkObject>().Spawn();

        // Log the assignment
        Debug.Log($"Assigned Dingo {dingo.name} to slot {slotNumber} for client {clientId} (IsPlayer: {isPlayer}).");
    }
    public static void RegisterClientInBattle(ulong clientId, NetworkDingo[] dingos)
    {
        if (!BattleClients.ContainsKey(clientId))
        {
            BattleClients[clientId] = new List<NetworkDingo>();
        }

        BattleClients[clientId].AddRange(dingos);
        Debug.Log($"Client {clientId} added to battle with {dingos.Length} Dingos.");
    }
    private static void SetBattlePositions(ulong clientId)
    {
        if (battlePrefabInstance == null)
        {
            Debug.LogError("Battle prefab instance is null.");
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
    private static void SetJoinedPlayerPositions(ulong clientId)
    {
        if (battlePrefabInstance == null)
        {
            Debug.LogError("Battle prefab instance is null.");
            return;
        }

        // Find the battle slots (child objects)

        Transform playerSlot2 = battlePrefabInstance.transform.Find("Players/Slot2");
        Transform playerPosition2 = battlePrefabInstance.transform.Find("Players/TrainerPosition2");

        if (playerPosition2 != null)
        {
            int playerNumber = PlayerManager.GetPlayerNumberByClientId(clientId);
            GameObject playerObject = GameObject.Find($"Player{playerNumber}");

            if (playerObject != null)
            {
                Movement playerMovement = playerObject.GetComponent<Movement>();
                if (playerMovement != null)
                {
                    playerMovement.movementEnabled = false;
                    playerObject.transform.position = playerPosition2.position;
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

        if (playerSlot2 == null)
        {
            Debug.LogError("One or more battle slot transforms not found in BattlePrefab.");
            return;
        }

        // Set the Dingos' positions to the respective slots
        if (playerSlot2 != null && BattleSlots[clientId].ContainsKey(1) && BattleSlots[clientId][1].Dingo != null)
        {
            BattleSlots[clientId][1].Dingo.transform.position = playerSlot2.position;
        }


        Debug.Log("Battle positions set successfully.");
    }
    public static void JoinBattleAsPlayer2(ulong clientId, string filePath)
    {
        // Check if a battle is already in progress
        if (BattleSlots.Count == 0)
        {
            Debug.LogError("No battle is currently in progress. Player 2 cannot join.");
            return;
        }

        // Get the existing battle's client ID (assuming only one battle is active at a time)
        ulong existingClientId = BattleSlots.Keys.First();

        // Check if Player 2 is already in the battle
        if (BattleSlots.ContainsKey(clientId))
        {
            Debug.LogError($"Client {clientId} is already in the battle.");
            return;
        }

        // Validate the file path
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("Failed to get the file path for Player 2's Dingo data.");
            return;
        }

        // Load Player 2's Dingo for slot 0 (first Dingo)
        NetworkDingo player2Dingo1 = DingoLoader.LoadNetworkDingoFromFileToReceive(filePath, 0);
        if (player2Dingo1 == null)
        {
            Debug.LogError("Failed to load Player 2's first Dingo from the file.");
            return;
        }

        // Create a list of Player 2's Dingos (assuming only one Dingo for now)
        List<NetworkDingo> player2Dingos = new List<NetworkDingo> { player2Dingo1 };

        // Get the opponent's Dingos (Player 1's Dingos)
        NetworkDingo[] opponentDingos = BattleSlots[existingClientId].Values
            .Where(slot => !slot.IsPlayer)
            .Select(slot => slot.Dingo)
            .ToArray();

        if (!BattleSlots.ContainsKey(clientId))
        {
            BattleSlots[clientId] = new Dictionary<int, BattleSlot>(); // Create new battle slot dictionary for this player
        }


        AssignDingoToSlot(clientId, player2Dingo1, 1, true);
        SetJoinedPlayerPositions(clientId);
        // Register Player 2 in the battle
        RegisterClientInBattle(clientId, player2Dingos.ToArray());

        Debug.Log($"Player 2 (client {clientId}) has joined the battle!");
    }
    private static void HandleDingoFaint(ulong clientId, NetworkDingo dingo)
    {
        Debug.Log($"{dingo.name} has fainted!");
        dingo.gameObject.SetActive(false);

        // Find the slot where the fainted Dingo is located
        foreach (var slot in BattleSlots[clientId])
        {
            if (slot.Value.Dingo == dingo)
            {
                int slotNumber = slot.Key;

                // Handle player Dingos (slots 0 and 1)
                if (slotNumber == 0 || slotNumber == 1)
                {
                    Debug.Log($"Player Dingo in slot {slotNumber} has fainted.");

                    // Check if both player slots are empty
                    bool bothPlayerSlotsEmpty = BattleSlots[clientId][0].Dingo.hp.Value <= 0 &&
                                                BattleSlots[clientId][1].Dingo.hp.Value <= 0;

                    if (bothPlayerSlotsEmpty)
                    {
                        Debug.Log("Both player Dingos have fainted! Ending battle...");
                        EndBattle(clientId);
                        return;
                    }

                    // Replace the fainted player Dingo with a new one
                    //slot.Value.Dingo = DingoLoader.LoadPrefabWithStats(1);
                    Debug.Log($"New player Dingo spawned in slot {slotNumber}.");
                    return;
                }

                // Handle opponent Dingos (slots 2 and 3)
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

        Debug.LogError($"Fainted Dingo {dingo.name} not found in any battle slot for client {clientId}.");
    }
    private static void EndBattle(ulong clientId)
    {
        Debug.Log($"Ending battle for client {clientId} and restoring player position...");

        // Remove only this client's battle slots
        if (BattleSlots.ContainsKey(clientId))
        {
            foreach (var slot in BattleSlots[clientId])
            {
                if (slot.Value.Dingo != null)
                {
                    slot.Value.Dingo.gameObject.SetActive(false); // Hide the Dingo
                }
            }
            BattleSlots.Remove(clientId);
        }

        // Restore only this player's original position
        if (playerStartPositions.ContainsKey(clientId))
        {
            Vector3 originalPosition = playerStartPositions[clientId];

            int playerNumber = PlayerManager.GetPlayerNumberByClientId(clientId);

            // Find the player GameObject
            GameObject playerObject = GameObject.Find($"Player{playerNumber}");
            if (playerObject != null)
            {
                // Restore movement
                playerObject.transform.position = originalPosition;
                Movement playerMovement = playerObject.GetComponent<Movement>();
                if (playerMovement != null)
                {
                    playerMovement.movementEnabled = true;
                }

                // Update Camera Follow Target
                CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
                if (cameraFollow != null)
                {
                    cameraFollow.target = playerObject.transform;
                }
                else
                {
                    Debug.LogError("CameraFollow script not found on the main camera!");
                }
            }
            playerStartPositions.Remove(clientId); // Remove player's position record

            // Call BattleStarter's EndBattle method
            if (BattleStarter.Instance != null)
            {
                BattleStarter.Instance.EndBattle(clientId);
            }
        }

        // Clean up battle prefab if no more players are in battle
        if (BattleSlots.Count == 0 && battlePrefabInstance != null)
        {
            GameObject.Destroy(battlePrefabInstance);
            battlePrefabInstance = null;
        }

        Debug.Log($"Battle ended and cleaned up for client {clientId}.");
    }
}
