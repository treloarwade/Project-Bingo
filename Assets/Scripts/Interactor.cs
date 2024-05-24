using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Events;

public class Interactor : MonoBehaviour
{
    public bool isInRange;
    public UnityEvent interactAction;
    public GameObject text;
    void Update()
    {
        if (isInRange)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("shingo2");
                interactAction.Invoke();
            }
        }
    }
    public void TurnOff()
    {
        isInRange = false;
        text.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isInRange = true;
            text.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TurnOff();
        }
    }
}
