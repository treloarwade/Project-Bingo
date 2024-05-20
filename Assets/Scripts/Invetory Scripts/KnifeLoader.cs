using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeLoader : MonoBehaviour
{
    public GameObject Knife;
    public Movement movement;

    private bool isKnifeEquipped = false;
    
    // Toggle between equipping and unequipping the knife
    public void ToggleKnife()
    {
        if (isKnifeEquipped)
        {
            UnequipKnife();
        }
        else
        {
            EquipKnife();
        }

        // Toggle the state
        isKnifeEquipped = !isKnifeEquipped;
    }

    private void EquipKnife()
    {
        movement.IncreaseRunSpeed();
        Knife.SetActive(true);
    }

    private void UnequipKnife()
    {
        movement.DecreaseRunSpeed();
        Knife.SetActive(false);
    }
}
