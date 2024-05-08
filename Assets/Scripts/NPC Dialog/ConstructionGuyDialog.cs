using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionGuyDialog : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    private float lastActivationTime;
    private float lastActivationTime2 = -5f;
    public PipeScript pipeScript;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && pipeScript.PipeWalk && Time.time - lastActivationTime2 >= 5f)
        {
            dialogBox.SetActive(true);
            dialogText.text = "Construction Guy: Why do you think it's okay to walk through our pipes?";
            lastActivationTime2 = Time.time;
        }
        else
        {
            if (collider.CompareTag("Player") && Time.time - lastActivationTime >= 20f)
            {
                dialogBox.SetActive(true);
                dialogText.text = "Construction Guy: Be careful, this is an unsafe area";
                lastActivationTime = Time.time;
            }
        }
    }
}
