using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static BattleStarter;

public class TrainerScript : NetworkBehaviour
{
    private const int TRAINER_LIST = 6; // Replace magic number with constant
    private const int TRAINER_ID = 0;

    private float lastActivationTime;
    private int interactionCount = 0;
    private SpriteRenderer spriteRenderer;
    public Interactor interactor;
    private bool playerWon = false;
    public NetworkVariable<bool> playerWonBattle = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> battleCompleted = new NetworkVariable<bool>(false);

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        BattleStarter.OnBattleEnd += HandleBattleEnd;
        playerWonBattle.OnValueChanged += OnPlayerWonChanged;
    }

    public override void OnDestroy()
    {
        BattleStarter.OnBattleEnd -= HandleBattleEnd;
        playerWonBattle.OnValueChanged -= OnPlayerWonChanged;
        base.OnDestroy();
    }
    private void OnPlayerWonChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
        }
    }
    private void HandleBattleEnd(object sender, BattleEndEventArgs e)
    {
        if (e.TrainerId == TRAINER_ID && IsServer)
        {
            playerWonBattle.Value = e.Won;
            battleCompleted.Value = true;
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
            battleCompleted.Value = false;
            string filePath = DingoLoader.LoadPlayerDingoFromFileToSend();
            string agentBingoPath = DingoLoader.LoadPlayerDataFromFileToSend();

            BattleStarter.Instance.RequestStartBattle(
                NetworkManager.Singleton.LocalClientId,
                TRAINER_LIST,
                transform.position,
                filePath,
                agentBingoPath,
                true,
                TRAINER_ID
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to start battle: {e.Message}");
        }
    }

    public void ContinueConversation()
    {
        if (interactionCount == 1)
        {
            ExitConversation();
        }
        else
        {
            interactionCount++;
            StartCoroutine(HandleConversation());
        }
    }

    public void BattleAgain()
    {
        StartBattle();
        ExitConversation();
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

        if (battleCompleted.Value)
        {
            // Post-battle dialog
            dialog = playerWonBattle.Value
                ? "Nice Trainer: You're strong! Let's battle again sometime!"
                : "Nice Trainer: Better luck next time!";

            if (!playerWonBattle.Value)
            {
                DialogManager.Instance.DisplayDialogButton("Battle Again", BattleAgain);
            }
        }
        else
        {
            // Pre-battle dialog
            dialog = GetPreBattleDialog(interactionCount);

            if (interactionCount == 1)
            {
                StartBattle();
                ExitConversation();
                yield break;
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
            case 0: return "Nice Trainer: Let's battle.";
            case 1: return "Nice Trainer: Nice Battle";
            default: return "Nice Trainer: Wow nice battle.";
        }
    }
}