using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionGuyDialog : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    private float lastActivationTime;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (Time.time - lastActivationTime >= 20f)
        {
            dialogBox.SetActive(true);
            dialogText.text = "In the future, I will call people out if they name their Dingos bad words like Fucker";
            lastActivationTime = Time.time;
        }
    }
}
