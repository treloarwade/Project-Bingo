using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoHider : MonoBehaviour
{
    public float delayInSeconds = 5f;
    void Start()
    {
        StartCoroutine(HideAfterDelay());
    }
    public void Bingo()
    {
        StartCoroutine(HideAfterDelay());
    }
    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(delayInSeconds);
        gameObject.SetActive(false);
    }
}
