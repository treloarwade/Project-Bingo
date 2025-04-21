using System.Collections;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkDingo : NetworkBehaviour
{
    public NetworkVariable<int> id = new NetworkVariable<int>();
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
    public NetworkVariable<int> move1 = new NetworkVariable<int>();
    public NetworkVariable<int> move2 = new NetworkVariable<int>();
    public NetworkVariable<int> move3 = new NetworkVariable<int>();
    public NetworkVariable<int> move4 = new NetworkVariable<int>();
    public NetworkVariable<bool> isFlipped = new NetworkVariable<bool>(false); // Default to not flipped
    public NetworkVariable<int> battleMoveId = new NetworkVariable<int>(-1); // Default to not flipped
    public NetworkVariable<int> battleTargetId = new NetworkVariable<int>(-1); // Default to not flipped
    public NetworkVariable<int> slotNumber = new NetworkVariable<int>(-1); // Default to not flipped
    public NetworkVariable<bool> hasAttemptedCatch = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> isPlayer = new NetworkVariable<bool>(false);


    public Slider healthSlider;
    private SpriteRenderer spriteRenderer;
    private Coroutine healthAnimation;
    public Text nameText;
    public Text typeText;
    public Text hpText;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        battleMoveId.Value = -1;
        // Subscribe to value changes
        spritePath.OnValueChanged += OnSpritePathChanged;
        name.OnValueChanged += OnNameChanged;
        isFlipped.OnValueChanged += OnFlippedChanged;

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

        UpdateSprite(spritePath.Value.ToString());
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
        isFlipped.OnValueChanged -= OnFlippedChanged;
        maxHP.OnValueChanged -= OnMaxHPChanged;
        hp.OnValueChanged -= OnHPChanged;


    }

    private void OnSpritePathChanged(FixedString128Bytes oldValue, FixedString128Bytes newValue)
    {
        UpdateSprite(newValue.ToString());
    }

    private void OnNameChanged(FixedString64Bytes oldValue, FixedString64Bytes newValue)
    {
        Debug.Log($"Dingo name changed to: {newValue}");
    }
    private void OnFlippedChanged(bool oldValue, bool newValue)
    {
        spriteRenderer.flipX = newValue;
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

        spriteRenderer.flipX = isFlipped.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetDingoAttributesServerRpc(int iD, FixedString128Bytes newSpritePath, FixedString64Bytes newName, FixedString64Bytes newType,
        int newHP, int newAttack, int newDefense, int newSpeed, int newMaxHP, int newXP, int newMaxXP, int newLevel, int Move1, int Move2, int Move3, int Move4)
    {
        id.Value = iD;
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
        move1.Value = Move1;
        move2.Value = Move2;
        move3.Value = Move3;
        move4.Value = Move4;
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
        if (healthSlider == null)
        {
            Debug.LogWarning("Health slider is not assigned.");
            yield break;
        }

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
