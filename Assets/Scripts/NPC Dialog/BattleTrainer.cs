using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTrainer : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    private float lastActivationTime;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (Time.time - lastActivationTime >= 20f)
        {
            dialogBox.SetActive(true);
            dialogText.text = "I will be the first battle";
            lastActivationTime = Time.time;
        }
    }
}
