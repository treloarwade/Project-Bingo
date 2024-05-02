using DingoSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrassBattle : MonoBehaviour
{
#pragma warning disable CS0414 // The field 'GrassBattle.isWiggling' is assigned but its value is never used
    private bool isWiggling;
#pragma warning restore CS0414 // The field 'GrassBattle.isWiggling' is assigned but its value is never used
    private float wiggleDuration = 0.5f;
    private float maxWiggleAngle = 10f;
    public List<DingoID> dingos = new List<DingoID>();
    private float lastActivationTime;

    private void Start()
    {
        lastActivationTime = Time.time;
        dingos = new List<DingoID>(DingoDatabase.allDingos);
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
        if (!isWiggling)
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
                else
                {
                    StartCoroutine(WiggleGrass());
                }
            }
        }
    }
    private IEnumerator WiggleGrass()
    {
        isWiggling = true; // Set wiggling flag to true

        float startTime = Time.time; // Record the start time
        Quaternion originalRotation = transform.rotation; // Store the original rotation

        // Randomly select the direction of the initial wiggle
        float direction = Random.Range(0, 2) == 0 ? -1f : 1f;

        while (Time.time - startTime < wiggleDuration)
        {
            float t = (Time.time - startTime) / wiggleDuration; // Calculate the interpolation parameter

            // Calculate the angle to rotate using a smooth oscillating motion
            float smoothAngle = Mathf.Lerp(-1f, 1f, Mathf.Sin(t * Mathf.PI));
            float angle = Mathf.Lerp(-maxWiggleAngle, maxWiggleAngle, smoothAngle * direction);

            transform.rotation = originalRotation * Quaternion.Euler(0f, 0f, angle); // Rotate the grass object
            yield return null; // Wait for the next frame
        }

        transform.rotation = originalRotation; // Reset rotation when wiggling is done
        isWiggling = false; // Set wiggling flag to false
    }
}
