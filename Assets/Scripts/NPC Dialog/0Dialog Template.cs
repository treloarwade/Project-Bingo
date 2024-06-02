using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogTemplate : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    public Interactor interactor;
    public void Dialogue()
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
            dialogText.text = "Agent Bingo checked didn't find anything behind the bushes of his house today";
        }
        interactor.TurnOff();
        yield return null;
    }
}
