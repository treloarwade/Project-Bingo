using UnityEngine;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Reference to the player object
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
        LoadCoordinates();
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

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the target position
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            // Smoothly interpolate towards the target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        Speed.text = "Speed: " + smoothTime.ToString();
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

}
