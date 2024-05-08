using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeScript : MonoBehaviour
{
    public bool PipeWalk = false;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if the collider that entered the trigger is the player
        if (collider.CompareTag("Player"))
        {
            PipeWalk = true;
            Debug.Log("worked" + PipeWalk);
        }
    }
}
