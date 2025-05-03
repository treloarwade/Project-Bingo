using DingoSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WatterBattle : NetworkBehaviour
{
    private float lastActivationTime;

    private void Start()
    {
        lastActivationTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && collider.GetComponent<NetworkObject>().IsOwner)
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
    }

    private void StartBattle()
    {
        if (BattleStarter.Instance != null)
        {
            string filePath = DingoLoader.LoadPlayerDingoFromFileToSend();
            string agentBingoPath = DingoLoader.LoadPlayerDataFromFileToSend();
            // Call the RequestStartBattle method on the instance
            BattleStarter.Instance.RequestStartBattle(NetworkManager.Singleton.LocalClientId, 0, transform.position, filePath, agentBingoPath);
        }
        else
        {
            Debug.LogError("[StartBattle] BattleStarter instance not found in the scene.");
        }
    }
}
