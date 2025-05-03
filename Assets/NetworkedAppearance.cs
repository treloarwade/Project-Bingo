using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Netcode;
using static UnityEngine.GraphicsBuffer;

public enum ClothingType
{
    Torso,
    Hat,
    Head,
    Hands,
    Legs
}

public class NetworkedAppearance : NetworkBehaviour
{
    [System.Serializable]
    public struct ClothingItem : INetworkSerializable
    {
        public ClothingType type;
        public int index;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref type);
            serializer.SerializeValue(ref index);
        }
    }

    [Header("Renderer References")]
    [SerializeField] private SpriteRenderer torsoRenderer;
    [SerializeField] private SpriteRenderer hatRenderer;
    [SerializeField] private SpriteRenderer headRenderer;
    [SerializeField] private SpriteRenderer handsRenderer;
    [SerializeField] private SpriteRenderer legsRenderer;

    private NetworkVariable<ClothingItem> currentAppearance = new NetworkVariable<ClothingItem>(
        new ClothingItem(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private Dictionary<ClothingType, SpriteRenderer> renderers;
    private Dictionary<ClothingType, List<Sprite>> clothingSprites;

    // Static reference to buttons to prevent multiple assignments
    private static Dictionary<ClothingType, (Button leftButton, Button rightButton)> clothingButtons;
    private static bool buttonsInitialized = false;
    private NetworkVariable<bool> isInWater = new NetworkVariable<bool>(false,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);
    public override void OnNetworkSpawn()
    {
        InitializeRenderers();
        LoadAllClothingSprites();

        if (IsOwner)
        {
            if (!buttonsInitialized)
            {
                InitializeButtons();
                buttonsInitialized = true;
            }
            //SetRandomAppearance();
        }

        currentAppearance.OnValueChanged += OnAppearanceChanged;
        OnAppearanceChanged(default, currentAppearance.Value);
        isInWater.OnValueChanged += (prev, current) => {
            UpdateWaterVisuals(current);
        };

        // Initialize visuals with current state
        UpdateWaterVisuals(isInWater.Value);
    }
    // Add this method to handle the water state changes
    public void SetInWaterState(bool inWater)
    {
        isInWater.Value = inWater;
        UpdateWaterVisuals(inWater);
    }
    private void UpdateWaterVisuals(bool inWater)
    {
        if (renderers.ContainsKey(ClothingType.Legs))
        {
            renderers[ClothingType.Legs].enabled = !inWater;
        }
    }
    private void InitializeRenderers()
    {
        renderers = new Dictionary<ClothingType, SpriteRenderer>
        {
            { ClothingType.Torso, torsoRenderer },
            { ClothingType.Hat, hatRenderer },
            { ClothingType.Head, headRenderer },
            { ClothingType.Hands, handsRenderer },
            { ClothingType.Legs, legsRenderer },
        };
    }

    private void InitializeButtons()
    {
        clothingButtons = new Dictionary<ClothingType, (Button leftButton, Button rightButton)>();

        // Find and assign buttons for each clothing type
        AssignButtons(ClothingType.Torso, "TorsoLeft", "TorsoRight");
        AssignButtons(ClothingType.Hat, "HatLeft", "HatRight");
        AssignButtons(ClothingType.Head, "HeadLeft", "HeadRight");
        AssignButtons(ClothingType.Hands, "HandsLeft", "HandsRight");
        AssignButtons(ClothingType.Legs, "LegsLeft", "LegsRight");
    }

    private void AssignButtons(ClothingType type, string leftButtonName, string rightButtonName)
    {
        string basePath = "Canvas/NonBattle/Character Menu/Viewport/Content/";
        Button leftButton = GameObject.Find(basePath + leftButtonName)?.GetComponent<Button>();
        Button rightButton = GameObject.Find(basePath + rightButtonName)?.GetComponent<Button>();

        if (leftButton != null && rightButton != null)
        {
            // Clear existing listeners first
            leftButton.onClick.RemoveAllListeners();
            rightButton.onClick.RemoveAllListeners();

            // Store references
            clothingButtons[type] = (leftButton, rightButton);

            // Add appropriate listeners based on type
            switch (type)
            {
                case ClothingType.Torso:
                    leftButton.onClick.AddListener(DecrementTorso);
                    rightButton.onClick.AddListener(IncrementTorso);
                    break;
                case ClothingType.Hat:
                    leftButton.onClick.AddListener(DecrementHat);
                    rightButton.onClick.AddListener(IncrementHat);
                    break;
                case ClothingType.Head:
                    leftButton.onClick.AddListener(DecrementHead);
                    rightButton.onClick.AddListener(IncrementHead);
                    break;
                case ClothingType.Hands:
                    leftButton.onClick.AddListener(DecrementHands);
                    rightButton.onClick.AddListener(IncrementHands);
                    break;
                case ClothingType.Legs:
                    leftButton.onClick.AddListener(DecrementLegs);
                    rightButton.onClick.AddListener(IncrementLegs);
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"Could not find buttons for {type}: {leftButtonName} and/or {rightButtonName}");
        }
    }

    private void LoadAllClothingSprites()
    {
        clothingSprites = new Dictionary<ClothingType, List<Sprite>>();

        foreach (ClothingType type in System.Enum.GetValues(typeof(ClothingType)))
        {
            clothingSprites[type] = new List<Sprite>();
            int i = 0;

            while (true)
            {
                string path = $"Clothing/{type.ToString().ToLower()}_{i}";
                Sprite sprite = Resources.Load<Sprite>(path);

                if (sprite == null) break;

                clothingSprites[type].Add(sprite);
                i++;
            }

            if (clothingSprites[type].Count == 0)
            {
                Debug.LogWarning($"No sprites found for {type}");
            }
        }
    }

    private void OnAppearanceChanged(ClothingItem previous, ClothingItem current)
    {
        if (renderers.ContainsKey(current.type) &&
            clothingSprites.ContainsKey(current.type) &&
            current.index < clothingSprites[current.type].Count)
        {
            renderers[current.type].sprite = clothingSprites[current.type][current.index];
        }
    }

    private void ChangeAppearance(ClothingType type, int index)
    {
        if (!IsOwner) return;

        if (clothingSprites[type].Count == 0)
        {
            index = 0;
        }
        else
        {
            if (index < 0)
            {
                index = clothingSprites[type].Count - 1;
            }
            else if (index >= clothingSprites[type].Count)
            {
                index = 0;
            }
        }

        currentAppearance.Value = new ClothingItem { type = type, index = index };
    }

    // Increment methods
    public void IncrementTorso() => ChangeAppearance(ClothingType.Torso, GetNextIndex(ClothingType.Torso));
    public void IncrementHat() => ChangeAppearance(ClothingType.Hat, GetNextIndex(ClothingType.Hat));
    public void IncrementHead() => ChangeAppearance(ClothingType.Head, GetNextIndex(ClothingType.Head));
    public void IncrementHands() => ChangeAppearance(ClothingType.Hands, GetNextIndex(ClothingType.Hands));
    public void IncrementLegs() => ChangeAppearance(ClothingType.Legs, GetNextIndex(ClothingType.Legs));

    // Decrement methods
    public void DecrementTorso() => ChangeAppearance(ClothingType.Torso, GetPreviousIndex(ClothingType.Torso));
    public void DecrementHat() => ChangeAppearance(ClothingType.Hat, GetPreviousIndex(ClothingType.Hat));
    public void DecrementHead() => ChangeAppearance(ClothingType.Head, GetPreviousIndex(ClothingType.Head));
    public void DecrementHands() => ChangeAppearance(ClothingType.Hands, GetPreviousIndex(ClothingType.Hands));
    public void DecrementLegs() => ChangeAppearance(ClothingType.Legs, GetPreviousIndex(ClothingType.Legs));

    private int GetNextIndex(ClothingType type)
    {
        if (!clothingSprites.ContainsKey(type) || clothingSprites[type].Count == 0) return 0;

        var current = currentAppearance.Value;
        int currentIndex = (current.type == type) ? current.index : 0;
        return (currentIndex + 1) % clothingSprites[type].Count;
    }

    private int GetPreviousIndex(ClothingType type)
    {
        if (!clothingSprites.ContainsKey(type) || clothingSprites[type].Count == 0) return 0;

        var current = currentAppearance.Value;
        int currentIndex = (current.type == type) ? current.index : 0;
        return (currentIndex - 1 + clothingSprites[type].Count) % clothingSprites[type].Count;
    }

    private void SetRandomAppearance()
    {
        foreach (ClothingType type in System.Enum.GetValues(typeof(ClothingType)))
        {
            if (clothingSprites.ContainsKey(type) && clothingSprites[type].Count > 0)
            {
                ChangeAppearance(type, Random.Range(0, clothingSprites[type].Count));
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (currentAppearance != null)
        {
            currentAppearance.OnValueChanged -= OnAppearanceChanged;
        }

        // Reset button initialization when the local player is destroyed
        if (IsOwner)
        {
            buttonsInitialized = false;
        }
    }
}