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
    public bool IsPlayerInRange()
    {
        return playerInTrigger && localPlayer != null && localPlayer.IsLocalPlayer;
    }
}