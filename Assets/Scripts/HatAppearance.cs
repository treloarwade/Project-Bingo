using UnityEngine;

public class HatAppearance : MonoBehaviour
{
    public Sprite[] hatsprites;
    public SpriteRenderer hat;
    public int hatIndex = 0;

    void Start()
    {
        hat = GetComponent<SpriteRenderer>();
    }

    public void RightArrow4()
    {
        hatIndex++;
        if (hatIndex >= hatsprites.Length)
        {
            hatIndex = 0;
        }
        hat.sprite = hatsprites[hatIndex];
    }
}
