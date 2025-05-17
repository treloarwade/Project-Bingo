using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainbowGuyDialog : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    public Interactor interactor;
    public Button shopButton;
    public List<int> items;
    public void Bingo2()
    {
        if (!dialogBox.activeSelf)
        {
            StartCoroutine(Bingo());
        }
    }
    IEnumerator Bingo()
    {
        // Check if the dialog box is not active
        if (!dialogBox.activeSelf)
        {
            yield return new WaitForSeconds(0.1f);
            string dialog;
            if (Random.value < 0.5f)
            {
                dialog = "dudes will play rainbow six but havent stopped and admired a rainbow since they were six. Siege";
            }
            else
            {
                dialog = "they buy skins but haven’t shed theirs in years. no evolution.";
            }
            DialogManager.Instance.DisplayDialogIsExitable(false, dialog);
            DialogManager.Instance.ClearDialogButtons();
            DialogManager.Instance.DisplayDialogButton("Cool", null);
            DialogManager.Instance.DisplayShopButton(items);
        }
        interactor.TurnOff();
        yield return null;
    }
}
