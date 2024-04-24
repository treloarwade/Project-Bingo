using UnityEngine;

public class HandAppearance : MonoBehaviour
{
    public Sprite[] handsprites;
    public SpriteRenderer hand;
    public int handIndex = 0;

    void Start()
    {
        hand = GetComponent<SpriteRenderer>();
    }

    public void RightArrow5()
    {
        handIndex++;
        if (handIndex >= handsprites.Length)
        {
            handIndex = 0;
        }
        hand.sprite = handsprites[handIndex];
    }
}
