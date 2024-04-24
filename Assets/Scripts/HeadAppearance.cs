using UnityEngine;

public class HeadAppearance : MonoBehaviour
{
    public Sprite[] headsprites;
    public SpriteRenderer head;
    public int headIndex = 0;

    void Start()
    {
        head = GetComponent<SpriteRenderer>();
    }

    public void RightArrow3()
    {
        headIndex++;
        if (headIndex >= headsprites.Length)
        {
            headIndex = 0;
        }
        head.sprite = headsprites[headIndex];
    }
}
