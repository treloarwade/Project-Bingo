using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class JerkScript : MonoBehaviour
{
    public Sprite[] frames;
    private float lastActivationTime;
    private int interactionCount = 0;
    private SpriteRenderer spriteRenderer;
    public Interactor interactor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Talk()
    {
        StartCoroutine(Bingo());
    }
    private void StartBattle()
    {
        if (BattleStarter.Instance != null)
        {
            string filePath = DingoLoader.LoadPlayerDingoFromFileToSend();
            string agentBingoPath = DingoLoader.LoadPlayerDataFromFileToSend();
            // Call the RequestStartBattle method on the instance
            BattleStarter.Instance.RequestStartBattle(NetworkManager.Singleton.LocalClientId, 2, transform.position, filePath, agentBingoPath, true, "Characters/jerk");
        }
        else
        {
            Debug.LogError("[StartBattle] BattleStarter instance not found in the scene.");
        }
    }

    IEnumerator Bingo()
    {
        if (!DialogManager.Instance.IsDialogActive())
        {
            yield return new WaitForSeconds(0.1f);
            lastActivationTime = Time.time;
            string dialog;
            // Increment the interaction count
            interactionCount++;

            // Set dialog based on interaction count
            switch (interactionCount)
            {
                case 1:
                    dialog = "Rich Jerk: I guess I have to teach you a lesson in Finance scrub.";
                    break;
                case 2:
                    dialog = "Rich Jerk: You think that's funny? My dad works at company, I can get you banned.";
                    spriteRenderer.sprite = frames[0];
                    StartBattle();
                    break;
                case 3:
                    dialog = "Rich Jerk: Get out of here before I tell my dad to ban you.";
                    spriteRenderer.sprite = frames[1];
                    break;
                default:
                    // If the player interacts more than three times, reset the interaction count
                    dialog = "Rich Jerk: Get out of here before I tell my dad to ban you.";
                    break;
            }
            DialogManager.Instance.DisplayDialogIsExitable(false, dialog);
            DialogManager.Instance.ClearDialogButtons();
            DialogManager.Instance.DisplayDialogButton("Cool", ContinueConversation);
            DialogManager.Instance.DisplayDialogButton("Nice", ExitConversation);
            interactor.TurnOff();
        }

    }
    public void ContinueConversation()
    {
        StartCoroutine(Bingo());
    }

    public void ExitConversation()
    {
        interactionCount = 0; // Reset if you want exiting to restart the conversation next time
        DialogManager.Instance.CloseDialogShopScreen();
    }
}


