using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkTrainer : NetworkBehaviour
{
    public NetworkVariable<FixedString128Bytes> spritePath = new NetworkVariable<FixedString128Bytes>();
    private SpriteRenderer spriteRenderer;
    public string trainerName;
    public NetworkVariable<int> trainerId = new NetworkVariable<int>();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Subscribe to value changes
        spritePath.OnValueChanged += OnSpritePathChanged;

    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        // Unsubscribe to prevent memory leaks
        spritePath.OnValueChanged -= OnSpritePathChanged;
    }

    private void OnSpritePathChanged(FixedString128Bytes oldValue, FixedString128Bytes newValue)
    {
        UpdateSprite(newValue.ToString());
    }
    public void UpdateSprite(string newSpritePath)
    {
        if (string.IsNullOrEmpty(newSpritePath))
        {
            Debug.LogError("Sprite path is empty or null.");
            return;
        }

        Sprite newSprite = Resources.Load<Sprite>(newSpritePath);

        if (newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
        }
        else
        {
            Debug.LogError($"Failed to load sprite at path: {newSpritePath}");
        }

        spriteRenderer.flipX = true;
    }
}
