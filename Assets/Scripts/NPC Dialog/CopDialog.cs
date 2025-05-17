using System.Collections;
using UnityEngine;


public class CopDialog : MonoBehaviour
{
    public Interactor interactor;
    public NpcMovementScript npcMovement;
    public void Bingo2()
    {
        if (!DialogManager.Instance.IsDialogActive())
        {
            StartCoroutine(Bingo());
        }
    }
    IEnumerator Bingo()
    {
        // Check if the dialog box is not active
        if (!DialogManager.Instance.IsDialogActive())
        {
            yield return new WaitForSeconds(0.1f);
            string dialog;
            if (Random.value < 0.5f)
            {
                dialog = "Everyone catches the high level Bingomon, so all the ones here are low level.";
            }
            else
            {
                dialog = "So far, no crimes today.";
            }
            DialogManager.Instance.DisplayDialogIsExitable(false, dialog);
            DialogManager.Instance.ClearDialogButtons();
            DialogManager.Instance.DisplayDialogButton("Cool", ContinueConversation);
            DialogManager.Instance.DisplayDialogButton("Nice", ExitConversation);
            npcMovement.conversation = true;
        }
        interactor.TurnOff();
        yield return new WaitForSeconds(10f);
        npcMovement.conversation = false;
    }
    public void ContinueConversation()
    {
        StartCoroutine(Bingo());
    }

    public void ExitConversation()
    {
        DialogManager.Instance.CloseDialogShopScreen();
    }
}
