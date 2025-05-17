using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Interactor : MonoBehaviour
{
    public bool isInRange;
    public UnityEvent interactAction;
    public GameObject text;

    private bool playerInTrigger = false;
    private NetworkBehaviour localPlayer;

    private void Update()
    {
        if (isInRange && !DialogManager.Instance.IsDialogActive())
        {
            if (Input.GetButtonDown("Fire1"))
            {
                interactAction.Invoke();
            }
        }
    }

    public void TurnOff()
    {
        isInRange = false;
        if (text != null) text.SetActive(false);
    }

    public void TurnOn()
    {
        if (!DialogManager.Instance.IsDialogActive() && playerInTrigger)
        {
            isInRange = true;
            if (text != null) text.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") &&
            collision.TryGetComponent(out NetworkBehaviour nb) &&
            nb.IsLocalPlayer)
        {
            playerInTrigger = true;
            localPlayer = nb;
            TurnOn();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") &&
            collision.TryGetComponent(out NetworkBehaviour nb) &&
            nb.IsLocalPlayer)
        {
            playerInTrigger = false;
            localPlayer = null;
            TurnOff();
        }
    }

    private void OnEnable()
    {
        DialogManager.Instance.OnDialogueStart += TurnOff;
        DialogManager.Instance.OnDialogueEnd += HandleDialogueEnd;
        DialogManager.Instance.OnShopOpened += TurnOff;
        DialogManager.Instance.OnShopClosed += HandleShopClosed;
    }

    private void OnDisable()
    {
        DialogManager.Instance.OnDialogueStart -= TurnOff;
        DialogManager.Instance.OnDialogueEnd -= HandleDialogueEnd;
        DialogManager.Instance.OnShopOpened -= TurnOff;
        DialogManager.Instance.OnShopClosed -= HandleShopClosed;
    }

    private void HandleShopClosed()
    {
        if (playerInTrigger && localPlayer != null && localPlayer.IsLocalPlayer)
        {
            TurnOn();
        }
    }
    // Replace the existing HandleDialogueEnd method with this:

    private void HandleDialogueEnd()
    {
        // Add a small delay to ensure all dialogue cleanup is complete
        StartCoroutine(DelayedTurnOn());
    }
    public bool IsPlayerInRange()
    {
        return playerInTrigger && localPlayer != null && localPlayer.IsLocalPlayer;
    }
    private IEnumerator DelayedTurnOn()
    {
        yield return new WaitForEndOfFrame(); // Wait one frame

        if (playerInTrigger && localPlayer != null && localPlayer.IsLocalPlayer)
        {
            // Double check that dialogue is really closed
            if (!DialogManager.Instance.IsDialogActive())
            {
                TurnOn();
            }
        }
    }
}