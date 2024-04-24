using UnityEngine;

public class TorsoAppearance : MonoBehaviour
{
    public Sprite[] torsosprites;
    public SpriteRenderer torso;
    public int torsoIndex = 0;

    void Start()
    {
        torso = GetComponent<SpriteRenderer>();
    }

    public void RightArrow1()
    {
        torsoIndex++;
        if (torsoIndex >= torsosprites.Length)
        {
            torsoIndex = 0;
        }
        torso.sprite = torsosprites[torsoIndex];
    }
}
