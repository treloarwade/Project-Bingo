using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CopScript : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    private float lastActivationTime;
    public float speed = 1f; // Speed of movement
    public float maxRotation = 5f;
    public float rotationSpeed = 50f;
    private float currentRotation = 0f;
    private float rotationDirection = 1f;
    Rigidbody2D rb;
    public Collider2D bingo;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody component
        StartCoroutine(MoveObject()); // Start the movement coroutine
    }
    private void FixedUpdate()
    {
        Vector2 movement = rb.velocity;
        if (movement.magnitude > 0.4f)
        {
            RotateSprite();
        }
        else
        {
            currentRotation = 0f;
            rb.SetRotation(currentRotation);
        }
    }
    private IEnumerator MoveObject()
    {
        while (true) // Infinite loop for continuous movement
        {
            bingo.enabled = false;
            float startTime = Time.time;
            while (Time.time - startTime < 11f)
            {
                rb.velocity = Vector2.right * speed;
                yield return null;
            }
            bingo.enabled = true;
            yield return new WaitForSeconds(5f);
            bingo.enabled = false;
            while (dialogBox.activeSelf)
            {
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < 11f)
            {
                rb.velocity = Vector2.left * speed;
                yield return null;
            }
            bingo.enabled = true;
            yield return new WaitForSeconds(5f);
            bingo.enabled = false;
            while (dialogBox.activeSelf)
            {
                yield return null;
            }
        }
    }
    private void RotateSprite()
    {
        currentRotation += rotationDirection * rotationSpeed * Time.fixedDeltaTime;

        if (Mathf.Abs(currentRotation) >= maxRotation)
        {
            rotationDirection *= -1;
        }

        rb.SetRotation(currentRotation);
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && Time.time - lastActivationTime >= 20f)
        {
            dialogBox.SetActive(true);
            dialogText.text = "Everyone catches the high level Dingos, so all the ones here are low level.";
            lastActivationTime = Time.time;
        }
    }
}
