using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TrainerScript : NetworkBehaviour
{
    private const int TRAINER_ID = 6; // Replace magic number with constant
    private const string TRAINER_SPRITE_PATH = "Characters/man1";

    private float lastActivationTime;
    private int interactionCount = 0;
    private SpriteRenderer spriteRenderer;
    public Interactor interactor;
    private bool battleCompleted = false;
    private bool playerWon = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        BattleStarter.OnBattleEnd += HandleBattleEnd;
    }

    public override void OnDestroy()
    {
        BattleStarter.OnBattleEnd -= HandleBattleEnd;
        base.OnDestroy();
    }

    private void HandleBattleEnd(ulong clientId, bool won)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            battleCompleted = true;
            playerWon = won;
        }
    }

    public void Talk()
    {
        StartCoroutine(HandleConversation());
    }

    private void StartBattle()
    {
        if (BattleStarter.Instance == null)
        {
            Debug.LogError("[StartBattle] BattleStarter instance not found in the scene.");
            return;
        }

        try
        {
            battleCompleted = false;
            string filePath = DingoLoader.LoadPlayerDingoFromFileToSend();
            string agentBingoPath = DingoLoader.LoadPlayerDataFromFileToSend();

            BattleStarter.Instance.RequestStartBattle(
                NetworkManager.Singleton.LocalClientId,
                TRAINER_ID,
                transform.position,
                filePath,
                agentBingoPath,
                true,
                TRAINER_SPRITE_PATH
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to start battle: {e.Message}");
        }
    }

    public void ContinueConversation()
    {
        if (interactionCount == 2)
        {
            ExitConversation();
        }
        else
        {
            StartCoroutine(HandleConversation());
        }
    }

    public void BattleAgain()
    {
        StartBattle();
    }

    public void ExitConversation()
    {
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.CloseDialogShopScreen();
            if (interactor != null && interactor.IsPlayerInRange())
            {
                // Small delay to ensure dialog is fully closed
                StartCoroutine(DelayedTurnOnInteractor());
            }
        }
    }
    private IEnumerator DelayedTurnOnInteractor()
    {
        yield return new WaitForEndOfFrame(); // Wait one frame
        if (interactor != null && !DialogManager.Instance.IsDialogActive())
        {
            interactor.TurnOn();
        }
    }
    private IEnumerator HandleConversation()
    {
        yield return new WaitForSeconds(0.1f);
        lastActivationTime = Time.time;

        if (DialogManager.Instance == null)
        {
            Debug.LogError("DialogManager instance not found!");
            yield break;
        }

        string dialog;
        DialogManager.Instance.ClearDialogButtons();

        if (battleCompleted)
        {
            // Post-battle dialog
            dialog = playerWon
                ? "Nice Trainer: You're strong! Let's battle again sometime!"
                : "Nice Trainer: Better luck next time!";

            if (!playerWon)
            {
                DialogManager.Instance.DisplayDialogButton("Battle Again", BattleAgain);
            }
        }
        else
        {
            // Pre-battle dialog
            interactionCount++;
            dialog = GetPreBattleDialog(interactionCount);

            if (interactionCount == 2)
            {
                StartBattle();
            }
        }

        DialogManager.Instance.DisplayDialogIsExitable(false, dialog);
        DialogManager.Instance.DisplayDialogButton("Continue", ContinueConversation);
        DialogManager.Instance.DisplayDialogButton("Exit", ExitConversation);

        if (interactor != null)
        {
            interactor.TurnOff();
        }
    }

    private string GetPreBattleDialog(int count)
    {
        switch (count)
        {
            case 1: return "Nice Trainer: Let's battle.";
            case 2: return "Nice Trainer: Nice Battle";
            default: return "Nice Trainer: Wow nice battle.";
        }
    }
}