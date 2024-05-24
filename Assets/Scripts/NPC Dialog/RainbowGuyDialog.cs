using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RainbowGuyDialog : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    public Interactor interactor;
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
            dialogBox.SetActive(true);
            dialogText.text = "dudes will play rainbow six but havent stopped and admired a rainbow since they were six. Siege";
        }
        interactor.TurnOff();
        yield return null;
    }
}
