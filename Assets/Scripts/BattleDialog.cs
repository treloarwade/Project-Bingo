using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class BattleDialog : NetworkBehaviour
{
    public static BattleDialog Instance { get; private set; }

    public int lettersPerSecond = 30;
    public Text dialogText;
    public Text overflow1;
    public Text overflow2;
    public Text overflow3;
    public Text overflow4;
    public Text overflow5;


    private Coroutine typingCoroutine;
    private Queue<string> messageQueue = new Queue<string>();
    private bool isTyping = false;
    private ulong? targetClientId = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    [ClientRpc]
    public void TypeDialogClientRpc(string dialog, ulong targetClientId, ClientRpcParams clientRpcParams = default)
    {
        // Only process if this is the intended client or targetClientId is 0 (broadcast)
        if (targetClientId != 0 && NetworkManager.Singleton.LocalClientId != targetClientId)
            return;

        this.targetClientId = targetClientId;
        StartCoroutine(TypeDialog(dialog));
    }

    public void SetDialog(string dialog)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        // Add new message to queue
        messageQueue.Enqueue(dialog);

        // If not currently typing, start processing queue
        if (!isTyping)
        {
            yield return StartCoroutine(ProcessMessageQueue());
        }
    }

    private IEnumerator ProcessMessageQueue()
    {
        isTyping = true;

        while (messageQueue.Count > 0)
        {
            string currentMessage = messageQueue.Dequeue();

            if (currentMessage.Length > 60)
            {
                string[] words = currentMessage.Split(' ');
                string chunk = "";

                foreach (string word in words)
                {
                    if ((chunk + word).Length + 1 > 60) // +1 for space
                    {
                        messageQueue.Enqueue(chunk.TrimEnd());
                        chunk = "";
                    }
                    chunk += word + " ";
                }

                if (!string.IsNullOrWhiteSpace(chunk))
                    messageQueue.Enqueue(chunk.TrimEnd());

                continue; // Skip processing now, the new chunks will be handled next
            }


            // Shift overflow
            overflow5.text = overflow4.text;
            overflow4.text = overflow3.text;
            overflow3.text = overflow2.text;
            overflow2.text = overflow1.text;
            overflow1.text = dialogText.text;
            dialogText.text = "";

            // Type out new message character by character
            foreach (var letter in currentMessage.ToCharArray())
            {
                dialogText.text += letter;
                yield return new WaitForSeconds(1f / lettersPerSecond);
            }

            yield return new WaitForSeconds(0.2f); // Small delay after message
        }

        isTyping = false;
        targetClientId = null;
    }

    // Helper method to send messages to specific clients
    public static void SendDialogToClient(ulong clientId, string message)
    {
        if (Instance == null) return;

        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        Instance.TypeDialogClientRpc(message, clientId, clientRpcParams);
    }

    // Method to send message to all clients (use 0 as clientId)
    public static void SendDialogToAll(string message)
    {
        if (Instance == null) return;
        Instance.TypeDialogClientRpc(message, 0);
    }

    // Check if this client should process the message
    public bool ShouldProcessMessage(ulong clientId)
    {
        // Process if:
        // 1. No specific client is targeted (broadcast)
        // 2. Or this is the targeted client
        return targetClientId == null || targetClientId == 0 || NetworkManager.Singleton.LocalClientId == targetClientId;
    }
}