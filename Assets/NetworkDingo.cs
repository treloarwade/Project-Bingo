using System.Collections;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkDingo : NetworkBehaviour
{
    // Networked properties
    public NetworkVariable<FixedString128Bytes> spritePath = new NetworkVariable<FixedString128Bytes>();
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public NetworkVariable<FixedString64Bytes> name = new NetworkVariable<FixedString64Bytes>();
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    public NetworkVariable<FixedString64Bytes> type = new NetworkVariable<FixedString64Bytes>();
    public NetworkVariable<int> hp = new NetworkVariable<int>();
    public NetworkVariable<int> attack = new NetworkVariable<int>();
    public NetworkVariable<int> defense = new NetworkVariable<int>();
    public NetworkVariable<int> speed = new NetworkVariable<int>();
    public NetworkVariable<int> maxHP = new NetworkVariable<int>();
    public NetworkVariable<int> xp = new NetworkVariable<int>();
    public NetworkVariable<int> maxXP = new NetworkVariable<int>();
    public NetworkVariable<int> level = new NetworkVariable<int>();
    public Slider healthSlider;
    private SpriteRenderer spriteRenderer;
    private Coroutine healthAnimation;
    public Text nameText;
    public Text typeText;
    public Text hpText;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Subscribe to value changes
        spritePath.OnValueChanged += OnSpritePathChanged;
        name.OnValueChanged += OnNameChanged;

        // Initialize sprite
        UpdateSprite(spritePath.Value.ToString());
        StartCoroutine(SetText());
        maxHP.OnValueChanged += OnMaxHPChanged;
        hp.OnValueChanged += OnHPChanged;
        StartCoroutine(WaitForNetworkValues());
    }
    private IEnumerator SetText()
    {
        yield return new WaitForSeconds(0.3f);

        nameText.text = name.Value.ToString();
        typeText.text = type.Value.ToString();
        hpText.text = hp.Value.ToString() + "/" + maxHP.Value.ToString();
        yield return null;
    }
    private IEnumerator WaitForNetworkValues()
    {
        yield return new WaitUntil(() => hp.Value > 0 || maxHP.Value > 0);
        //StartCoroutine(AnimateHealthBar(hp.Value, maxHP.Value));
        healthSlider.maxValue = maxHP.Value;
        healthSlider.value = hp.Value;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        // Unsubscribe to prevent memory leaks
        spritePath.OnValueChanged -= OnSpritePathChanged;
        name.OnValueChanged -= OnNameChanged;
    }

    private void OnSpritePathChanged(FixedString128Bytes oldValue, FixedString128Bytes newValue)
    {
        UpdateSprite(newValue.ToString());
    }

    private void OnNameChanged(FixedString64Bytes oldValue, FixedString64Bytes newValue)
    {
        Debug.Log($"Dingo name changed to: {newValue}");
    }

    public void UpdateSprite(string newSpritePath)
    {
        Sprite newSprite = Resources.Load<Sprite>(newSpritePath);
        if (newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
        }
        else
        {
            Debug.LogError($"Failed to load sprite at path: {newSpritePath}");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetDingoAttributesServerRpc(FixedString128Bytes newSpritePath, FixedString64Bytes newName, FixedString64Bytes newType,
        int newHP, int newAttack, int newDefense, int newSpeed, int newMaxHP, int newXP, int newMaxXP, int newLevel)
    {
        spritePath.Value = newSpritePath;
        name.Value = newName;
        type.Value = newType;
        hp.Value = newHP;
        attack.Value = newAttack;
        defense.Value = newDefense;
        speed.Value = newSpeed;
        maxHP.Value = newMaxHP;
        xp.Value = newXP;
        maxXP.Value = newMaxXP;
        level.Value = newLevel;
    }

    // Update Health Bar Smoothly
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthSlider == null) return;

        if (healthAnimation != null)
            StopCoroutine(healthAnimation);

        healthAnimation = StartCoroutine(AnimateHealthBar(currentHealth));
    }

    private IEnumerator AnimateHealthBar(int targetHealth)
    {
        float startValue = healthSlider.value;
        float endValue = (float)targetHealth;
        float duration = 1f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            healthSlider.value = Mathf.Lerp(startValue, endValue, elapsed / duration);
            yield return null;
        }

        healthSlider.value = endValue;
    }
    private void OnMaxHPChanged(int oldValue, int newValue)
    {
        Debug.Log($"Max HP updated: {newValue}");
        if (healthSlider != null)
        {
            healthSlider.maxValue = newValue;
        }
    }

    private void OnHPChanged(int oldValue, int newValue)
    {
        Debug.Log($"HP updated: {newValue}");
        if (healthSlider != null)
        {
            hpText.text = hp.Value.ToString() + "/" + maxHP.Value.ToString();
            StartCoroutine(AnimateHealthBar(newValue));
        }
    }
}
