using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Netcode;

public enum ClothingType
{
    Torso,
    Hat,
    Head,
    Legs,
    Hands
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
    [SerializeField] private SpriteRenderer legsRenderer;
    [SerializeField] private SpriteRenderer handsRenderer;
    [SerializeField] private Image torsoImage;
    [SerializeField] private Image hatImage;
    [SerializeField] private Image headImage;
    [SerializeField] private Image legsImage;

    private NetworkVariable<ClothingItem> currentAppearance = new NetworkVariable<ClothingItem>(
        new ClothingItem(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private Dictionary<ClothingType, SpriteRenderer> renderers;
    private Dictionary<ClothingType, Image> images;
    private Dictionary<ClothingType, List<Sprite>> clothingSprites;
    private Dictionary<int, Dictionary<ClothingType, int>> outfitPresets;
    private int outfitIndex;
    // Static reference to buttons to prevent multiple assignments
    private static Dictionary<ClothingType, (Button leftButton, Button rightButton)> clothingButtons;
    private static bool buttonsInitialized = false;
    private NetworkVariable<bool> isInWater = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        InitializeRenderers();
        InitializeImages();
        LoadAllClothingSprites();
        InitializeOutfitPresets();

        if (IsOwner)
        {
            if (!buttonsInitialized)
            {
                InitializeButtons();
                buttonsInitialized = true;
            }
        }

        currentAppearance.OnValueChanged += OnAppearanceChanged;
        OnAppearanceChanged(default, currentAppearance.Value);
        isInWater.OnValueChanged += (prev, current) => {
            UpdateWaterVisuals(current);
        };

        UpdateWaterVisuals(isInWater.Value);
    }

    private void InitializeOutfitPresets()
    {
        outfitPresets = new Dictionary<int, Dictionary<ClothingType, int>>()
        {
            // Outfit 0: All items set to index 0
            { 0, new Dictionary<ClothingType, int>() {
                { ClothingType.Torso, 0 },
                { ClothingType.Hat, 0 },
                { ClothingType.Head, 0 },
                { ClothingType.Legs, 0 }
            }},
            // Outfit 1: All items 0 except head is 1
            { 1, new Dictionary<ClothingType, int>() {
                { ClothingType.Torso, 0 },
                { ClothingType.Hat, 0 },
                { ClothingType.Head, 1 },
                { ClothingType.Legs, 0 }
            }},
            // Outfit 2: All items 1 except head is 2
            { 2, new Dictionary<ClothingType, int>() {
                { ClothingType.Torso, 1 },
                { ClothingType.Hat, 1 },
                { ClothingType.Head, 2 },
                { ClothingType.Legs, 1 }
            }},
            { 3, new Dictionary<ClothingType, int>() {
                { ClothingType.Torso, 2 },
                { ClothingType.Hat, 0 },
                { ClothingType.Head, 3 },
                { ClothingType.Legs, 0 }
            }}
            // Add more outfit presets as needed
        };
    }

    public void SetOutfit(int outfitIndex)
    {
        if (!IsOwner || !outfitPresets.ContainsKey(outfitIndex)) return;

        var outfit = outfitPresets[outfitIndex];
        foreach (var item in outfit)
        {
            ChangeAppearance(item.Key, item.Value);
        }
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
            { ClothingType.Legs, legsRenderer }
        };
    }
    private void InitializeImages()
    {
        string basePath = "Canvas/NonBattle/Character Menu/Viewport/";
        torsoImage = GameObject.Find(basePath + "torso").GetComponent<Image>();
        hatImage = GameObject.Find(basePath + "hat").GetComponent<Image>();
        legsImage = GameObject.Find(basePath + "legs").GetComponent<Image>();
        headImage = GameObject.Find(basePath + "head").GetComponent<Image>();
        images = new Dictionary<ClothingType, Image>
        {
            { ClothingType.Torso, torsoImage },
            { ClothingType.Hat, hatImage },
            { ClothingType.Head, headImage },
            { ClothingType.Legs, legsImage }
        };
    }

