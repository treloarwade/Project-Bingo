using UnityEngine;

public class LegsAppearance : MonoBehaviour
{
    public Sprite[] legssprites;
    public SpriteRenderer legs;
    public int legsIndex = 0;

    void Start()
    {
        legs = GetComponent<SpriteRenderer>();
    }

    public void RightArrow2()
    {
        legsIndex++;
        if (legsIndex >= legssprites.Length)
        {
            legsIndex = 0;
        }
        legs.sprite = legssprites[legsIndex];
    }
}
