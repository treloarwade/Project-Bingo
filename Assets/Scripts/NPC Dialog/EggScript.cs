using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EggScript : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    private float lastActivationTime;
    public float speed = 1f; // Speed of movement
    public float maxRotation = 3f;
    public float rotationSpeed = 30f;
    private float currentRotation = 0f;
    private float rotationDirection = 1f;
    Rigidbody2D rb;
    public Collider2D bingo2;
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
            bingo2.enabled = false;
            float startTime = Time.time;
            while (Time.time - startTime < 5f)
            {
                rb.velocity = Vector2.left * speed;
                yield return null;
            }
            bingo2.enabled = true;
            yield return new WaitForSeconds(1f);
            bingo2.enabled = false;
            while (dialogBox.activeSelf)
            {
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < 5f)
            {
                rb.velocity = Vector2.right * speed;
                yield return null;
            }
            bingo2.enabled = true;
            yield return new WaitForSeconds(1f);
            bingo2.enabled = false;
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
            dialogText.text = "Egg: I'm actually a Dingo, but I'm very fragile so I don't like to fight.";
            lastActivationTime = Time.time;
        }
    }
}
