using UnityEngine;
using Unity.Netcode;

public class LocalPlayerManager : MonoBehaviour
{
    public static LocalPlayerManager Instance { get; private set; }
    public GameObject LocalPlayer { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLocalPlayer(GameObject player)
    {
        LocalPlayer = player;
    }
}
