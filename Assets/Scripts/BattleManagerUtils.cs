using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;
using Unity.Collections;

public class BattleManagerUtils : NetworkBehaviour
{
    public static BattleManagerUtils Instance;
    private Dictionary<ulong, GameObject> spawnedDingos = new Dictionary<ulong, GameObject>();

    public static event Action<GameObject> OnDingoSpawned;

    private void Awake()
    {
        Instance = this;
    }

    [ServerRpc]
    public void SpawnDingoServerRpc(ulong clientId, string dingoSpritePath)
    {
        if (!IsServer) return;

        GameObject dingoInstance = Instantiate(DingoLoader.dingoPrefab);
        if (dingoInstance == null)
        {
            Debug.LogError("[ServerRpc] Failed to instantiate DingoPrefab!");
            return;
        }

        NetworkObject networkObject = dingoInstance.GetComponent<NetworkObject>();
        if (networkObject == null)
        {
            Debug.LogError("[ServerRpc] DingoPrefab does not have a NetworkObject component!");
            return;
        }

        networkObject.SpawnWithOwnership(clientId);

        Debug.Log($"[ServerRpc] Spawned DingoPrefab with ID: {networkObject.NetworkObjectId}");

        // Notify the client that the Dingo has been spawned
        NotifyDingoSpawnedClientRpc(networkObject.NetworkObjectId, dingoSpritePath);
    }

    [ClientRpc]
    private void NotifyDingoSpawnedClientRpc(ulong dingoId, string dingoSpritePath)
    {
        if (!NetworkManager.Singleton.IsClient) return;

        Debug.Log($"[ClientRpc] Received spawn notification for Dingo ID {dingoId} with sprite path: {dingoSpritePath}");

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(dingoId, out NetworkObject networkObject))
        {
            GameObject dingoObject = networkObject.gameObject;
            if (dingoObject != null)
            {
                // Assign sprite path to the NetworkDingo component
                NetworkDingo networkDingo = dingoObject.GetComponent<NetworkDingo>();
                if (networkDingo != null)
                {
                    Debug.Log($"[ClientRpc] Assigning sprite path to {dingoObject.name}");
                    networkDingo.UpdateSprite(dingoSpritePath); // Ensure server sets the sprite path
                }
                else
                {
                    Debug.LogError("[ClientRpc] Spawned Dingo does not have a NetworkDingo component!");
                }

                OnDingoSpawned?.Invoke(dingoObject);
            }
        }
        else
        {
            Debug.LogError($"[ClientRpc] Failed to find spawned Dingo with ID {dingoId}");
        }
    }



    public static void RequestDingoSpawn(string dingoSpritePath, Action<GameObject> onSpawnedCallback)
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient)
        {
            return;
        }


        if (Instance != null && Instance.IsClient)
        {
            OnDingoSpawned += onSpawnedCallback;
            Instance.SpawnDingoServerRpc(NetworkManager.Singleton.LocalClientId, dingoSpritePath);
        }
        else
        {
        }
    }
}
