using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MushroomManScript : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    private float lastActivationTime;
    private bool treesMoved = false;
    public int moveDistance = 2; // Adjusted moveDistance for a larger movement
    public float moveSpeed = 1.5f; // Adjusted moveSpeed for smoother animation
    public GameObject trees;
    private Vector3 targetPosition;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if it hasn't activated in the last 20 seconds
        if (Time.time - lastActivationTime >= 20f)
        {
            dialogBox.SetActive(true);
            dialogText.text = "The path is open my friend. I cannot follow you down the path to glory... I can only open it";
            lastActivationTime = Time.time;
            if (!treesMoved)
            {
                dialogText.text = "I'll open the trees for you.";
                // Set target position to current position + offset to the right
                targetPosition = trees.transform.position + Vector3.right * moveDistance;
                StartCoroutine(MoveTreesCoroutine());
            }
        }
    }
    private IEnumerator MoveTreesCoroutine()
    {
        yield return new WaitForSeconds(2f);
        float elapsedTime = 0f;
        Vector3 initialPosition = trees.transform.position;
        while (elapsedTime < 2f) // Adjust duration as needed
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / 2f); // 2f is the duration of the animation
            trees.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        treesMoved = true;
    }
}
