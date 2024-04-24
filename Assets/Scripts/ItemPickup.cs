using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item Item;
    public GameObject Knife;

    void Pickup()
    {
        InventoryManager.Instance.Add(Item);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Movement movement = collider.GetComponent<Movement>();
        if (movement != null)
        {
            movement.IncreaseRunSpeed();
            Knife.SetActive(true);
            Pickup();
        }
    }
}
