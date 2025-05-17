using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NpcMovementScriptStatic : NetworkBehaviour
{
    public float maxRotation = 5f;
    public float rotationSpeed = 50f;

    private float currentRotation = 0f;
    private float rotationDirection = 1f;
    private Rigidbody2D rb;
    private Coroutine rotationRoutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetMovement(bool movement)
    {
        if (movement)
        {
            rotationRoutine = StartCoroutine(RotateSprite());
        }
        else
        {
            if (rotationRoutine != null)
            {
                StopCoroutine(rotationRoutine);
                rotationRoutine = null;
            }
            currentRotation = 0f;
            rb.SetRotation(currentRotation);
        }
    }

    private IEnumerator RotateSprite()
    {
        while (true)
        {
            Debug.Log("running");
            currentRotation += rotationDirection * rotationSpeed * Time.deltaTime;

            if (Mathf.Abs(currentRotation) >= maxRotation)
            {
                rotationDirection *= -1;
            }

            rb.SetRotation(currentRotation);
            yield return null;
        }
    }
}
