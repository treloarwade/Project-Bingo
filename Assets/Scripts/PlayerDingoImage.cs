using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DingoSystem;

public class PlayerDingoImage : MonoBehaviour
{
    public Image dingoImage; // Reference to the Image component where the Dingo sprite will be displayed

    void Start()
    {
        // Instantiate BingoStar from the DingoIDs namespace
        DingoID bingoStar = new BingoStar();

        // Load BingoStar's sprite from the Resources folder using its Sprite property
        Sprite dingoSprite = Resources.Load<Sprite>(bingoStar.Sprite);

        // Assign the loaded sprite to the Image component
        if (dingoSprite != null)
        {
            dingoImage.sprite = dingoSprite;
        }
        else
        {
            return;
        }
    }
}
