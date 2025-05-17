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
    private float lastActivationTime;

    private void Start()
    {
        lastActivationTime = Time.time;
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
            string filePath = DingoLoader.LoadPlayerDingoFromFileToSend();
            string agentBingoPath = DingoLoader.LoadPlayerDataFromFileToSend();
            // Call the RequestStartBattle method on the instance
            BattleStarter.Instance.RequestStartBattle(NetworkManager.Singleton.LocalClientId, 0, transform.position, filePath, agentBingoPath, false, -1);
        }
        else
        {
            Debug.LogError("[StartBattle] BattleStarter instance not found in the scene.");
        }
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
