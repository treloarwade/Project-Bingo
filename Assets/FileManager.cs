using System.Collections;
using System.IO;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    void Start()
    {
        MakeInitialSave();
        StartCoroutine(CheckAgain());
    }
    private IEnumerator CheckAgain()
    {
        yield return new WaitForSeconds(0.5f);
        MakeInitialSave();
        yield return null;
    }
    private void MakeInitialSave()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "playerinfo.json");
        if (!File.Exists(filePath))
        {
            NetworkDingo dingo = DingoLoader.LoadRandomDingoFromList(DingoDatabase.agentBingo);
            BattleHandler.RequestSaveDingo(NetworkManager.Singleton.LocalClientId, dingo, false);
            if (dingo.gameObject != null)
            {
                if (dingo.GetComponent<NetworkObject>().IsSpawned)
                {
                    dingo.GetComponent<NetworkObject>().Despawn();
                }
                GameObject.Destroy(dingo.gameObject);
            }
        }
    }
}
