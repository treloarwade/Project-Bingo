using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GardenerScript : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    public void Bingo()
    {
        StartCoroutine(Dingo());
    }
    IEnumerator Dingo()
    {
        // Check if the dialog box is not active
        if (!dialogBox.activeSelf)
        {
            yield return new WaitForSeconds(0.1f);
            dialogBox.SetActive(true);
            dialogText.text = "Gardener: Feel free to pick one";
            int count = Flower.destroyedFlowerCount;
            Debug.Log($"Destroyed Flower Count: {count}");
        }
        yield return null;
    }
    //
}
