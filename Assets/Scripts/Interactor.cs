using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Interactor : MonoBehaviour
{
    public bool isInRange;
    public UnityEvent interactAction;
    public GameObject text;
    void Update()
    {
        if (isInRange)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                interactAction.Invoke();
            }
        }
    }
    public void TurnOff()
    {
        isInRange = false;
        text.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out NetworkBehaviour nb) && nb.IsLocalPlayer)
        {
            isInRange = true;
            text.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out NetworkBehaviour nb) && nb.IsLocalPlayer)
        {
            TurnOff();
        }
    }
}
