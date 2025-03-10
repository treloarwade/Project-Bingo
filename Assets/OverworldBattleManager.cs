using UnityEngine;
using System.Collections.Generic;
using DingoSystem;
using Unity.Netcode;
using System.Collections;
using UnityEngine.UI;
using System.ComponentModel;

public class OverworldBattleManager : NetworkBehaviour
{
    [Header("Player Positions")]
    public GameObject Slot1, Slot2, TrainerPosition1, TrainerPosition2;
    [Header("Opponent Positions")]
    public GameObject Opponent1, Opponent2, OpponentTrainerPosition1, OpponentTrainerPosition2;
    private Dictionary<GameObject, GameObject> entityInPosition = new Dictionary<GameObject, GameObject>();

    public Dictionary<GameObject, bool> positionOccupied = new Dictionary<GameObject, bool>();
    private ulong battleStarterClientId = 99; // Track the clientId of the player who initiated the battle
    private Camera mainCamera;
    private HashSet<ulong> activePlayers = new HashSet<ulong>();
    public NetworkVariable<bool> player1HasChosenMove = new NetworkVariable<bool>();
    public NetworkVariable<bool> player2HasChosenMove = new NetworkVariable<bool>();
    private Button targetButton1, targetButton2, bothTargetsButton;
    private Dictionary<int, GameObject> slotMapping;
    private void Awake()
    {
        CacheUIElements();
        InitializePositions();
        StartCoroutine(DelayCollider2D());
        mainCamera = Camera.main;
        SetCameraToObjectPosition();
    }

    private void CacheUIElements()
    {
        targetButton1 = GameObject.Find("Opponents/Canvas/TargetButton1")?.GetComponent<Button>();
        targetButton2 = GameObject.Find("Opponents/Canvas/TargetButton2")?.GetComponent<Button>();
        bothTargetsButton = GameObject.Find("Opponents/Canvas/TargetButton3")?.GetComponent<Button>();
    }

    private void ShowTargetSelectionUI(int moveId, int slotNumber)
    {
        targetButton1?.gameObject.SetActive(true);
        targetButton1?.onClick.RemoveAllListeners();
        targetButton1?.onClick.AddListener(() => ConfirmAttackServerRPC(1, moveId, slotNumber));

        targetButton2?.gameObject.SetActive(true);
        targetButton2?.onClick.RemoveAllListeners();
        targetButton2?.onClick.AddListener(() => ConfirmAttackServerRPC(2, moveId, slotNumber));

        bothTargetsButton?.gameObject.SetActive(true);
        bothTargetsButton?.onClick.RemoveAllListeners();
        bothTargetsButton?.onClick.AddListener(() => ConfirmAttackServerRPC(3, moveId, slotNumber));
    }

    [ServerRpc(RequireOwnership = false)]
    private void ConfirmAttackServerRPC(int targetPosition, int moveId, int slotNumber)
    {
        NetworkDingo attacker = GetPlayerNetworkDingo(slotNumber);
        if (attacker == null) return;

        attacker.battleMoveId.Value = moveId;
        attacker.battleTargetId.Value = targetPosition;
        CheckIfAllPlayersReady();
        HideTargetSelectionUIClientRPC();
    }

    private void CheckIfAllPlayersReady()
    {
        NetworkDingo player1 = GetPlayerNetworkDingo(1);
        NetworkDingo player2 = GetPlayerNetworkDingo(2);

        if (player1?.battleMoveId.Value != -1 && (player2 == null || player2.battleMoveId.Value != -1))
        {
            ResolveTurn(player1, player2);
        }
    }

    private void ResolveTurn(NetworkDingo player1, NetworkDingo player2)
    {
        if (player2 == null)
        {
            ExecuteMove(player1);
        }
        else
        {
            NetworkDingo first = player1.speed.Value >= player2.speed.Value ? player1 : player2;
            NetworkDingo second = (first == player1) ? player2 : player1;
            ExecuteMove(first);
            ExecuteMove(second);
        }
        ResetBattleState(player1, player2);
    }



