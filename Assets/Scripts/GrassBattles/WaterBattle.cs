using DingoSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBattle : MonoBehaviour
{
    public List<DingoID> dingos = new List<DingoID>();
    private float lastActivationTime;

    private void Start()
    {
        lastActivationTime = Time.time;
        dingos = new List<DingoID>(DingoDatabase.waterDingos);
    }
    public void SaveCoordinates()
    {
        // Save the position and rotation of the object
        PlayerPrefs.SetFloat("PosX", transform.position.x);
        PlayerPrefs.SetFloat("PosY", transform.position.y);
        PlayerPrefs.SetFloat("PosZ", transform.position.z);

        // Save PlayerPrefs to disk
        PlayerPrefs.Save();

        Debug.Log("Coordinates saved.");
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            int randomNumber = Random.Range(0, 10);
            if (randomNumber < 1)
            {
                if (Time.time - lastActivationTime >= 3f)
                {
                    SaveCoordinates();
                    Loader.Load(Loader.Scene.Battle, dingos);
                }

            }
        }
    }
}
