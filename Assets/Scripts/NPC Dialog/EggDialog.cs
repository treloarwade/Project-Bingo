using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggDialog : MonoBehaviour
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
                dialog = "I'm actually a bingomon, but I'm too fragile to fight.";
            }
            else
            {
                dialog = "I'm actually a bingomon, but I'm too fragile to fight.";
            }
            DialogManager.Instance.DisplayDialogIsExitable(false, dialog);
            DialogManager.Instance.ClearDialogButtons();
            DialogManager.Instance.DisplayDialogButton("Cool");
            DialogManager.Instance.DisplayDialogButton("Nice");
            npcMovement.conversation = true;
        }
        interactor.TurnOff();
        yield return new WaitForSeconds(10f);
        npcMovement.conversation = false;
    }
}
