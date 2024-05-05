using DingoSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTrainer : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    private float lastActivationTime;
    public List<DingoID> dingos = new List<DingoID>();
    public bool isTrainer = true;
    private void Start()
    {
        lastActivationTime = Time.time;
        dingos = new List<DingoID>(DingoDatabase.trainerDingos);
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (Time.time - lastActivationTime >= 20f)
        {
            dialogBox.SetActive(true);
            dialogText.text = "I will be the first battle";
            lastActivationTime = Time.time;
            SaveCoordinates();
            Loader.Load(Loader.Scene.Battle, dingos, isTrainer);
        }
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
}
