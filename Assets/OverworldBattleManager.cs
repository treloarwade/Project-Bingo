using UnityEngine;
using System.Collections.Generic;
using DingoSystem;
using Unity.Netcode;
using System.Collections;
using System;
using UnityEditor.PackageManager;

public class OverworldBattleManager : NetworkBehaviour
{
    [Header("Player Positions")]
    public GameObject Slot1;  // Player 1
    public GameObject Slot2;  // Player 2 (or AI ally)
    public GameObject TrainerPosition1;  // Trainer for Slot1
    public GameObject TrainerPosition2;  // Trainer for Slot2

    [Header("Opponent Positions")]
    public GameObject Opponent1;  // Enemy 1
    public GameObject Opponent2;  // Enemy 2
    public GameObject OpponentTrainerPosition1;  // Trainer for Opponent1
    public GameObject OpponentTrainerPosition2;  // Trainer for Opponent2
    private Dictionary<GameObject, GameObject> entityInPosition = new Dictionary<GameObject, GameObject>();

    private Dictionary<GameObject, bool> positionOccupied = new Dictionary<GameObject, bool>();
    private ulong battleStarterClientId = 99; // Track the clientId of the player who initiated the battle
    private Camera mainCamera;
    private HashSet<ulong> activePlayers = new HashSet<ulong>();
    private void Awake()
    {
        InitializePositions();
        StartCoroutine(DelayCollider2D());
        mainCamera = Camera.main;
        SetCameraToObjectPosition();
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
    public void HitButton()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Attack(Opponent1, 50);
            Attack(Opponent2, 100);
        }
        else
        {
            RequestAttackServerRpc(3);
        }

    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestAttackServerRpc(int targetPosition)
    {
        // Based on the target position, attack the corresponding opponent
        if (targetPosition == 1)
        {
            Attack(Opponent1, 1); // Attack Opponent1
        }
        else if (targetPosition == 2)
        {
            Attack(Opponent2, 1); // Attack Opponent2
        }
        else if (targetPosition == 3)
        {
            Attack(Opponent1, 25); // Attack both targets, or however you want to handle this case
            Attack(Opponent2, 25);
        }
    }
    public void Attack(GameObject position, int damage)
    {
        if (entityInPosition.ContainsKey(position) && entityInPosition[position] != null)
        {
            GameObject targetEntity = entityInPosition[position];
            NetworkDingo dingo = targetEntity.GetComponent<NetworkDingo>();

            if (dingo != null)
            {
                dingo.hp.Value -= damage; // Correct way to modify a NetworkVariable<int>
                Debug.Log($"{targetEntity.name} was attacked! New health: {dingo.hp.Value}");
                if (dingo.hp.Value <= 0)
                {
                    RemoveDingoClientRpc(targetEntity.GetComponent<NetworkObject>().NetworkObjectId);
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
                        RemoveDingoClientRpc(targetEntity.GetComponent<NetworkObject>().NetworkObjectId);
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
    public void DingoMoveButton()
    {
    }
    [ClientRpc]
    public void RemoveDingoClientRpc(ulong networkObjectId)
    {
        // Try to find the NetworkObject with this id in the local SpawnedObjects
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject netObj))
        {
            // Start the removal animation on this client
            StartCoroutine(RemoveDingoFromBattle(netObj.gameObject));
        }
        else
        {
            Debug.LogWarning($"Could not find NetworkObject with id {networkObjectId} on client.");
        }
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

        yield return new WaitForSeconds(0.2f); // Small delay before removal
                                               // Remove from entity tracking
        foreach (var position in entityInPosition.Keys)
        {
            if (entityInPosition[position] == dingoObject)
            {
                entityInPosition[position] = null;
                positionOccupied[position] = false; // Mark the position as occupied
                break; // Stop looping once we've found the correct position
            }
        }
        if (!IsPositionOccupied(Opponent1) && !IsPositionOccupied(Opponent2))
        {
            BattleEnd(); // End the battle if both positions are empty
            Destroy(dingoObject); // Destroy the Dingo object
        }
    }
    private void BattleEnd()
    {
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
        }
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
                    playerDingoData.Sprite, playerDingoData.Name, playerDingoData.Type,
                    playerDingoData.HP, playerDingoData.Attack, playerDingoData.Defense,
                    playerDingoData.Speed, playerDingoData.MaxHP, playerDingoData.XP,
                    playerDingoData.MaxXP, playerDingoData.Level);
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

        // Initialize positionOccupied dictionary
        positionOccupied[Slot1] = false;
        positionOccupied[Slot2] = false;
        positionOccupied[TrainerPosition1] = false;
        positionOccupied[TrainerPosition2] = false;
        positionOccupied[Opponent1] = false;
        positionOccupied[Opponent2] = false;
        positionOccupied[OpponentTrainerPosition1] = false;
        positionOccupied[OpponentTrainerPosition2] = false;
    }
    private GameObject FindChildByName(string parentName, string childName)
    {
        // Find the parent GameObject first
        Transform parentTransform = transform.Find(parentName);
        if (parentTransform != null)
        {
            // Find the child GameObject under the parent
            Transform childTransform = parentTransform.Find(childName);
            if (childTransform != null)
            {
                return childTransform.gameObject; // Return the GameObject if found
            }
            else
            {
                Debug.LogError($"Child with name {childName} not found under parent {parentName}!");
            }
        }
        else
        {
            Debug.LogError($"Parent with name {parentName} not found!");
        }
        return null; // Return null if not found
    }
    public bool IsPositionOccupied(GameObject position)
    {
        return positionOccupied.ContainsKey(position) && positionOccupied[position];
    }
    public void AssignEntityToPosition(GameObject entity, GameObject position)
    {
        Debug.Log($"Attempting to assign {entity.name} to {position.name}");

        if (!entityInPosition.ContainsKey(position))
        {
            entityInPosition[position] = null;
        }

        if (entityInPosition[position] != null)
        {
            Debug.LogError($"Attempted to assign {entity.name} to {position.name}, but it's already occupied by {entityInPosition[position].name}!");
            return;
        }
        SpriteRenderer spriteRenderer = entity.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && (position.name == "Opponent1" || position.name == "Opponent2"))
        {
            spriteRenderer.flipX = true;
        }

        StartCoroutine(AssignAnimation(entity, position));
        entityInPosition[position] = entity;
        positionOccupied[position] = true; // Mark the position as occupied
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
    }
    public IEnumerator DelayedAssign(GameObject entity, GameObject position)
    {
        yield return new WaitForSeconds(0.1f); // Wait until the next frame to ensure the object is spawned

        Debug.Log($"Delayed assignment: {entity.name} to {position.name}");

        if (entity != null && position != null)
        {
            AssignEntityToPosition(entity, position);
        }
        else
        {
            Debug.LogError("Entity or position is null after delay!");
        }
    }
    public void ClearPosition(GameObject position)
    {
        if (positionOccupied.ContainsKey(position))
        {
            positionOccupied[position] = false;
        }
    }
}
