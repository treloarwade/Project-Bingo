using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : MonoBehaviour
{
    public GameObject FoodSpot;
    public int currentlyEquipped;
    public Sprite[] Food;
    // Start is called before the first frame update
    void Start()
    {
        currentlyEquipped = -1;
    }
    public void EquipFood(int ID)
    {
        if (FoodSpot.activeSelf)
        {
            if (currentlyEquipped == ID)
            {
                FoodSpot.SetActive(false);
            }
        }
        else
        {
            FoodSpot.SetActive(true);
        }
        SpriteRenderer renderer = FoodSpot.GetComponent<SpriteRenderer>();
        renderer.sprite = Food[ID];
        currentlyEquipped = ID;
    }
    public void ForceFoodActive(int ID)
    {
        FoodSpot.SetActive(true);
        SpriteRenderer renderer = FoodSpot.GetComponent<SpriteRenderer>();
        renderer.sprite = Food[ID];
        currentlyEquipped = ID;
    }
    public void UnequipFood()
    {
        FoodSpot.SetActive(false);
    }
}