    private void ExecuteMove(NetworkDingo attacker)
    {
        if (attacker.battleMoveId.Value == -1) return;

        DingoID dingo = DingoDatabase.GetDingoByID(attacker.id.Value);
        DingoMove move = DingoDatabase.GetMoveByID(attacker.battleMoveId.Value, dingo);
        RequestAttackServerRpc(attacker.battleTargetId.Value, move.Power, move.Accuracy);
    }


    [ServerRpc(RequireOwnership = false)]
    private void RequestAttackServerRpc(int targetPosition, int power, float accuracy)
    {
        switch (targetPosition)
        {
            case 1: UseDamageMove(Opponent1, power, accuracy); break;
            case 2: UseDamageMove(Opponent2, power, accuracy); break;
            case 3: StartCoroutine(StaggerMoves(Opponent1, power, accuracy, Opponent2)); break;
        }
    }
    private IEnumerator StaggerMoves(GameObject target1, int power, float accuracy, GameObject target2)
    {
        UseDamageMove(target1, power, accuracy);
        yield return new WaitForSeconds(0.3f);
        UseDamageMove(target2, power, accuracy);
    }
    private void ResetBattleState(NetworkDingo player1, NetworkDingo player2)
    {
        player1.battleMoveId.Value = -1;
        player1.battleTargetId.Value = 0;
        if (player2 != null)
        {
            player2.battleMoveId.Value = -1;
            player2.battleTargetId.Value = 0;
        }
    }
    private NetworkDingo GetPlayerNetworkDingo(int slotNumber)
    {
        return DecodeSlotNumber(slotNumber)?.GetComponent<NetworkDingo>();
    }
    [ClientRpc]
    private void HideTargetSelectionUIClientRPC()
    {
        targetButton1?.gameObject.SetActive(false);
        targetButton2?.gameObject.SetActive(false);
        bothTargetsButton?.gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestMoveServerRPC(int slotNumber, int moveId)
    {
        GameObject targetEntity = DecodeSlotNumber(slotNumber);
        if (targetEntity == null)
        {
            Debug.LogWarning($"No entity found in slot {slotNumber}.");
            return;
        }

        // Ensure the target entity has a NetworkDingo component
        NetworkDingo networkDingo = targetEntity.GetComponent<NetworkDingo>();
        if (networkDingo == null)
        {
            Debug.LogWarning("Target entity has no NetworkDingo component.");
            return;
        }

        DingoID dingo = DingoDatabase.GetDingoByID(networkDingo.id.Value);
        DingoMove move = DingoDatabase.GetMoveByID(moveId, dingo);

        Text targetText = targetEntity.GetComponentInChildren<Text>();
        if (targetText != null)
        {
            targetText.text = $"Move: {move.Name}"; // Display the move's name
        }
        else
        {
            Debug.LogWarning("No Text component found in the target entity's children.");
        }
    }
    private void InitializePositions()
    {
        // Find all positions under Players and Opponents
        Slot1 = FindChildByName("Players", "Slot1");
        Slot2 = FindChildByName("Players", "Slot2");
        TrainerPosition1 = FindChildByName("Players", "TrainerPosition1");
        TrainerPosition2 = FindChildByName("Players", "TrainerPosition2");

        Opponent1 = FindChildByName("Opponents", "Opponent1");
        Opponent2 = FindChildByName("Opponents", "Opponent2");
        OpponentTrainerPosition1 = FindChildByName("Opponents", "OpponentTrainerPosition1");
        OpponentTrainerPosition2 = FindChildByName("Opponents", "OpponentTrainerPosition2");

        slotMapping = new Dictionary<int, GameObject>
        {
            {1, Slot1}, {2, Slot2}, {3, Opponent1}, {4, Opponent2}
        };
    }
    private GameObject FindChildByName(string parentName, string childName)
    {
        Transform parentTransform = transform.Find(parentName);
        return parentTransform?.Find(childName)?.gameObject;
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "shoes")
        {
            return;
        }

        // Only let the server handle the logic
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        NetworkObject netObj = collider.GetComponent<NetworkObject>();

        if (netObj == null)
        {
            Debug.Log("[Server] Collider does not have a NetworkObject component.");
            return;
        }

        StartCoroutine(DelayCheck(netObj));
    }
    // Function to decode the slot number into the correct GameObject
    private GameObject DecodeSlotNumber(int slotNumber)
    {
        GameObject slotKey = null;

        switch (slotNumber)
        {
            case 1: slotKey = Slot1; break;
            case 2: slotKey = Slot2; break;
            case 3: slotKey = Opponent1; break;
            case 4: slotKey = Opponent2; break;
            default:
                Debug.LogWarning($"Invalid slot number: {slotNumber}");
                return null;
        }

        if (slotKey != null && entityInPosition.ContainsKey(slotKey))
        {
            return entityInPosition[slotKey];
        }

        return null;
    }
    private int EncodeSlot(GameObject slot)
    {
        if (slot == Slot1) return 1;
        if (slot == Slot2) return 2;
        if (slot == Opponent1) return 3;
        if (slot == Opponent2) return 4;

        Debug.LogWarning($"Invalid slot: {slot.name}");
        return -1; // Return -1 for invalid slots
    }


