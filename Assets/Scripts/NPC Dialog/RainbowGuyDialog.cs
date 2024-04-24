using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RainbowGuyDialog : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    private float lastActivationTime;
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (Time.time - lastActivationTime >= 20f)
        {
            dialogBox.SetActive(true);
            dialogText.text = "dudes will play rainbow six but havent stopped and admired a rainbow since they were six. Siege";
            lastActivationTime = Time.time;
        }
    }
}
