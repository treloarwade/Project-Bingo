using DingoSystem;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class GrassBattle : NetworkBehaviour
{
#pragma warning disable CS0414 // The field 'GrassBattle.isWiggling' is assigned but its value is never used
    private bool isWiggling;
#pragma warning restore CS0414 // The field 'GrassBattle.isWiggling' is assigned but its value is never used
    private float wiggleDuration = 0.5f;
    private float maxWiggleAngle = 10f;
    public List<DingoID> dingos = new List<DingoID>();
    private float lastActivationTime;
    public string filePath;
    public string jsonData;
    private JSONArray jsonDingos;
    public GameObject battlePrefab;
    private void Start()
    {
        lastActivationTime = Time.time;
        dingos = new List<DingoID>(DingoDatabase.allDingos);
    }
    public void SaveCoordinates()
    {
        // Save the position and rotation of the object
        PlayerPrefs.SetFloat("PosX", transform.position.x);
        PlayerPrefs.SetFloat("PosY", transform.position.y);
        PlayerPrefs.SetFloat("PosZ", transform.position.z);

        // Save PlayerPrefs to disk
        PlayerPrefs.Save();

        Debug.Log("Coordinates saved.");
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isWiggling && collider.CompareTag("Player") && collider.GetComponent<NetworkObject>().IsOwner)
        {
            int randomNumber = Random.Range(0, 10);
            if (randomNumber < 1)
            {
                if (Time.time - lastActivationTime >= 3f)
                {
                    lastActivationTime = Time.time; // Update activation time

                    // Call a separate function to handle the battle setup
                    StartBattle();
                }
            }
        }
        else
        {
            StartCoroutine(WiggleGrass());
        }
    }
    private void StartBattle()
    {
        // Only host/server can initiate the battle logic here
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Host is starting a battle.");
            SpawnBattleOnServer(NetworkManager.Singleton.LocalClientId); // Server directly handles spawning
        }
        else
        {
            // Client requests the server to start the battle
            Debug.Log("Client is requesting a battle start.");
            RequestBattleStartServerRpc(NetworkManager.Singleton.LocalClientId); // Client sends ServerRpc
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestBattleStartServerRpc(ulong clientId)
    {
        // Server is receiving the request to spawn the battle
        Debug.Log($"[Server] Client {clientId} requested to start a battle.");

        // Call server method to spawn the battle
        SpawnBattleOnServer(clientId); // This must be handled ONLY by the server
    }
    private void SpawnBattleOnServer(ulong clientId)
    {
        // Ensure battle prefab is not already present in the scene
        if (FindObjectOfType<OverworldBattleManager>() == null)
        {
            Debug.Log("[Server] Starting battle spawn...");

            // Find all BattleSpot GameObjects in the scene using a specific tag
            GameObject[] battleSpots = GameObject.FindGameObjectsWithTag("BattleSpot");

            // Check if there are any BattleSpots
            if (battleSpots.Length > 0)
            {
                // Calculate the closest BattleSpot
                GameObject closestSpot = null;
                float minDistance = float.MaxValue;

                foreach (GameObject spot in battleSpots)
                {
                    // Calculate distance to the spot (assuming 'transform.position' is the reference point)
                    float distance = Vector3.Distance(spot.transform.position, transform.position); // You can replace 'transform.position' with another reference point if needed

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestSpot = spot;
                    }
                }

                // Instantiate the battle prefab at the closest BattleSpot position
                GameObject battleInstance = Instantiate(DingoLoader.battlePrefab, closestSpot.transform.position, Quaternion.identity);
                NetworkObject networkObject = battleInstance.GetComponent<NetworkObject>();

                // Check if the prefab has a NetworkObject to spawn it correctly
                if (networkObject != null)
                {
                    // Spawn battle on the network (networking done server-side)
                    networkObject.Spawn(); // This happens only on the server side

                    OverworldBattleManager battleManager = battleInstance.GetComponent<OverworldBattleManager>();

                    if (battleManager != null)
                    {
                        Debug.Log("[Server] Battle prefab spawned successfully.");
                        DingoID enemyDingoData1 = GetRandomDingo();
                        DingoID enemyDingoData2 = GetRandomDingo();
                        DingoID playerDingoData = DingoLoader.LoadPlayerDingoFromFile(0);
                        int[] moves = DingoLoader.LoadDingoMovesToSend(0);
                        int[] enemyMoves1 = { 0, 1, 2, 3 };
                        int[] enemyMoves2 = { 0, 1, 2, 3 };



                        SpawnAndAssignDingo(moves, playerDingoData, battleManager.Slot1);
                        SpawnAndAssignDingo(enemyMoves1, enemyDingoData1, battleManager.Opponent1);
                        SpawnAndAssignDingo(enemyMoves2, enemyDingoData2, battleManager.Opponent2);

                        battleManager.SetAttackMoves(1);
                        // Set the player who started the battle (only on server)
                        battleManager.SetBattleStarter(clientId);
                        PlayerManager.SetPlayerBattleStatus(clientId, true);
                        Debug.Log($"[Server] Battle started by Player {clientId}");
                    }
                    else
                    {
                        Debug.LogError("[Server] BattleManager not found in the battle prefab.");
                    }
                }
                else
                {
                    Debug.LogError("[Server] NetworkObject not found on battle prefab.");
                }
            }
            else
            {
                Debug.LogError("[Server] No BattleSpots found in the scene.");
            }
        }
        else
        {
            // If battle prefab is already in the scene
            Debug.Log("[Server] Battle prefab already exists.");
        }
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

            StartCoroutine(FindObjectOfType<OverworldBattleManager>().DelayedAssign(dingoObject, slot));
        });
    }
    public DingoID GetRandomDingo()
    {
        if (dingos.Count == 0)
        {
            Debug.LogWarning("No Dingos available to select.");
            return null; // Handle empty list case
        }

        int randomIndex = Random.Range(0, dingos.Count);
        return dingos[randomIndex];
    }
    private IEnumerator WiggleGrass()
    {
        isWiggling = true; // Set wiggling flag to true

        float startTime = Time.time; // Record the start time
        Quaternion originalRotation = transform.rotation; // Store the original rotation

        // Randomly select the direction of the initial wiggle
        float direction = Random.Range(0, 2) == 0 ? -1f : 1f;

        while (Time.time - startTime < wiggleDuration)
        {
            float t = (Time.time - startTime) / wiggleDuration; // Calculate the interpolation parameter

            // Calculate the angle to rotate using a smooth oscillating motion
            float smoothAngle = Mathf.Lerp(-1f, 1f, Mathf.Sin(t * Mathf.PI));
            float angle = Mathf.Lerp(-maxWiggleAngle, maxWiggleAngle, smoothAngle * direction);

            transform.rotation = originalRotation * Quaternion.Euler(0f, 0f, angle); // Rotate the grass object
            yield return null; // Wait for the next frame
        }

        transform.rotation = originalRotation; // Reset rotation when wiggling is done
        isWiggling = false; // Set wiggling flag to false
    }
}
