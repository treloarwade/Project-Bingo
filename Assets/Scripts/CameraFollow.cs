using System.Collections;
using System.IO;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Reference to the player object
    public Vector3 battlePosition;
    public bool battleActive;
    public Text Speed;
    public float smoothTime = 0.5f;
    private Vector3 velocity = Vector3.zero;
    private int smoothTimeIndex = 0;
    private float[] smoothTimeValues = { 1f, 0.5f, 0f }; // Array of smoothTime values to cycle through
    public Camera mainCamera; // Reference to the main camera
    public float[] zoomLevels = { 1f, 2f, 5f, 10f, 15f }; // Orthographic sizes for each zoom level
    private int zoomLevelIndex = 0;
    private Vector3 savedPosition;

    private void Start()
    {
        StartCoroutine(FindAndFollowLocalPlayer());

    }
    public IEnumerator FindAndFollowLocalPlayer()
    {
        yield return new WaitForSeconds(0.2f); // Small delay to ensure players are spawned

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<NetworkObject>().IsOwner)
            {
                LocalPlayerManager.Instance.SetLocalPlayer(player);

                target = player.transform;
                Debug.Log("Camera now following: " + player.name);
                break;
            }
        }
    }
    public void LoadCoordinates()
    {
        // Load the position from PlayerPrefs
        Vector3 savedPosition = new Vector3(
            PlayerPrefs.GetFloat("PosX", 0),
            PlayerPrefs.GetFloat("PosY", 0),
            -10 // Assuming default Z position is -10
        );
        transform.localPosition = savedPosition;
    }
    public void ChangeTarget(int newtarget)
    {
        GameObject newName = GameObject.Find("Player" + newtarget);
        target = newName.transform;
    }

    void LateUpdate()
    {
        ulong Bingo = NetworkManager.Singleton.LocalClientId;
        if (target != null)
        {

            if (!battleActive)
            {
                // Calculate the target position when battle is not active
                Vector3 targetPosition = new Vector3(target.position.x, target.position.y, -10);

                // Smoothly interpolate towards the target position
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            }
            else
            {
                // Use battlePosition when battle is active
                // Assuming battlePosition is set somewhere else in your code.
                Vector3 targetPosition = battlePosition;
                targetPosition.z = -10;
                // Smoothly interpolate towards the battle position
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            }

            // Update the speed text (you may want to adjust this as per your requirement)
            Speed.text = "Speed: " + smoothTime.ToString();
        }
    }

    public void OnClick()
    {
        // Increment the smoothTime index
        smoothTimeIndex = (smoothTimeIndex + 1) % smoothTimeValues.Length;

        // Update smoothTime to the value at the current index
        smoothTime = smoothTimeValues[smoothTimeIndex];

        Debug.Log("smoothTime toggled to: " + smoothTime);
    }
    public void ToggleZoom()
    {
        if (mainCamera != null)
        {
            // Increment the zoom level index
            zoomLevelIndex = (zoomLevelIndex + 1) % zoomLevels.Length;

            // Set the orthographic size based on the current zoom level
            mainCamera.orthographicSize = zoomLevels[zoomLevelIndex];
        }
        else
        {
            Debug.LogWarning("Main camera reference is not set!");
        }
    }
    public void CoordinateFetch()
    {
        mainCamera.orthographicSize = 20f;
        StartCoroutine(ReturntoZoom());
    }
    IEnumerator ReturntoZoom()
    {
        yield return new WaitForSeconds(2f);
        mainCamera.orthographicSize = zoomLevels[zoomLevelIndex];
    }
}
