using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocation : MonoBehaviour
{
    private const string playerPosXKey = "PlayerPosX";
    private const string playerPosYKey = "PlayerPosY";
    private const string playerPosZKey = "PlayerPosZ";

    void Start()
    {
        LoadPlayerPosition();
    }

    void OnApplicationQuit()
    {
        SavePlayerPosition();
    }

    private void SavePlayerPosition()
    {
        PlayerPrefs.SetFloat(playerPosXKey, transform.position.x);
        PlayerPrefs.SetFloat(playerPosYKey, transform.position.y);
        PlayerPrefs.SetFloat(playerPosZKey, transform.position.z);
        PlayerPrefs.Save();
    }

    private void LoadPlayerPosition()
    {
        float posX = PlayerPrefs.GetFloat(playerPosXKey);
        float posY = PlayerPrefs.GetFloat(playerPosYKey);
        float posZ = PlayerPrefs.GetFloat(playerPosZKey);

        transform.position = new Vector3(posX, posY, posZ);
    }
}
