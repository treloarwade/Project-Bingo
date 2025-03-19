using DingoSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GrassBattle : NetworkBehaviour
{
    private bool isWiggling;
    private float wiggleDuration = 0.5f;
    private float maxWiggleAngle = 10f;
    public List<DingoID> dingos = new List<DingoID>();
    private float lastActivationTime;

    private void Start()
    {
        lastActivationTime = Time.time;
        dingos = new List<DingoID>(DingoDatabase.allDingos); // Assume DingoDatabase contains all available Dingos
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
        if (BattleStarter.Instance != null)
        {
            // Call the RequestStartBattle method on the instance
            BattleStarter.Instance.RequestStartBattle(NetworkManager.Singleton.LocalClientId, 0, transform.position);
        }
        else
        {
            Debug.LogError("[StartBattle] BattleStarter instance not found in the scene.");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestBattleStartServerRpc(ulong clientId, Vector3 triggerPosition)
    {
        // Log the request from the client
        Debug.Log($"[Server] Client {clientId} requested battle start.");

        // Ensure there are enough Dingos (2 for the player, 2 for the opponent)
        if (dingos.Count < 4)
        {
            Debug.LogError("Not enough Dingos available for the battle. Need 4 Dingos (2 for the player, 2 for the opponent).");
            return;
        }

        // Start battle using BattleHandler and pass the list of opponent Dingos



        Debug.Log("Battle started successfully!");
    }


    // Select a random Dingo from the list
    public DingoID GetRandomDingo()
    {
        if (dingos.Count == 0)
        {
            Debug.LogWarning("No Dingos available to select.");
            return null;
        }

        int randomIndex = Random.Range(0, dingos.Count);
        return dingos[randomIndex];
    }

    private GameObject FindClosestBattleSpot(Vector3 position)
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

    private IEnumerator WiggleGrass()
    {
        isWiggling = true;
        float startTime = Time.time;
        Quaternion originalRotation = transform.rotation;
        float direction = Random.Range(0, 2) == 0 ? -1f : 1f;

        while (Time.time - startTime < wiggleDuration)
        {
            float t = (Time.time - startTime) / wiggleDuration;
            float smoothAngle = Mathf.Lerp(-1f, 1f, Mathf.Sin(t * Mathf.PI));
            float angle = Mathf.Lerp(-maxWiggleAngle, maxWiggleAngle, smoothAngle * direction);

            transform.rotation = originalRotation * Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }

        transform.rotation = originalRotation;
        isWiggling = false;
    }
}
