using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class JoinButton : MonoBehaviour
{
    private CameraFollow cameraFollow;

    private void Start()
    {
        // Find the CameraFollow script in the scene
        cameraFollow = FindObjectOfType<CameraFollow>();

        if (cameraFollow == null)
        {
            Debug.LogError("CameraFollow script not found in the scene!");
        }

        // Add button listener
        GetComponent<Button>().onClick.AddListener(() => StartCoroutine(JoinGame()));
    }

    private IEnumerator JoinGame()
    {
        // Shut down host if running
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Shutting down host...");
            NetworkManager.Singleton.Shutdown();
            yield return new WaitForSeconds(1f); // Wait before restarting as a client
        }

        // Start as a client
        Debug.Log("Starting as client...");
        if (!NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StartClient();
        }

        // Wait a moment to ensure client connection before following
        yield return new WaitForSeconds(1f);

        // Find and follow the local player
        if (cameraFollow != null)
        {
            StartCoroutine(cameraFollow.FindAndFollowLocalPlayer());
            LayerSwitcherManager.Instance?.OnLocalPlayerReady();
        }
    }
}