    public void UseDamageMove(GameObject position, int power, float accuracy)
    {
        if (entityInPosition.ContainsKey(position) && entityInPosition[position] != null)
        {
            GameObject targetEntity = entityInPosition[position];
            NetworkDingo dingo = targetEntity.GetComponent<NetworkDingo>();

            if (dingo != null)
            {
                float hitChance = UnityEngine.Random.value; // Generates a random value between 0 and 1
                if (hitChance <= accuracy) // Check if move hits
                {
                    float typeMultiplier = 1f; // Placeholder for future type effectiveness system
                    int finalDamage = Mathf.RoundToInt(power * typeMultiplier);

                    dingo.hp.Value -= finalDamage; // Apply damage
                    Debug.Log($"{targetEntity.name} was hit for {finalDamage} damage! New health: {dingo.hp.Value}");

                    if (dingo.hp.Value <= 0)
                    {
                        int battlePosition = EncodeSlot(position);

                        RemoveDingoServerRpc(battlePosition);
                    }
                }
                else
                {
                    Debug.Log($"{targetEntity.name} dodged the attack!");
                }
            }
            else
            {
                Debug.LogError($"{targetEntity.name} does not have a NetworkDingo component!");
            }
        }
        else
        {
            Debug.LogWarning($"No entity found at {position.name} to attack.");
        }
    }

    [ServerRpc]
    public void RemoveDingoServerRpc(int position)
    {
        GameObject dingo = DecodeSlotNumber(position);
        StartCoroutine(RemoveDingoFromBattle(dingo));

    }
    public void RemoveDingos(GameObject position)
    {
        if (entityInPosition.ContainsKey(position)) // Check if the position exists in the dictionary
        {
            GameObject targetEntity = entityInPosition[position];

            if (targetEntity != null) // Ensure the entity is not null
            {
                Destroy(targetEntity); // Destroy the target entity
                entityInPosition[position] = null; // Optionally clear the slot after destruction
            }
        }
    }
    public void RemoveDingoByEntity(GameObject entity)
    {
        if (entity == null) return;

        DebugPrintEntityPositions("Before Removing");

        foreach (var kvp in entityInPosition)
        {
            if (kvp.Value == entity)
            {
                Debug.Log($"[RemoveDingoByEntity] Removing {entity.name} from {kvp.Key.name}");

                Destroy(entity); // Destroy the entity
                entityInPosition[kvp.Key] = null; // Clear the position
                positionOccupied[kvp.Key] = false; // Ensure slot is marked as empty

                DebugPrintEntityPositions("After Removing");
                return;
            }
        }

        Debug.LogWarning("[RemoveDingoByEntity] Entity not found in any battle position.");
    }




    private IEnumerator RemoveDingoFromBattle(GameObject dingoObject)
    {
        yield return new WaitForSeconds(0.9f); // Small delay before removal

        Transform dingoTransform = dingoObject.transform;
        Vector3 originalScale = dingoTransform.localScale;
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float newYScale = Mathf.Lerp(originalScale.y, 0f, elapsedTime / duration);
            dingoTransform.localScale = new Vector3(originalScale.x, newYScale, originalScale.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dingoTransform.localScale = new Vector3(originalScale.x, 0f, originalScale.z); // Ensure it fully shrinks
        RemoveDingoByEntity(dingoObject);

        yield return new WaitForSeconds(0.4f);
        bool isOpponent1Empty = IsPositionOccupied(Opponent1);
        yield return new WaitForSeconds(0.1f);
        bool isOpponent2Empty = IsPositionOccupied(Opponent2);
        yield return new WaitForSeconds(0.1f);
        if (!isOpponent1Empty)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                if (Random.value < 0.5f)
                {
                    SpawnNewDingo();
                    Debug.Log("hopefully shit doesn't get crazy");
                }

            }
        }

