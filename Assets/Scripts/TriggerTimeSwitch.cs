using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTimeSwitch : MonoBehaviour
{
    public DayAndNight dayAndNight;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            dayAndNight.TurnToNightIf();
        }
    }
}
