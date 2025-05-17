using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using DingoSystem;
using UnityEngine.UI;
using Unity.Collections;
using System.Collections;
using System;

public class BattleStarter : NetworkBehaviour
{
    public static BattleStarter Instance { get; private set; }
    private static HashSet<Vector3> activeBattleSpots = new HashSet<Vector3>();
    private static Dictionary<ulong, Vector3> clientBattleSpots = new Dictionary<ulong, Vector3>();
    private static Dictionary<ulong, int> clientSlotIndex = new Dictionary<ulong, int>();
    public static Dictionary<ulong, int> clientDingoCount = new Dictionary<ulong, int>();
    private bool isBattlePaused;
    public BattleDingos battleDingos;
    public GameObject battleUI; // Reference to your battle UI GameObject
    public GameObject nonBattleUI; // Reference to your non-battle UI GameObject
    public class BattleEndEventArgs : EventArgs
    {
        public ulong ClientId { get; set; }
        public bool Won { get; set; }
        public int TrainerId { get; set; }
        public string TrainerName { get; set; }
    }

    // Modify the event declaration
    public static event EventHandler<BattleEndEventArgs> OnBattleEnd;
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
    // In BattleStarter.cs
    [ClientRpc]
    public void SetBattleUIVisibilityClientRPC(bool battleActive, ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;

        if (battleUI != null) battleUI.SetActive(battleActive);
        if (nonBattleUI != null) nonBattleUI.SetActive(!battleActive);
    }
    // Add to BattleStarter class
    [ClientRpc]
    public void SetPlayerPhysicsStateClientRPC(bool inBattle, ulong clientId, int playerNumber)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;


        GameObject playerObject = GameObject.Find($"Player{playerNumber}");