        yield return new WaitForSeconds(1f);

        isOpponent1Empty = IsPositionOccupied(Opponent1);
        yield return new WaitForSeconds(0.1f);
        isOpponent2Empty = IsPositionOccupied(Opponent2);
        yield return new WaitForSeconds(0.1f);
        Debug.Log("IsPositionOccupied1: " + isOpponent1Empty + " IsPositionOccupied2: " + isOpponent2Empty);


        if (!isOpponent1Empty && !isOpponent2Empty)
        {
            Debug.Log("[Server] Both opponent positions are empty. Attempting to end the battle.");

            if (NetworkManager.Singleton.IsHost)
            {
                BattleEnd(); // End the battle if both positions are empty
                Debug.Log("[Server] BattleEnd() was called.");
            }
        }
        else
        {
            Debug.Log("[Server] At least one opponent position is occupied. Battle will not end.");
        }
        if (!(entityInPosition.ContainsKey(Opponent1) && entityInPosition[Opponent1] != null) && !(entityInPosition.ContainsKey(Opponent2) && entityInPosition[Opponent2] != null))
        {
            if (NetworkManager.Singleton.IsHost)
            {
                BattleEnd(); // End the battle if both positions are empty
            }
        }

    }
    private bool dingoSpawning = false;
    public void SpawnNewDingo()
    {
        if (dingoSpawning)
        {
            return;
        }
        dingoSpawning = true;

        DebugPrintEntityPositions("Before Spawning New Dingo");

        DingoID enemyDingoData = GetRandomDingo();
        int[] enemyMoves = { 0, 1, 2, 3 };

        GameObject emptySlot = null;

        if (!IsPositionOccupied(Opponent1) && entityInPosition.GetValueOrDefault(Opponent1, null) == null)
        {
            emptySlot = Opponent1;
        }
        else if (!IsPositionOccupied(Opponent2) && entityInPosition.GetValueOrDefault(Opponent2, null) == null)
        {
            emptySlot = Opponent2;
        }
        else
        {
            Debug.LogWarning("[SpawnNewDingo] No empty slots available to spawn a new Dingo.");
            dingoSpawning = false;
            return;
        }

        Debug.Log($"[SpawnNewDingo] Spawning new Dingo in {emptySlot.name}");

        SpawnAndAssignDingo(enemyMoves, enemyDingoData, emptySlot);

        DebugPrintEntityPositions("After Spawning New Dingo");
    }



    private void SpawnAndAssignDingo(int[] moves, DingoID dingoData, GameObject slot)
    {
        BattleManagerUtils.RequestDingoSpawn(dingoData.Sprite, (GameObject dingoObject) =>
        {
            NetworkDingo networkDingo = dingoObject.GetComponent<NetworkDingo>();
            if (networkDingo != null)
            {
                networkDingo.SetDingoAttributesServerRpc(
                    dingoData.ID, dingoData.Sprite, dingoData.Name, dingoData.Type,
                    dingoData.HP, dingoData.Attack, dingoData.Defense,
                    dingoData.Speed, dingoData.MaxHP, dingoData.XP,
                    dingoData.MaxXP, dingoData.Level, moves[0], moves[1], moves[2], moves[3]);
            }
            //AssignEntityToPosition(dingoObject, slot);

            StartCoroutine(DelayedAssign(dingoObject, slot));
        });
    }
    public DingoID GetRandomDingo()
    {
        List<DingoID> dingos = new List<DingoID>(DingoDatabase.allDingos);

        if (dingos.Count == 0)
        {
            Debug.LogWarning("No Dingos available to select.");
            return null; // Handle empty list case
        }

        int randomIndex = Random.Range(0, dingos.Count);
        return dingos[randomIndex];
    }
    private void BattleEnd()
    {
        Debug.Log("Battle Ended");
        Debug.Log("Battle Ended");

        Debug.Log("Battle Ended");


        foreach (ulong clientId in activePlayers)
        {
            PlayerManager.SetPlayerBattleStatus(clientId, false);
        }
        PlayerManager.SetPlayerBattleStatus(battleStarterClientId, false);
        activePlayers.Clear(); // Reset the list
        RemoveDingos(Slot1);
        RemoveDingos(Slot2);
        Destroy(gameObject);
    }
    public void SetCameraToObjectPosition()
    {
        if (mainCamera != null)
        {
            // Get the current position of the object the script is attached to
            Vector3 newCameraPosition = transform.position;

            // Adjust the Y position by -0.5
            newCameraPosition.y -= 0.5f;
            newCameraPosition.z = -10;
            // Set the camera's position to the new calculated position
            mainCamera.transform.position = newCameraPosition;
        }
        else
        {
            Debug.LogWarning("Main camera not found!");
        }
    }
    private IEnumerator DelayCheck(NetworkObject netObj)
    {
        ulong clientId = netObj.OwnerClientId; // Get the actual ClientId from the collider

        yield return new WaitForSeconds(0.5f);
        if (PlayerManager.IsPlayerInBattle(clientId))
        {
            yield break;
        }
        yield return new WaitForSeconds(0.1f);
        if (battleStarterClientId != clientId)
        {
            // Inform the client to join the battle and load the Dingo data
            StartBattleOnClient(clientId);

            PlayerManager.SetPlayerBattleStatus(clientId, true);
            activePlayers.Add(clientId);
        }
        yield return null;
    }
    private void StartBattleOnClient(ulong clientId)
    {
        Debug.Log($"[Server] Starting battle on client {clientId}");
        StartBattleClientRpc(clientId);
    }
    [ClientRpc]
    private void StartBattleClientRpc(ulong clientId)
    {
        Debug.Log($"[Client] Received StartBattleClientRpc for client {clientId}");
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            Debug.Log("[Client] Loading Dingo data from file...");
            string playerDingoData = DingoLoader.LoadPlayerDingoFromFileToSend(); // Load client data from slot 0
            if (string.IsNullOrEmpty(playerDingoData))
            {
                Debug.LogError("[Client] Failed to load Dingo data from file.");
                return;
            }
            Debug.Log("[Client] Sending Dingo data to server...");
            SendDingoDataToServer(playerDingoData); // Send the loaded data to the server
            SetAttackMovesPlayer2();
        }
    }
    public void SetAttackMoves(int slotNumber)
    {
        StartCoroutine(DelayAttackButtons(slotNumber));
    }
    public IEnumerator DelayAttackButtons(int slotNumber)
    {
        yield return new WaitForSeconds(0.3f);
        Button[] moveButtons = BattleUI.GetMoveButtons();
        int[] moves = DingoLoader.LoadDingoMovesToSend(0);

        SetMoveButton(moveButtons[0], slotNumber, moves[0]);
        SetMoveButton(moveButtons[1], slotNumber, moves[1]);
        SetMoveButton(moveButtons[2], slotNumber, moves[2]);
        SetMoveButton(moveButtons[3], slotNumber, moves[3]);

    }
    private void SetMoveButton(Button button, int slotNumber, int moveId)
    {
        Text buttonText = button.GetComponentInChildren<Text>();
        GameObject targetEntity = DecodeSlotNumber(slotNumber);
        if (targetEntity == null)
        {
            Debug.LogWarning($"No entity found in slot {slotNumber}.");
            return;
        }

        // Ensure the target entity has a NetworkDingo component
        NetworkDingo networkDingo = targetEntity.GetComponent<NetworkDingo>();
        if (networkDingo == null)
        {
            Debug.LogWarning("Target entity has no NetworkDingo component.");
            return;
        }

        DingoID dingo = DingoDatabase.GetDingoByID(networkDingo.id.Value);
        DingoMove move = DingoDatabase.GetMoveByID(moveId, dingo);
        if (buttonText != null)
        {
            buttonText.text = move.Name;
        }
        else
        {
            Debug.LogWarning("No Text component found in the button!");
        }

        // Clear existing listeners before adding new ones to prevent multiple calls
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            ShowTargetSelectionUI(moveId, slotNumber); // Open UI for choosing a target
        });
        //button.onClick.AddListener(() => RequestMoveServerRPC(slotNumber, moveId));
    }
    public void SetAttackMovesPlayer2()
    {
        StartCoroutine(DelayAttackButtonsPlayer2());
    }
    public IEnumerator DelayAttackButtonsPlayer2()
    {
        yield return new WaitForSeconds(0.3f);
        Button[] moveButtons = BattleUI.GetMoveButtons();
        DingoID dingo = DingoLoader.LoadPlayerDingoFromFile(0);
        int[] moves = DingoLoader.LoadDingoMovesToSend(0);
        if (dingo == null)
        {
            Debug.LogError("[Client] Failed to load Dingo data from file!");
            yield break;
        }
        SetMoveButtonPlayer2(moveButtons[0], dingo.ID, moves[0]);
        SetMoveButtonPlayer2(moveButtons[1], dingo.ID, moves[1]);
        SetMoveButtonPlayer2(moveButtons[2], dingo.ID, moves[2]);
        SetMoveButtonPlayer2(moveButtons[3], dingo.ID, moves[3]);

    }


    private void SetMoveButtonPlayer2(Button button, int dingoId, int moveId)
    {
        DingoMove move = DingoDatabase.GetMoveByID(moveId, DingoDatabase.GetDingoByID(dingoId));

        Text buttonText = button.GetComponentInChildren<Text>();


        if (buttonText != null && move != null)
        {
            buttonText.text = move.Name;
        }
        else
        {
            Debug.LogWarning("No Text component found in the button!");
        }

        // Clear existing listeners before adding new ones to prevent multiple calls
        button.onClick.RemoveAllListeners();
        //button.onClick.AddListener(() => RequestMoveServerRPC(2, moveId));
        button.onClick.AddListener(() =>
        {
            ShowTargetSelectionUI(moveId, 2); // Open UI for choosing a target
        });
    }
    private void SendDingoDataToServer(string playerDingoData)
    {
        Debug.Log("[Client] Sending Dingo data to server via ServerRpc...");
        OnReceiveDingoDataServerRpc(playerDingoData);
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnReceiveDingoDataServerRpc(string playerDingoString)
    {
        Debug.Log("[Server] Received Dingo data from client.");
        if (string.IsNullOrEmpty(playerDingoString))
        {
            Debug.LogError("[Server] Received empty Dingo data from client.");
            return;
        }

        Debug.Log("[Server] Loading Dingo data from string...");
        DingoID playerDingoData = DingoLoader.LoadPlayerDingoFromFileToRecieve(playerDingoString, 0);
        int[] moves = DingoLoader.LoadDingoMovesToSend(0);
        if (playerDingoData == null)
        {
            Debug.LogError("[Server] Failed to load Dingo data from string.");
            return;
        }

        string playerSpritePath = playerDingoData.Sprite; // Assuming the sprite path is stored in the DingoID object
        Debug.Log($"[Server] Requesting Dingo spawn with sprite path: {playerSpritePath}");

        BattleManagerUtils.RequestDingoSpawn(playerSpritePath, (GameObject playerDingo) =>
        {
            NetworkDingo networkDingo = playerDingo.GetComponent<NetworkDingo>();
            if (networkDingo != null)
            {
                networkDingo.SetDingoAttributesServerRpc(
                    playerDingoData.ID, playerDingoData.Sprite, playerDingoData.Name, playerDingoData.Type,
                    playerDingoData.HP, playerDingoData.Attack, playerDingoData.Defense,
                    playerDingoData.Speed, playerDingoData.MaxHP, playerDingoData.XP,
                    playerDingoData.MaxXP, playerDingoData.Level, moves[0], moves[1], moves[2], moves[3]);
            }

            Debug.Log($"[Server] Dingo spawned at position {Slot2.transform.position}");
            StartCoroutine(DelayedAssign(playerDingo, Slot2));
        });
    }
    public void SetBattleStarter(ulong clientId)
    {
        battleStarterClientId = clientId;
    }
    private IEnumerator DelayCollider2D()
    {
        Collider2D selfcollider = GetComponent<Collider2D>();
        yield return new WaitForSeconds(1.5f);
        selfcollider.enabled = true;
        yield return null;
    }
    public bool IsPositionOccupied(GameObject position)
    {
        return positionOccupied.ContainsKey(position) && positionOccupied[position];
    }
    private void DebugPrintEntityPositions(string context)
    {
        Debug.Log($"[Debug] --- {context} ---");
        foreach (var kvp in entityInPosition)
        {
            string entityName = kvp.Value != null ? kvp.Value.name : "NULL";
            Debug.Log($"[Debug] Position {kvp.Key.name}: {entityName}");
        }
    }

    public void AssignEntityToPosition(GameObject entity, GameObject position)
    {
        if (entity == null || position == null)
        {
            Debug.LogError("[AssignEntityToPosition] Entity or position is null!");
            return;
        }

        DebugPrintEntityPositions("Before Assigning");

        if (entityInPosition.TryGetValue(position, out var currentEntity) && currentEntity != null)
        {
            Debug.LogWarning($"[AssignEntityToPosition] Position {position.name} is already occupied by {currentEntity.name}. Removing old entity...");
            RemoveDingoByEntity(currentEntity);
        }

        entityInPosition[position] = entity;
        positionOccupied[position] = true;

        DebugPrintEntityPositions("After Assigning");

        StartCoroutine(AssignAnimation(entity, position));

        Debug.Log($"[AssignEntityToPosition] Successfully assigned {entity.name} to {position.name}");
    }


    public void AssignEntityToPosition2(GameObject entity, int position2)
    {
        GameObject position = null;
        if (position2 == 3)
        {
            position = Opponent1;
        }
        else if (position2 == 4) 
        {
            position = Opponent2;
        }

        if (!entityInPosition.TryGetValue(position, out var currentEntity))
        {
            entityInPosition[position] = null;  // Initialize if not already present
            currentEntity = null;
        }

        if (currentEntity != null)
        {
            Debug.LogWarning($"Position {position.name} is already occupied by {currentEntity.name}.");
            return;
        }


        SpriteRenderer spriteRenderer = entity.GetComponent<SpriteRenderer>();
        NetworkDingo networkDingo = entity.GetComponent<NetworkDingo>();

        if (spriteRenderer != null && (position.name == "Opponent1" || position.name == "Opponent2"))
        {
            spriteRenderer.flipX = true;
            networkDingo.isFlipped.Value = true;
        }
        else
        {
            networkDingo.isFlipped.Value = false;
        }

        StartCoroutine(AssignAnimation(entity, position));
        entityInPosition[position] = entity;
        positionOccupied[position] = true;
    }

    public void UnassignEntityFromPosition(GameObject position)
    {
        if (entityInPosition.ContainsKey(position) && entityInPosition[position] != null)
        {
            Debug.Log($"Removing {entityInPosition[position].name} from {position.name}");
            entityInPosition[position] = null;
            positionOccupied[position] = false; // Mark the position as occupied

        }
        else
        {
            Debug.LogWarning($"Attempted to remove entity from {position.name}, but it was already empty.");
        }
    }
    public IEnumerator AssignAnimation(GameObject entity, GameObject position)
    {
        Vector3 startPos = entity.transform.position;
        Vector3 endPos = position.transform.position;
        float timeToMove = 0.3f;  // Duration of the animation
        float elapsedTime = 0f;

        // Smoothly move the entity to the new position over 0.3 seconds
        while (elapsedTime < timeToMove)
        {
            entity.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the entity is exactly at the target position at the end
        entity.transform.position = endPos;
        dingoSpawning = false;
    }
    public IEnumerator DelayedAssign(GameObject entity, GameObject position)
    {
        yield return new WaitForSeconds(0.15f); // Wait until the next frame to ensure the object is spawned

        if (entity == null || position == null)
        {
            Debug.LogError("[DelayedAssign] Entity or position is null!");
            yield break;
        }

        if (entityInPosition.ContainsKey(position) && entityInPosition[position] != null)
        {
            Debug.LogWarning($"[DelayedAssign] {position.name} is already occupied by {entityInPosition[position].name}. Skipping reassignment!");
            yield break;
        }

        AssignEntityToPosition(entity, position);
    }

    public void ClearPosition(GameObject position)
    {
        if (positionOccupied.ContainsKey(position))
        {
            positionOccupied[position] = false;
        }
    }
}
