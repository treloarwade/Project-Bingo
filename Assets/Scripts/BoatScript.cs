using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatScript : MonoBehaviour
{
    private bool movementEnabled = true;
    public GameObject Bingo;
    public GameObject Boat;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            movementEnabled = false;
            UpdateMovement();
            StartCoroutine(BoatAnimation());
        }
    }
    IEnumerator BoatAnimation()
    {
        Vector3 originalPosition = Bingo.transform.localPosition;
        Vector3 targetPosition = new Vector3(109.87f, 69.25f, 0);
        Vector3 targetPosition2 = new Vector3(-40, 200, 2.05347633f);
        int randomIndex = Random.Range(0, 3);
        float[] possibleValues = { 0.5f, 1f, 1.5f };
        float randomFloatValue = possibleValues[randomIndex];
        Vector3 newPosition = Boat.transform.localPosition;
        newPosition.y -= 100f;

        // Phase 1: Move to the right
        float startTime = Time.time;
        while (Time.time - startTime < 1)
        {
            float fracJourney = (Time.time - startTime) / 1;
            Bingo.transform.localPosition = Vector3.Lerp(Bingo.transform.localPosition, targetPosition, fracJourney);
            Bingo.transform.Rotate(Vector3.forward, randomFloatValue);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        startTime = Time.time;
        while (Time.time - startTime < 1)
        {
            float fracJourney = (Time.time - startTime) / 1;
            Bingo.transform.localPosition = Vector3.Lerp(Bingo.transform.localPosition, targetPosition, fracJourney);
            Bingo.transform.Rotate(Vector3.forward, randomFloatValue);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        // Phase 2: Move back
        startTime = Time.time;
        while (Time.time - startTime < 5)
        {
            float fracJourney = (Time.time - startTime) / 5;
            Bingo.transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, fracJourney);
            Bingo.transform.Rotate(Vector3.forward, -randomFloatValue);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        movementEnabled = true;
        UpdateMovement();
    }
    void UpdateMovement()
    {
        // Enable or disable player movement based on movementEnabled flag
        Movement movementScript = GetComponent<Movement>();
        if (movementScript != null)
        {
            movementScript.enabled = movementEnabled;
        }
    }
}