    private void InitializeButtons()
    {
        clothingButtons = new Dictionary<ClothingType, (Button leftButton, Button rightButton)>();

        // Find and assign buttons for each clothing type
        AssignButtons(ClothingType.Torso, "TorsoLeft", "TorsoRight");
        AssignButtons(ClothingType.Hat, "HatLeft", "HatRight");
        AssignButtons(ClothingType.Head, "HeadLeft", "HeadRight");
        AssignButtons(ClothingType.Legs, "LegsLeft", "LegsRight");
        AssignOutfitButtons("HandsLeft", "HandsRight");
        Button menuButton = GameObject.Find("Canvas/NonBattle/Debug Menu/Viewport/Content/Character Customization/")?.GetComponent<Button>();
        menuButton.onClick.AddListener(UpdateAllAppearances);
    }
    private void AssignOutfitButtons(string leftButtonName, string rightButtonName)
    {
        string basePath = "Canvas/NonBattle/Character Menu/Viewport/Content/";
        Button leftButton = GameObject.Find(basePath + leftButtonName)?.GetComponent<Button>();
        Button rightButton = GameObject.Find(basePath + rightButtonName)?.GetComponent<Button>();

        if (leftButton != null && rightButton != null)
        {
            leftButton.onClick.AddListener(DecrementOutfit);
            rightButton.onClick.AddListener(IncrementOutfit);
        }
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
    private void UpdateAllAppearances()
    {
        SetOutfit(outfitIndex);
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

        // Load hands sprites
        clothingSprites[ClothingType.Torso].ForEach(sprite => {
            string handsPath = $"Clothing/hands_{sprite.name.Split('_')[1]}";
            Sprite handsSprite = Resources.Load<Sprite>(handsPath);
            if (handsSprite != null)
            {
                if (!clothingSprites.ContainsKey(ClothingType.Hands))
                {
                    clothingSprites[ClothingType.Hands] = new List<Sprite>();
                }
                clothingSprites[ClothingType.Hands].Add(handsSprite);
            }
        });
    }
    private void OnAppearanceChanged(ClothingItem previous, ClothingItem current)
    {
        if (renderers.ContainsKey(current.type) &&
            clothingSprites.ContainsKey(current.type) &&
            current.index < clothingSprites[current.type].Count)
        {
            renderers[current.type].sprite = clothingSprites[current.type][current.index];
            images[current.type].sprite = clothingSprites[current.type][current.index];

            // Update hands when torso changes
            if (current.type == ClothingType.Torso && handsRenderer != null)
            {
                int handsIndex = current.index;
                if (clothingSprites.ContainsKey(ClothingType.Hands) &&
                    handsIndex < clothingSprites[ClothingType.Hands].Count)
                {
                    handsRenderer.sprite = clothingSprites[ClothingType.Hands][handsIndex];
                }
            }
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
    public void IncrementLegs() => ChangeAppearance(ClothingType.Legs, GetNextIndex(ClothingType.Legs));
    public void IncrementOutfit() => SetOutfit(GetNextIndex1());

    // Decrement methods
    public void DecrementTorso() => ChangeAppearance(ClothingType.Torso, GetPreviousIndex(ClothingType.Torso));
    public void DecrementHat() => ChangeAppearance(ClothingType.Hat, GetPreviousIndex(ClothingType.Hat));
    public void DecrementHead() => ChangeAppearance(ClothingType.Head, GetPreviousIndex(ClothingType.Head));
    public void DecrementLegs() => ChangeAppearance(ClothingType.Legs, GetPreviousIndex(ClothingType.Legs));
    public void DecrementOutfit() => SetOutfit(GetPreviousIndex1());

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
    private int GetNextIndex1()
    {
        outfitIndex += 1;
        if (outfitIndex > 3)
        {
            outfitIndex = 0;
        }
        return outfitIndex;
    }

    private int GetPreviousIndex1()
    {
        outfitIndex -= 1;
        if (outfitIndex < 0)
        {
            outfitIndex = 3;
        }
        return outfitIndex;
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