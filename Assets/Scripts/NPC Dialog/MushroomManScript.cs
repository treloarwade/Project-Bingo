using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MushroomManScript : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    private bool treesMoved = false;
    public int moveDistance = 2; // Adjusted moveDistance for a larger movement
    public float moveSpeed = 1.5f; // Adjusted moveSpeed for smoother animation
    public GameObject trees;
    private Vector3 targetPosition;
    public Interactor interactor;
    public void ShingoMushroom()
    {
        StartCoroutine(Dingo());
    }
    IEnumerator Dingo()
    {
        // Check if the dialog box is not active
        if (!dialogBox.activeSelf)
        {
            yield return new WaitForSeconds(0.1f);
            dialogBox.SetActive(true);
            dialogText.text = "The path is open my friend. I cannot follow you down the path to glory... I can only open it";
            if (!treesMoved)
            {
                dialogText.text = "I'll open the trees for you.";
                // Set target position to current position + offset to the right
                targetPosition = new Vector3(-60.8199982f, 27.69f, 0);
                StartCoroutine(MoveTreesCoroutine());
            }
        }
        interactor.TurnOff();
        yield return null;
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