        if (playerObject != null && playerObject.TryGetComponent<Movement>(out var movement))
        {
            movement.SetPhysicsStateForBattle(inBattle);
        }
    }
    public void RequestStartBattle(ulong clientId, int dingoList, Vector3 triggerPosition, string filePath, string agentBingoPath, bool isTrainer, int trainerSprite)
    {
        Debug.Log($"[BattleStarter] RequestStartBattle called by client {clientId} at position {triggerPosition}.");
        // Load the player's Dingo count from their save file
        int dingoCount = DingoLoader.GetPlayerDingoCount(filePath);
        clientDingoCount[clientId] = dingoCount;
        if (IsServer)
        {
            Debug.Log("[BattleStarter] Running on server, handling directly.");
            HandleStartBattleServerRPC(clientId, dingoList, triggerPosition, filePath, agentBingoPath, isTrainer, trainerSprite);
        }
        else
        {
            Debug.Log("[BattleStarter] Running on client, sending ServerRpc.");
            RequestStartBattleServerRpc(clientId, dingoList, triggerPosition, filePath, agentBingoPath, isTrainer, trainerSprite);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestStartBattleServerRpc(ulong clientId, int dingoList, Vector3 triggerPosition, string filePath, string agentBingoPath, bool isTrainer, int trainerSprite)
    {
        Debug.Log($"[Server] Received battle start request from client {clientId} at position {triggerPosition}.");
        HandleStartBattleServerRPC(clientId, dingoList, triggerPosition, filePath, agentBingoPath, isTrainer, trainerSprite);
    }
    [ServerRpc]
    private void HandleStartBattleServerRPC(ulong clientId, int dingoList, Vector3 triggerPosition, string filePath, string agentBingoPath, bool isTrainer, int trainerSprite)
    {
        Debug.Log($"[Server] HandleStartBattleServerRPC called for Client {clientId} at Position {triggerPosition}");
        // Check if the client is already in a battle
        if (clientBattleSpots.ContainsKey(clientId))
        {
            Debug.Log($"[Server] Client {clientId} is already in battle. Ignoring battle start request.");
            return;
        }

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
            //string filePath = DingoLoader.LoadPlayerDingoFromFileToSend();
            // Join the battle as Player 2
            BattleHandler.JoinBattleAsPlayer2(clientId, filePath, agentBingoPath);
            return;
        }

        // Reserve the battle spot for Player 1
        ReserveBattleSpot(closestSpot.transform.position);

        // Store the client-battle spot association
        clientBattleSpots[clientId] = closestSpot.transform.position;

        // Start the battle for Player 1
        BattleHandler.StartBattle(clientId, dingoList, closestSpot.transform.position, filePath, agentBingoPath, isTrainer, trainerSprite);
        Debug.Log($"[Server] Battle started successfully for client {clientId}!");
    }
    public void RequestDingoSpawn(ulong clientId, int battleSlotIndex)
    {
        if (IsOwner) // Ensure the client is the one calling the spawn
        {
            string filePath = DingoLoader.LoadPlayerDingoFromFileToSend();

            // Ask the host to spawn this Dingo into the given slot for the specific client
            RequestDingoSpawnServerRpc(clientId, battleSlotIndex, filePath);
        }
    }
    [ServerRpc]
    private void RequestDingoSpawnServerRpc(ulong clientId, int battleSlotIndex, string filePath)
    {
        // Spawn the Dingo on the host side
        NetworkDingo playerDingo = DingoLoader.LoadNetworkDingoFromFileToReceive(filePath, 0);

        BattleHandler.SpawnDingoInSlot(clientId, battleSlotIndex, playerDingo);
    }
    [ClientRpc]
    public void AssignMoveButtonsSlotClientRPC(ulong clientId, int slot)
    {
        if (!(clientId == NetworkManager.Singleton.LocalClientId)) { return; }

        StartCoroutine(AssignButtonsAfterDelay(slot));
    }
    private IEnumerator AssignButtonsAfterDelay(int slot)
    {
        int maxAttempts = 5;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            yield return new WaitForSeconds(0.1f); // Slight delay between attempts
            if (NewAssignMoveButtons(slot))
            {
                yield break; // Success, stop coroutine
            }

            attempts++;
        }

        Debug.LogError("AssignButtonsAfterDelay: Failed to assign move buttons after multiple attempts.");
    }
    // Updated to return bool indicating success
    public bool NewAssignMoveButtons(int moveslot)
    {

        // Find move buttons in the scene
        GameObject moveButton1 = GameObject.Find("MoveButton1");
        GameObject moveButton2 = GameObject.Find("MoveButton2");
        GameObject moveButton3 = GameObject.Find("MoveButton3");
        GameObject moveButton4 = GameObject.Find("MoveButton4");

        if (moveButton1 == null || moveButton2 == null || moveButton3 == null || moveButton4 == null)
        {
            Debug.LogError("AssignMoveButtons: One or more move buttons not found in the scene!");
            return false;
        }
        DingoMove[] moves = null;
        if (moveslot == -1)
        {
            moves = DingoLoader.LoadAgentBingoMoves();

        }
        else
        {
            moves = DingoLoader.LoadDingoMoves(moveslot);

        }
        if (moves == null || moves.Length < 4)
        {
            Debug.LogError("AssignMoveButtons: Failed to load moves for Dingo.");
            return false;
        }

        // Assign move names and functions to buttons
        NewAssignMoveToButton(moveButton1, moves[0]);
        NewAssignMoveToButton(moveButton2, moves[1]);
        NewAssignMoveToButton(moveButton3, moves[2]);
        NewAssignMoveToButton(moveButton4, moves[3]);

        Debug.Log("Assigned moves to buttons and updated text.");
        return true;

    }
    private void NewAssignMoveToButton(GameObject buttonObj, DingoMove move)
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
        button.onClick.AddListener(() => NewAssignTargetButtons(NetworkManager.Singleton.LocalClientId)); // Assign new move function

        //NewAssignTargetButtons();
    }
    [ServerRpc(RequireOwnership = false)]
    public void SelectMoveServerRpc(ulong clientId, int moveId, ServerRpcParams rpcParams = default)
    {
        bool isBattlePaused = IsBattlePaused(clientId);

        if (isBattlePaused)
        {
            return;
        }
        // Handle the move selection on the host
        Debug.Log($"Client {clientId} selected move {moveId}");
        // You can now update the game state or perform other actions based on the move selection
        NetworkDingo dingo = BattleHandler.GetPlayer2Dingo(clientId);
        if (dingo.battleMoveId.Value == -2)
        {

            Debug.Log("Cannot switch moves after attempting to catch.");
            return;

        }
        dingo.battleMoveId.Value = moveId;

        Debug.Log($"Dingo {dingo.name.Value} (Client {clientId}) is selecting move {dingo.battleMoveId.Value} slot: {dingo.slotNumber.Value}");
    }
    public void NewAssignTargetButtons(ulong clientId)
    {
        bool isBattlePaused = IsBattlePaused(clientId);

        if (isBattlePaused)
        {
            return;
        }
        // Find target buttons
        GameObject targetButton1 = GameObject.Find("Canvas/Battle/Target1");
        GameObject targetButton2 = GameObject.Find("Canvas/Battle/Target2");
        GameObject targetButton3 = GameObject.Find("Canvas/Battle/Target3");
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
        buttonObj.SetActive(true);
        if (button == null)
        {
            Debug.LogError($"AssignTargetToButton: Button component missing on {buttonObj.name}.");
            return;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SelectTargetServerRpc(NetworkManager.Singleton.LocalClientId, targetId));
        //button.onClick.AddListener(NewAssignTargetButtons);
    }
    public bool IsBattlePaused(ulong clientId)
    {
        return isBattlePaused && NetworkManager.Singleton.LocalClientId == clientId;
    }
    public void SwitchPlayerDingos(int slot)
    {
        string file = DingoLoader.LoadPlayerDingoFromFileToSend();
        SwitchPlayerDingosServerRPC(slot, file, NetworkManager.Singleton.LocalClientId);
    }
    public void UseAgentBingo()
    {
        string file = DingoLoader.LoadPlayerDataFromFileToSend();
        UseAgentBingoServerRpc(file, NetworkManager.Singleton.LocalClientId);
    }
    [ServerRpc(RequireOwnership = false)]
    public void UseAgentBingoServerRpc(string file, ulong clientId)
    {
        BattleHandler.SwitchAgentBingo(file, clientId);
    }
    [ServerRpc(RequireOwnership = false)]
    public void SwitchPlayerDingosServerRPC(int slot, string file, ulong clientId)
    {
        StoreClientSlot(clientId, slot);
        BattleHandler.SwitchDingos(slot, file, clientId);
        AssignMoveButtonsSlotClientRPC(clientId, slot);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestFeedDingoServerRpc(ulong clientId, int targetId, int foodItemId, ServerRpcParams rpcParams = default)
    {
        // Get the player's Dingo
        NetworkDingo playerDingo = BattleHandler.GetPlayer2Dingo(clientId);
        if (playerDingo == null) return;

        // Check if player has already made their move this turn
        if (playerDingo.battleMoveId.Value != -1)
        {
            Debug.Log($"Client {clientId} already made their move this turn");
            return;
        }

        NetworkDingo targetDingo = BattleHandler.GetPlayerDingo(clientId);
        if (targetDingo == null) return;

        // Mark the player's turn as complete (-3 = feed attempt)
        playerDingo.hasAttemptedCatch.Value = true;
        playerDingo.battleMoveId.Value = -2;
        playerDingo.battleTargetId.Value = targetId;

        // Trigger animation on the client who owns the player Dingo
        PlayFeedAnimationClientRpc(clientId, targetDingo.NetworkObjectId, foodItemId);
    }

    [ClientRpc]
    public void PlayFeedAnimationClientRpc(ulong clientId, ulong targetDingoId, int foodItemId)
    {
        // Only the feeding client should process this
        if (NetworkManager.Singleton.LocalClientId != clientId) return;

        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetDingoId, out NetworkObject netObj);
        if (netObj == null) return;

        NetworkDingo targetDingo = netObj.GetComponent<NetworkDingo>();
        if (targetDingo == null) return;

        // Start the animation coroutine on this client
        StartCoroutine(FeedAnimation(targetDingoId, foodItemId, clientId, targetDingo, success => {
            // Send result back to server
            FeedResultServerRpc(clientId, targetDingoId, success, foodItemId);
        }));
    }

    public IEnumerator FeedAnimation(ulong targetDingoId, int foodItemId, ulong clientId, NetworkDingo targetDingo, Action<bool> onComplete)
    {
        if (targetDingo == null)
        {
            onComplete?.Invoke(false);
            yield break;
        }

        Transform bingoTransform = null;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<NetworkObject>().IsOwner)
            {
                bingoTransform = player.transform;
                break;
            }
        }

        if (bingoTransform == null)
        {
            Debug.LogWarning("FeedAnimation: Could not find local player.");
            onComplete?.Invoke(false);
            yield break;
        }

        // Store original position and rotation
        Vector3 originalPosition = bingoTransform.position;
        Quaternion originalRotation = bingoTransform.rotation;

        // Dynamic positions
        Vector3 targetPosition = targetDingo.transform.position;

        // Midpoints with vertical offset
        Vector3 mid1 = Vector3.Lerp(originalPosition, targetPosition, 0.5f) + Vector3.up * 1f;
        Vector3 mid2 = Vector3.Lerp(targetPosition, originalPosition, 0.5f) + Vector3.up * 1f;

        float speed = 0.5f;

        // Approach the target
        float start = Time.time;
        while (Time.time - start < 1f * speed)
        {
            float t = (Time.time - start) / (1f * speed);
            bingoTransform.position = BezierCurve(originalPosition, mid1, targetPosition, t);
            yield return null;
        }

        // Play feed effects (could add particle effects here)
        yield return new WaitForSeconds(0.5f);
        InventoryManager.Instance.UnequipServerRpc(clientId);
        FeedDingoServerRpc(clientId, targetDingoId, foodItemId);
        InventoryManager.Instance.RemoveItemServerRpc(foodItemId, NetworkManager.Singleton.LocalClientId);
        // Return to original position
        start = Time.time;
        while (Time.time - start < 1f * speed)
        {
            float t = (Time.time - start) / (1f * speed);
            bingoTransform.position = BezierCurve(targetPosition, mid2, originalPosition, t);
            yield return null;
        }

        // Ensure exact final position
        bingoTransform.position = originalPosition;
        bingoTransform.rotation = originalRotation;

        onComplete?.Invoke(true);
    }
    [ServerRpc(RequireOwnership = false)]
    private void FeedDingoServerRpc(ulong clientId, ulong targetDingoId, int foodItemId)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetDingoId, out NetworkObject netObj);
        if (netObj == null) return;

        NetworkDingo dingo = netObj.GetComponent<NetworkDingo>();
        if (dingo == null) return;
        BattleHandler.ApplyFoodEffects(clientId, dingo, foodItemId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void FeedResultServerRpc(ulong clientId, ulong targetDingoId, bool success, int foodItemId)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetDingoId, out NetworkObject netObj);
        if (netObj == null) return;

        NetworkDingo targetDingo = netObj.GetComponent<NetworkDingo>();
        if (targetDingo == null) return;

        // Mark feed attempt as complete for this player
        BattleHandler.MarkCatchInProgress(clientId, false);

        if (success)
        {
            Debug.Log($"Player {clientId} successfully fed {targetDingo.name.Value} with item {foodItemId}!");
            // Apply food effects here

        }
        else
        {
            Debug.Log($"Player {clientId} failed to feed {targetDingo.name.Value}!");
        }

        // Only proceed if there are no more ongoing feed attempts in this battle
        if (!BattleHandler.IsCatchInProgress(clientId))
        {
            BattleHandler.CheckIfPlayersReady(clientId);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestCatchDingoServerRpc(ulong clientId, int targetId, ServerRpcParams rpcParams = default)
    {
        // Get the player's Dingo
        NetworkDingo playerDingo = BattleHandler.GetPlayer2Dingo(clientId);
        if (playerDingo == null) return;

        // Check if player has already made their move this turn
        if (playerDingo.battleMoveId.Value != -1)
        {
            Debug.Log($"Client {clientId} already made their move this turn");
            return;
        }

        NetworkDingo targetDingo = BattleHandler.GetOpponentDingo(clientId, targetId);
        if (targetDingo == null) return;

        // Mark the player's turn as complete (-2 = catch attempt)
        playerDingo.hasAttemptedCatch.Value = true;
        playerDingo.battleMoveId.Value = -2;
        playerDingo.battleTargetId.Value = targetId;

        // Mark catch in progress
        BattleHandler.MarkCatchInProgress(clientId, true);

        // Calculate catch chance (you can adjust this formula)
        float catchChance = 0.7f;

        // Trigger animation on the client who owns the player Dingo
        PlayCatchAnimationClientRpc(clientId, targetDingo.NetworkObjectId, catchChance);
    }
    [ClientRpc]
    public void PlayCatchAnimationClientRpc(ulong clientId, ulong targetDingoId, float catchChance)
    {
        // Only the catching client should process this
        if (NetworkManager.Singleton.LocalClientId != clientId) return;

        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetDingoId, out NetworkObject netObj);
        if (netObj == null) return;

        NetworkDingo targetDingo = netObj.GetComponent<NetworkDingo>();
        if (targetDingo == null) return;

        // Start the animation coroutine on this client
        StartCoroutine(CatchAnimation(targetDingo, success =>
        {
            // Send result back to server
            CatchResultServerRpc(clientId, targetDingoId, success);
        }, catchChance));
    }
    [ClientRpc]
    public void MoveAnimationClientRPC(
        ulong clientId,
        ulong slotNetworkID0,
        ulong slotNetworkID1,
        ulong slotNetworkID2,
        ulong slotNetworkID3,
        string movename,
        int movetype)
    {
        if (NetworkManager.Singleton.IsHost) return;

        // Retrieve all relevant network objects
        var spawnManager = NetworkManager.Singleton.SpawnManager;
        spawnManager.SpawnedObjects.TryGetValue(slotNetworkID0, out var netObj0);
        spawnManager.SpawnedObjects.TryGetValue(slotNetworkID1, out var netObj1);
        spawnManager.SpawnedObjects.TryGetValue(slotNetworkID2, out var netObj2);
        spawnManager.SpawnedObjects.TryGetValue(slotNetworkID3, out var netObj3);

        // Get NetworkDingo components, null-safe
        NetworkDingo dingoSlot0 = netObj0?.GetComponent<NetworkDingo>();
        NetworkDingo dingoSlot1 = netObj1?.GetComponent<NetworkDingo>();
        NetworkDingo dingoSlot2 = netObj2?.GetComponent<NetworkDingo>();
        NetworkDingo dingoSlot3 = netObj3?.GetComponent<NetworkDingo>();

        Debug.Log($"LocalAnimationFunction starting with {dingoSlot0} {dingoSlot2} {movename} {movetype}");

        // You can expand this later to include other slots if needed
        StartCoroutine(BattleHandler.LocalAnimationFunction(clientId, dingoSlot0, dingoSlot2, movename, movetype));
    }


    [ServerRpc(RequireOwnership = false)]
    private void CatchResultServerRpc(ulong clientId, ulong targetDingoId, bool success)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetDingoId, out NetworkObject netObj);
        if (netObj == null) return;

        NetworkDingo targetDingo = netObj.GetComponent<NetworkDingo>();
        if (targetDingo == null) return;

        // Mark catch attempt as complete for this player
        BattleHandler.MarkCatchInProgress(clientId, false);

        if (success)
        {
            Debug.Log($"Player {clientId} successfully caught {targetDingo.name.Value}!");
            BattleHandler.AttemptCatchDingo(clientId, targetDingo);
        }
        else
        {
            Debug.Log($"Player {clientId} failed to catch {targetDingo.name.Value}!");
        }

        // Get the host client ID for this battle
        ulong hostClientId = BattleHandler.GetHostClientIdForDingo(targetDingo);

        // Only proceed if there are no more ongoing catch attempts in this battle
        if (!BattleHandler.IsCatchInProgress(hostClientId))
        {
            BattleHandler.CheckIfPlayersReady(hostClientId);
        }
    }

    public IEnumerator CatchAnimation(NetworkDingo targetDingo, Action<bool> onComplete, float catchChance)
    {
        if (targetDingo == null)
        {
            onComplete?.Invoke(false);
            yield break;
        }

        // Get the SpriteRenderer component to disable when caught
        SpriteRenderer dingoSprite = targetDingo.GetComponent<SpriteRenderer>();

        Transform bingoTransform = null;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<NetworkObject>().IsOwner)
            {
                bingoTransform = player.transform;
                break;
            }
        }

        if (bingoTransform == null)
        {
            Debug.LogWarning("CatchAnimation: Could not find local player.");
            onComplete?.Invoke(false);
            yield break;
        }

        // Store original position and rotation
        Vector3 originalPosition = bingoTransform.position;
        Quaternion originalRotation = bingoTransform.rotation;

        // Dynamic positions
        Vector3 targetPosition = targetDingo.transform.position;

        // Midpoints with vertical offset
        Vector3 mid1 = Vector3.Lerp(originalPosition, targetPosition, 0.5f) + Vector3.up * 2f;
        Vector3 mid2 = Vector3.Lerp(targetPosition, originalPosition, 0.5f) + Vector3.up * 2f;

        float[] spins = { 0.5f, 1f, 1.5f };
        float spin = spins[UnityEngine.Random.Range(0, spins.Length)];

        int inTime = UnityEngine.Random.Range(1, 4);
        int outTime = UnityEngine.Random.Range(1, 3);
        float speed = 0.5f;

        // Approach the target
        float start = Time.time;
        while (Time.time - start < inTime * speed)
        {
            float t = (Time.time - start) / (inTime * speed);
            bingoTransform.position = BezierCurve(originalPosition, mid1, targetPosition, t);
            bingoTransform.Rotate(Vector3.forward, spin);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        float roll = UnityEngine.Random.value;
        int percent = Mathf.RoundToInt(roll * 100);
        Debug.Log($"Agent Bingo rolled a {percent}. You need {catchChance * 100} or higher.");

        bool success = roll >= catchChance;
        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        if (BattleHandler.IsTrainerBattle(localClientId))
        {

            RequestTrainerSwatServerRpc(localClientId, targetPosition);
            success = false;
            yield return new WaitForSeconds(1f);
        }

        if (success)
        {
            // Disable the Dingo's sprite when caught
            if (dingoSprite != null)
            {
                dingoSprite.enabled = false;
            }

            // Play catch success effects
            yield return new WaitForSeconds(0.5f);
        }

        // Return to original position (with spin in both cases)
        start = Time.time;
        while (Time.time - start < outTime * speed)
        {
            float t = (Time.time - start) / (outTime * speed);
            bingoTransform.position = BezierCurve(
                success ? targetPosition : targetPosition,
                mid2,
                originalPosition,
                t
            );

            // Continue spinning during return
            bingoTransform.Rotate(Vector3.forward, success ? -spin : spin);
            yield return null;
        }

        // Ensure exact final position
        bingoTransform.position = originalPosition;
        bingoTransform.rotation = originalRotation;

        onComplete?.Invoke(success);
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestTrainerSwatServerRpc(ulong clientId, Vector3 targetDingo)
    {
        // Get the trainer defending their Dingo
        if (!BattleHandler._trainerInstances.TryGetValue(clientId, out NetworkTrainer defendingTrainer))
        {
            ulong hostId = BattleHandler.GetHostForPlayer2(clientId);
            if (!BattleHandler._trainerInstances.TryGetValue(hostId, out defendingTrainer))
            {
                Debug.LogWarning($"No trainer found for client {clientId} or host {hostId}");
                return;
            }
        }
        StartCoroutine(TrainerSwatAnimation(defendingTrainer, targetDingo));
    }
    private IEnumerator TrainerSwatAnimation(NetworkTrainer networkTrainer, Vector3 target)
    {
        Transform trainerTransform = networkTrainer.transform;
        if (trainerTransform == null) yield break;
        NpcMovementScriptStatic moveScript = trainerTransform.GetComponent<NpcMovementScriptStatic>();
        Vector3 trainerOriginalPos = trainerTransform.position;
        float duration = 0.5f;
        float pullbackDelay = 0.4f;
        float elapsed = 0f;

        Vector3 swatPeakPosition = trainerOriginalPos;
        moveScript.SetMovement(true);
        // Move trainer toward the target
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            trainerTransform.position = Vector3.Lerp(trainerOriginalPos, target, t);

            // Capture position after pullbackDelay
            if (elapsed >= pullbackDelay && swatPeakPosition == trainerOriginalPos)
            {
                swatPeakPosition = trainerTransform.position;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Slight pullback to swatPeakPosition
        elapsed = 0f;
        Vector3 startPullback = trainerTransform.position;
        float pullbackDuration = 0.1f;
        while (elapsed < pullbackDuration)
        {
            float t = elapsed / pullbackDuration;
            trainerTransform.position = Vector3.Lerp(startPullback, swatPeakPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        moveScript.SetMovement(false);
        // Message delay
        BattleHandler.SendBattleMessage(NetworkManager.Singleton.LocalClientId, networkTrainer.trainerName + ": Hey!!!");
        yield return new WaitForSeconds(0.5f);
        moveScript.SetMovement(true);

        // Return to original position
        elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            trainerTransform.position = Vector3.Lerp(trainerTransform.position, trainerOriginalPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        moveScript.SetMovement(false);
    }
    private Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }
    public void NewAssignCatchButtons()
    {
        // Find target buttons
        GameObject targetButton1 = GameObject.Find("Canvas/Battle/Target1");
        GameObject targetButton2 = GameObject.Find("Canvas/Battle/Target2");
        GameObject targetButton3 = GameObject.Find("Canvas/Battle/Target3");

        if (targetButton1 == null || targetButton2 == null || targetButton3 == null)
        {
            Debug.LogError("AssignCatchButtons: One or more target buttons not found!");
            return;
        }

        NewAssignCatchToButton(targetButton1, 0); // First opponent
        NewAssignCatchToButton(targetButton2, 1); // Second opponent
        NewAssignCatchToButton(targetButton3, 2); // Both opponents (not used for catching)
    }
    private void NewAssignCatchToButton(GameObject buttonObj, int targetId)
    {
        Button button = buttonObj.GetComponent<Button>();
        buttonObj.SetActive(true);
        if (button == null)
        {
            Debug.LogError($"AssignCatchToButton: Button component missing on {buttonObj.name}.");
            return;
        }

        // Get the player's Dingo
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        NetworkDingo playerDingo = BattleHandler.GetPlayer2Dingo(clientId);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            // Only allow catch if player hasn't made a move yet this turn
            if (playerDingo == null || playerDingo.battleMoveId.Value == -1 || playerDingo.battleMoveId.Value == -2)
            {
                RequestCatchDingoServerRpc(clientId, targetId);
            }
            else
            {
                Debug.Log("You must wait until next turn to attempt another catch");
            }
        });
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestSaveDingoServerRPC(
        ulong clientId,
        int dingoId,
        FixedString64Bytes dingoName,
        FixedString32Bytes dingoType,
        int currentHp,
        int maxHp,
        int atk,
        int def,
        int spd,
        FixedString128Bytes spritePath,
        int xp,
        int maxXp,
        int level,
        int move1Id,
        int move2Id,
        int move3Id,
        int move4Id, bool wild)
    {
        Debug.Log("SHINGO...");
        int slotIndex;
        if (wild)
        {
            slotIndex = -1;
        }
        else
        {
            slotIndex = GetClientSlot(clientId);
        }
        if (slotIndex == -2)
        {
            Debug.LogError($"No save slot found for client {clientId}");
            return;
        }

        SaveDingoDataClientRPC(
            clientId,
            slotIndex,
            dingoId,
            dingoName.Value,
            dingoType.Value,
            currentHp,
            maxHp,
            atk,
            def,
            spd,
            spritePath.Value,
            xp,
            maxXp,
            level,
            move1Id,
            move2Id,
            move3Id,
            move4Id
        );
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestSavePlayerServerRPC(
    ulong clientId,
    int dingoId,
    FixedString64Bytes dingoName,
    FixedString32Bytes dingoType,
    int currentHp,
    int maxHp,
    int atk,
    int def,
    int spd,
    FixedString128Bytes spritePath,
    int xp,
    int maxXp,
    int level,
    int move1Id,
    int move2Id,
    int move3Id,
    int move4Id)
    {
        SavePlayerDataClientRPC(
            clientId,
            0,
            dingoId,
            dingoName.Value,
            dingoType.Value,
            currentHp,
            maxHp,
            atk,
            def,
            spd,
            spritePath.Value,
            xp,
            maxXp,
            level,
            move1Id,
            move2Id,
            move3Id,
            move4Id
        );
    }
    [ClientRpc]
    public void SaveDingoDataClientRPC(ulong clientId, int slotIndex, int dingoId, string dingoName, string dingoType,
    int currentHp, int maxHp, int atk, int def, int spd, string spritePath,
    int xp, int maxXp, int level, int move1Id, int move2Id, int move3Id, int move4Id)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;

        BattleHandler.SavePlayerDingoData(
            slotIndex,
            dingoId,
            dingoName,
            dingoType,
            currentHp,
            maxHp,
            atk,
            def,
            spd,
            spritePath,
            xp,
            maxXp,
            level,
            move1Id,
            move2Id,
            move3Id,
            move4Id
        );
    }
    [ClientRpc]
    public void SavePlayerDataClientRPC(ulong clientId, int slotIndex, int dingoId, string dingoName, string dingoType,
int currentHp, int maxHp, int atk, int def, int spd, string spritePath,
int xp, int maxXp, int level, int move1Id, int move2Id, int move3Id, int move4Id)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;

        BattleHandler.SavePlayerCharacterData(
            slotIndex,
            dingoId,
            dingoName,
            dingoType,
            currentHp,
            maxHp,
            atk,
            def,
            spd,
            spritePath,
            xp,
            maxXp,
            level,
            move1Id,
            move2Id,
            move3Id,
            move4Id
        );
    }
    public void StoreClientSlot(ulong clientId, int slotIndex)
    {
        clientSlotIndex[clientId] = slotIndex;
    }
    public int GetClientSlot(ulong clientId)
    {
        return clientSlotIndex.TryGetValue(clientId, out int slot) ? slot : -2;
    }
    [ServerRpc(RequireOwnership = false)]
    public void SelectTargetServerRpc(ulong clientId, int targetId, ServerRpcParams rpcParams = default)
    {
        // Handle the target selection on the host
        Debug.Log($"Client {clientId} selected target {targetId}");
        NetworkDingo dingo = BattleHandler.GetPlayer2Dingo(clientId);
        Debug.Log($"Dingo {dingo.name.Value} (Client {clientId}) is selecting target {targetId} slot: {dingo.slotNumber.Value}");
        dingo.battleTargetId.Value = targetId;
        BattleHandler.CheckIfPlayersReady(clientId);
    }
    [ClientRpc]
    public void CameraPositionClientRPC(Vector2 targetPosition, ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            return;
        }
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.battleActive = true;
            cameraFollow.battlePosition = targetPosition;
        }
        else
        {
            Debug.LogError("CameraFollow script not found on the main camera!");
        }
    }
    [ClientRpc]
    public void ReturnCameraPositionClientRPC(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            return;
        }
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.battleActive = false;
            cameraFollow.battlePosition = Vector3.zero;
        }
    }
    [ClientRpc]
    public void BattlePositionClientRPC(Vector3 targetPosition, ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            return;
        }

        // Only update position if it's the client's turn to move
        if (NetworkManager.Singleton.IsClient)
        {
            // Find the player object
            int playerNumber = PlayerManager.GetPlayerNumberByClientId(clientId);
            GameObject playerObject = GameObject.Find($"Player{playerNumber}");

            if (playerObject != null)
            {
                playerObject.transform.position = targetPosition;

                // Enable movement if applicable
                Movement playerMovement = playerObject.GetComponent<Movement>();
                if (playerMovement != null)
                {
                    playerMovement.movementEnabled = true;
                    playerMovement.SetRigidbodyStatic(false);
                }
            }
            else
            {
                Debug.LogError($"Player {playerNumber} not found to set position.");
            }
        }
    }
    [ClientRpc]
    public void NotifyBattleEndClientRpc(ulong clientId, bool won, int trainerId)
    {
        OnBattleEnd?.Invoke(this, new BattleEndEventArgs
        {
            ClientId = clientId,
            Won = won,
            TrainerId = trainerId
        });
    }
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
    private static bool IsBattleSpotAvailable(Vector3 position)
    {
        return !activeBattleSpots.Contains(position);
    }
    private static void ReserveBattleSpot(Vector3 position)
    {
        activeBattleSpots.Add(position);
    }
    private static void ReleaseBattleSpot(Vector3 position)
    {
        activeBattleSpots.Remove(position);
    }
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
    public void ShowSwitchUI(ulong clientId, int slotNumber)
    {
        ShowSwitchUIClientRPC(clientId, slotNumber);
    }
    [ClientRpc]
    private void ShowSwitchUIClientRPC(ulong clientId, int slotNumber)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            battleDingos.ListDingos();
        }
    }
    public void PauseBattle(ulong clientId, bool pause)
    {
        PauseBattleClientRPC(clientId, pause);
    }
    [ClientRpc]
    private void PauseBattleClientRPC(ulong clientId, bool pause)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            isBattlePaused = pause;
        }
    }
}