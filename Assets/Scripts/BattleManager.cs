using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DingoSystem;
using System.IO;
using SimpleJSON;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;


public class BattleManager : MonoBehaviour
{

    private static List<DingoID> randomDingos;
    private static List<DingoID> trainerDingos;
    private static bool trainerbattle;
    public int minRandomStat = -5; // Minimum value for randomizing stats
    public int maxRandomStat = 5;  // Maximum value for randomizing stats
    public int dingoID;
    public GameObject DingoMenu;
    public Text CatchText;
    public Text Status;
    public Text Status2;
    public Text Status3;
    public Text Status4;
    public Text Status5;
    public Text dingoType;
    public SpriteRenderer dingoImage;
    public Text dingoName;
    public Text dingoATK;
    public Text dingoDEF;
    public Text dingoSPD;
    public Text dingoHP;
    public int playerID;
    public Text playerType;
    public SpriteRenderer bingoImage;
    public SpriteRenderer playerImage;
    public SpriteRenderer attackImage1;
    public SpriteRenderer attackImage2;
    public SpriteRenderer attackImage3;
    public SpriteRenderer attackImage4;
    public SpriteRenderer attackImage5;
    public SpriteRenderer attackImage6;
    public SpriteRenderer attackImage7;
    public SpriteRenderer attackImage8;
    public SpriteRenderer attackImage9;
    public SpriteRenderer attackImage10;
    public SpriteRenderer attackImage11;
    public SpriteRenderer attackImage12;
    public SpriteRenderer attackImage13;
    public SpriteRenderer attackImage14;
    public SpriteRenderer attackImage15;
    public SpriteRenderer attackImage101;
    public SpriteRenderer attackImage102;
    public SpriteRenderer attackImage106;
    public SpriteRenderer attackImage107;
    public SpriteRenderer attackImage109;
    public SpriteRenderer attackImage110;
    public SpriteRenderer attackImage112;
    public SpriteRenderer attackImage115;
    public Text playerName;
    public Text playerATK;
    public Text playerDEF;
    public Text playerSPD;
    public Text playerHP;
    public Text playerMove1;
    public Text playerMove2;
    public Text playerMove3;
    public Text playerMove4;
    public Button playerMoveButton1;
    public Button playerMoveButton2;
    public Button playerMoveButton3;
    public Button playerMoveButton4;
    public Color Lightning;
    public Color Water;
    public Color Fire;
    public Color Ice;
    public Color Nature;
    public Color Ground;
    public Color Spirit;
    public Color Dark;
    public Color Abnormal;
    public Color Finance;
    public Color Wind;
    public Color Light;
    public HealthBar healthBar;
    public HealthBar healthBar2;
    public InputField inputField;
    public int maxHealth = 99;
    public int currentHealth = 99;
    public int playerHealth;
    public int maxplayerHealth;
    public DingoMove playermove1;
    public DingoMove playermove2;
    public DingoMove playermove3;
    public DingoMove playermove4;
    public DingoMove dingomove1;
    public DingoMove dingomove2;
    public DingoMove dingomove3;
    public DingoMove dingomove4;
    public DingoMove moveinfo;
    public int move1power;
    public int move2power;
    public int move3power;
    public int move4power;
    public int move1accuracy;
    public int move2accuracy;
    public int move3accuracy;
    public int move4accuracy;
    public int move1id;
    public int move2id;
    public int move3id;
    public int move4id;
    public int dingo1id;
    public int dingo2id;
    public int dingo3id;
    public int dingo4id;
    public int dingo1accuracy;
    public int dingo2accuracy;
    public int dingo3accuracy;
    public int dingo4accuracy;
    public string dingo1name;
    public string dingo2name;
    public string dingo3name;
    public string dingo4name;
    public string move1name;
    public string move2name;
    public string move3name;
    public string move4name;
    public int dingo1power;
    public int dingo2power;
    public int dingo3power;
    public int dingo4power;
    private bool canPlayerInput = true;
    public DingoID opponentDingo;
    public DingoID playerDingo;
    public int dingostatID;
    public int dingostatATK;
    public int dingostatDEF;
    public int dingostatSPD;
    public string dingostatType;
    public string dingostatName;
    public string dingostatDescription;
    public string dingostatSprite;
    public int dingostatXP;
    public int dingostatMaxXP;
    public int dingostatLevel;
    public int playerstatID;
    public int playerstatATK;
    public int playerstatDEF;
    public int playerstatSPD;
    public string playerstatType;
    public string playerstatName;
    public string playerstatDescription;
    public string playerstatSprite;
    public int playerstatXP;
    public int playerstatMaxXP;
    public int playerstatLevel;
    public int playerpreviousmoveID;
    public int opponentpreviousmoveID;
    public string filePath;
    public string jsonData;
    private JSONArray jsonDingos;
    public int AttackerHP;
    public int DefenderHP;
    public bool isAttackBackRunning;
    private List<StatusEffect> playerEffects;
    private List<StatusEffect> opponentEffects;
    private List<EnvironmentEffect> environmentEffects;
    public Transform PlayerDingoContent;
    public GameObject DingoItem;
    public ParticleSystem IceEffect;
    public ParticleSystem IceEffect2;
    public ParticleSystem MarshmellowEffect;
    public ParticleSystem MarshmellowEffect2;
    public ParticleSystem MarshmellowEffect3;
    public GameObject MarshmellowGameObjectEffect3;
    public ParticleSystem MarshmellowEffect103;
    public GameObject MarshmellowGameObjectEffect103;
    public ParticleSystem ShootingstarEffect;
    public GameObject ShootingstarGameObjectEffect;
    public ParticleSystem ShootingstarEffect2;
    public GameObject ShootingstarGameObjectEffect2;
    public ParticleSystem CashEffect;
    public GameObject CashEffectGameObject;
    public ParticleSystem CashEffect2;
    public GameObject CashEffectGameObject2;
    private int slotIndex2;
    private int moveDistance = 8;
    public float moveSpeed = 0.5f;
    public float jumpSpeed = 0.2f;
    public float animationTime = 0.2f;
    public int previousrandomIndex2;
    public BattleDialog dialogBox;
    public BattleDialog dialogBox2;
    public AutoHider autoHider;
    // Constructor
    public BattleManager()
    {
        // Initialize playerEffects list
        playerEffects = new List<StatusEffect>();
        opponentEffects = new List<StatusEffect>();
        environmentEffects = new List<EnvironmentEffect>();
    }
    public void Leave()
    {
        SavePlayerDingo();
        // Load the "SampleScene" when the button is clicked
        Loader.Load(Loader.Scene.SampleScene);
    }
    public Color GetTypeColor(string type, out Color textColor)
    {
        // Convert the type string to lowercase for case-insensitive comparison
        string typeLower = type.ToLower();

        // Check the type and return the corresponding color
        switch (typeLower)
        {
            case "lightning":
                textColor = Color.black;
                return Lightning;
            case "water":
                textColor = Color.black;
                return Water;
            case "fire":
                textColor = Color.black;
                return Fire;
            case "ice":
                textColor = Color.black;
                return Ice;
            case "nature":
                textColor = Color.black;
                return Nature;
            case "ground":
                textColor = Color.black;
                return Ground;
            case "spirit":
                textColor = Color.white;
                return Spirit;
            case "dark":
                textColor = Color.white;
                return Dark;
            case "abnormal":
                textColor = Color.black;
                return Abnormal;
            case "finance":
                textColor = Color.black;
                return Finance;
            case "wind":
                textColor = Color.black;
                return Wind;
            case "light":
                textColor = Color.black;
                return Light;
            default:
                Debug.LogError("Invalid type: " + type);
                textColor = Color.black; // Default to black text for unrecognized types
                return Color.white; // Return white for unrecognized types
        }
    }
    public void ChangeButtonColor(Button button, Color buttonColor, Color textColor)
    {
        // Ensure the button is not null
        if (button != null)
        {
            // Get the Image component of the button
            Image buttonImage = button.GetComponent<Image>();

            // If the button has an Image component, change its color
            if (buttonImage != null)
            {
                buttonImage.color = buttonColor;
            }
            else
            {
                Debug.LogError("Button does not have an Image component.");
            }

            // Get the Text component of the button
            Text buttonText = button.GetComponentInChildren<Text>();

            // If the button has a Text component, change its color
            if (buttonText != null)
            {
                buttonText.color = textColor;
            }
            else
            {
                Debug.LogError("Button does not have a Text component.");
            }
        }
        else
        {
            Debug.LogError("Button reference is null. Assign the button in the inspector.");
        }
    }
    public static void SetRandomDingos(List<DingoID> dingos)
    {
        randomDingos = dingos;
    }
    public static void SetTrainerDingos(bool istrainerbattle)
    {
        trainerbattle = istrainerbattle;
    }
    public static (DingoID, int) GetRandomDingos()
    {
        // Check if the list of dingos is not null and not empty
        if (randomDingos != null && randomDingos.Count > 0)
        {
            // Generate a random index within the range of the list
            int randomIndex = Random.Range(0, randomDingos.Count);
            // Get the dingo at the random index
            DingoID randomDingo = randomDingos[randomIndex];
            // Return the dingo at the random index along with the index itself
            return (randomDingo, randomIndex);
        }
        else
        {
            // If the list is null or empty, return null for the dingo and -1 for the index
            return (null, -1);
        }
    }
    public DingoID NextDingo(int randomIndex)
    {
        randomDingos.RemoveAt(randomIndex);
        // Check if the list of dingos is not null and not empty
        if (randomDingos != null && randomDingos.Count > 0)
        {
            // Generate a random index within the range of the list
            randomIndex = Random.Range(0, randomDingos.Count);
            // Return the dingo at the random index
            previousrandomIndex2 = randomIndex;
            return randomDingos[randomIndex];
        }
        else
        {
            Loader.Load(Loader.Scene.SampleScene);
            return null;
        }
    }
    public DingoID GetRandomDingo()
    {
        // Get the total number of Dingos in the database
        int totalDingos = DingoDatabase.GetTotalDingos();

        // Generate a random index between 0 and totalDingos - 1
        int randomIndex = UnityEngine.Random.Range(0, totalDingos);

        // Return the Dingo at the randomly generated index
        return DingoDatabase.GetDingoAtIndex(randomIndex);
    }
    public void Start()
    {
        StartCoroutine(LeftMovetoPosition());
        StartCoroutine(RightMovetoPosition());
        LoadPlayerDingoFromFile(0);
        if (jsonDingos == null || jsonDingos.Count == 0)
        {
            DingoID playerDingo = GetRandomDingo();
            DingoMove playermove1 = playerDingo.Moves[0];
            DingoMove playermove2 = playerDingo.Moves[1];
            DingoMove playermove3 = playerDingo.Moves[2];
            DingoMove playermove4 = playerDingo.Moves[3];
            move1id = playermove1.MoveID;
            move2id = playermove2.MoveID;
            move3id = playermove3.MoveID;
            move4id = playermove4.MoveID;
            move1power = playermove1.Power;
            move2power = playermove2.Power;
            move3power = playermove3.Power;
            move4power = playermove4.Power;
            move1name = playermove1.Name;
            move2name = playermove2.Name;
            move3name = playermove3.Name;
            move4name = playermove4.Name;
            move1accuracy = playermove1.Accuracy;
            move2accuracy = playermove2.Accuracy;
            move3accuracy = playermove3.Accuracy;
            move4accuracy = playermove4.Accuracy;
            playerHealth = playerDingo.HP;
            maxplayerHealth = playerDingo.MaxHP;
            playerMove1.text = playermove1.Name;
            playerMove2.text = playermove2.Name;
            playerMove3.text = playermove3.Name;
            playerMove4.text = playermove4.Name;
            playerName.text = playerDingo.Name;
            playerstatID = playerDingo.ID;
            playerstatATK = playerDingo.Attack;
            playerstatDEF = playerDingo.Defense;
            playerstatSPD = playerDingo.Speed;
            playerstatType = playerDingo.Type;
            playerstatName = playerDingo.Name;
            playerstatDescription = playerDingo.Description;
            playerstatSprite = playerDingo.Sprite;
            playerstatXP = playerDingo.XP;
            playerstatMaxXP = playerDingo.MaxXP;
            playerstatLevel = playerDingo.Level;
            playerType.text = "Type: " + playerDingo.Type;
            playerATK.text = ("Attack: " + playerDingo.Attack);
            playerDEF.text = ("Defense: " + playerDingo.Defense);
            playerSPD.text = ("Speed: " + playerDingo.Speed);
            playerHP.text = ("HP: " + playerHealth + "/" + maxplayerHealth);
            Sprite playerSprite = Resources.Load<Sprite>(playerDingo.Sprite);
            healthBar2.SetMaxHealth(maxplayerHealth);
            healthBar2.SetHealth(playerHealth);
            playerImage.sprite = playerSprite;
        }
        (DingoID opponentDingo, int previousrandomIndex) = GetRandomDingos();
        previousrandomIndex2 = previousrandomIndex;
        opponentDingo.Attack += UnityEngine.Random.Range(minRandomStat, maxRandomStat); 
        opponentDingo.Defense += UnityEngine.Random.Range(minRandomStat, maxRandomStat);
        opponentDingo.Speed += UnityEngine.Random.Range(minRandomStat, maxRandomStat);
        SetOpponentDingo(opponentDingo);
        opponentpreviousmoveID = 99;
        playerpreviousmoveID = 99;
    }
    public void SetOpponentDingo(DingoID opponentDingo)
    {
        DingoMove dingomove1 = opponentDingo.Moves[0];
        DingoMove dingomove2 = opponentDingo.Moves[1];
        DingoMove dingomove3 = opponentDingo.Moves[2];
        DingoMove dingomove4 = opponentDingo.Moves[3];
        currentHealth = opponentDingo.HP;
        maxHealth = opponentDingo.MaxHP;

        healthBar.SetMaxHealth(maxHealth);

        dingo1name = dingomove1.Name;
        dingo2name = dingomove2.Name;
        dingo3name = dingomove3.Name;
        dingo4name = dingomove4.Name;
        dingo1power = dingomove1.Power;
        dingo2power = dingomove2.Power;
        dingo3power = dingomove3.Power;
        dingo4power = dingomove4.Power;
        dingo1id = dingomove1.MoveID;
        dingo2id = dingomove2.MoveID;
        dingo3id = dingomove3.MoveID;
        dingo4id = dingomove4.MoveID;
        dingo1accuracy = dingomove1.Accuracy;
        dingo2accuracy = dingomove2.Accuracy;
        dingo3accuracy = dingomove3.Accuracy;
        dingo4accuracy = dingomove4.Accuracy;
        dingostatID = opponentDingo.ID;
        dingostatATK = opponentDingo.Attack;
        dingostatDEF = opponentDingo.Defense;
        dingostatSPD = opponentDingo.Speed;
        dingostatType = opponentDingo.Type;
        dingostatName = opponentDingo.Name;
        dingostatDescription = opponentDingo.Description;
        dingostatSprite = opponentDingo.Sprite;
        dingostatXP = opponentDingo.XP;
        dingostatMaxXP = opponentDingo.MaxXP;
        dingostatLevel = opponentDingo.Level;
        dingoName.text = opponentDingo.Name;
        dingoATK.text = ("Attack: " + opponentDingo.Attack);
        dingoDEF.text = ("Defense: " + opponentDingo.Defense);
        dingoSPD.text = ("Speed: " + opponentDingo.Speed);
        dingoHP.text = ("HP: " + opponentDingo.HP + "/" + opponentDingo.MaxHP);
        dingoType.text = ("Type: " + opponentDingo.Type);
        Sprite dingoSprite = Resources.Load<Sprite>(opponentDingo.Sprite);


        dingoImage.sprite = dingoSprite;
        healthBar.SetHealth(currentHealth);
    }
    public void LoadPlayerDingoFromFile(int slot)
    {
        filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
        if (File.Exists(filePath))
        {
            jsonData = File.ReadAllText(filePath);
            jsonDingos = JSON.Parse(jsonData) as JSONArray;
            if (jsonDingos != null && jsonDingos.Count > 0)
            {
                JSONObject firstDingo = jsonDingos[slot].AsObject;
                LoadPlayerDingoFromJsonObject(firstDingo);
            }
        }
    }
    public void LoadPlayerDingoFromJsonObject(JSONObject dingoData)
    {
        DingoID playerDingo = new DingoID(
            dingoData["DingoID"],
            dingoData["Name"],
            dingoData["Type"],
            dingoData["Description"],
            dingoData["CurrentHealth"],
            dingoData["ATK"],
            dingoData["DEF"],
            dingoData["SPD"],
            dingoData["Sprite"],
            dingoData["MaxHealth"],
            dingoData["XP"],
            dingoData["MaxXP"],
            dingoData["Level"]);

        DingoID playerDingoMoves = DingoDatabase.GetDingoByID(dingoData["DingoID"]);
        DingoMove[] moves = new DingoMove[]
        {
        DingoDatabase.GetMoveByID(dingoData["Move1ID"], playerDingoMoves),
        DingoDatabase.GetMoveByID(dingoData["Move2ID"], playerDingoMoves),
        DingoDatabase.GetMoveByID(dingoData["Move3ID"], playerDingoMoves),
        DingoDatabase.GetMoveByID(dingoData["Move4ID"], playerDingoMoves)
        };

        InitializePlayerDingo(playerDingo, moves);
    }
    public void InitializePlayerDingo(DingoID playerDingo, DingoMove[] moves)
    {
        move1power = moves[0].Power;
        move2power = moves[1].Power;
        move3power = moves[2].Power;
        move4power = moves[3].Power;
        move1accuracy = moves[0].Accuracy;
        move2accuracy = moves[1].Accuracy;
        move3accuracy = moves[2].Accuracy;
        move4accuracy = moves[3].Accuracy;
        playerHealth = playerDingo.HP;
        maxplayerHealth = playerDingo.MaxHP;
        playerMove1.text = moves[0].Name;
        playerMove2.text = moves[1].Name;
        playerMove3.text = moves[2].Name;
        playerMove4.text = moves[3].Name;
        move1name = moves[0].Name;
        move2name = moves[1].Name;
        move3name = moves[2].Name;
        move4name = moves[3].Name;
        move1id = moves[0].MoveID;
        move2id = moves[1].MoveID;
        move3id = moves[2].MoveID;
        move4id = moves[3].MoveID;
        ChangeButtonColor(playerMoveButton1, GetTypeColor(moves[0].Type, out Color textColor), textColor);
        ChangeButtonColor(playerMoveButton2, GetTypeColor(moves[1].Type, out textColor), textColor);
        ChangeButtonColor(playerMoveButton3, GetTypeColor(moves[2].Type, out textColor), textColor);
        ChangeButtonColor(playerMoveButton4, GetTypeColor(moves[3].Type, out textColor), textColor);
        playerName.text = playerDingo.Name;
        playerType.text = "Type: " + playerDingo.Type;
        playerATK.text = ("Attack: " + playerDingo.Attack);
        playerDEF.text = ("Defense: " + playerDingo.Defense);
        playerSPD.text = ("Speed: " + playerDingo.Speed);
        playerHP.text = ("HP: " + playerHealth + "/" + maxplayerHealth);
        Sprite playerSprite = Resources.Load<Sprite>(playerDingo.Sprite);
        healthBar2.SetMaxHealth(maxplayerHealth);
        healthBar2.SetHealth(playerHealth);
        playerImage.sprite = playerSprite;
        playerstatID = playerDingo.ID;
        playerstatATK = playerDingo.Attack;
        playerstatDEF = playerDingo.Defense;
        playerstatSPD = playerDingo.Speed;
        playerstatType = playerDingo.Type;
        playerstatName = playerDingo.Name;
        playerstatDescription = playerDingo.Description;
        playerstatSprite = playerDingo.Sprite;
        playerstatXP = playerDingo.XP;
        playerstatMaxXP = playerDingo.MaxXP;
        playerstatLevel = playerDingo.Level;
        DingoMenu.SetActive(false);
        canPlayerInput = true;
    }
    public void ListDingos()
    {
        // Clear existing Dingo items before populating the list
        foreach (Transform child in PlayerDingoContent)
        {
            Destroy(child.gameObject);
        }

        int dingoCount = 0;
        if (File.Exists(filePath))
        {
            try
            {
                if (jsonDingos != null)
                {
                    foreach (JSONNode dingoData in jsonDingos)
                    {
                        JSONObject dingo = dingoData.AsObject;
                        GameObject obj = Instantiate(DingoItem, PlayerDingoContent);
                        var dingoName = obj.transform.Find("ItemName").GetComponent<Text>();
                        var dingoIcon = obj.transform.Find("ItemIcon").GetComponent<UnityEngine.UI.Image>();
                        var dingoType = obj.transform.Find("ItemType").GetComponent<Text>();
                        var dingoID = obj.transform.Find("ItemID").GetComponent<Text>();

                        dingoName.text = dingo["Name"];
                        dingoType.text = dingo["Type"];
                        dingoID.text = dingoCount.ToString();
                        dingoCount++;

                        // Load Dingo icon sprite from Resources folder based on the icon path in JSON data
                        string iconPath = dingo["Sprite"];
                        Sprite iconSprite = Resources.Load<Sprite>(iconPath);
                        if (iconSprite != null)
                        {
                            dingoIcon.sprite = iconSprite;
                        }
                        else
                        {
                            Debug.LogWarning("Failed to load Dingo icon: " + iconPath);
                        }

                        // Add a click event handler to each Dingo item
                        int dingoIndex = dingoCount - 1; // Adjust for zero-based indexing
                        obj.GetComponent<Button>().onClick.AddListener(() => OnDingoItemClick(dingoIndex));
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to parse JSON data.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error reading JSON file: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Dingo JSON file not found at path: " + filePath);
        }
    }
    private void OnDingoItemClick(int dingoIndex)
    {
        Debug.Log("Dingo item clicked: " + dingoIndex);
        // Call Use2nd() script with the selected Dingo index
        StartCoroutine(UseNewDingo(dingoIndex));
    }
    IEnumerator UseNewDingo(int dingoIndex)
    {
        yield return StartCoroutine(LeftMoveoutPosition());
        Use2nd(dingoIndex);
        yield return StartCoroutine(LeftMovetoPosition());
    }
    public void Move1()
    {
        // Check if player input is allowed
        if (!canPlayerInput)
        {
            return; // Exit the method if input is blocked
        }
        UseMove(move1id, move1power, move1accuracy, move1name);
    }
    public void Move2()
    {
        // Check if player input is allowed
        if (!canPlayerInput)
        {
            return; // Exit the method if input is blocked
        }
        UseMove(move2id, move2power, move2accuracy, move2name);
    }
    public void Move3()
    {
        // Check if player input is allowed
        if (!canPlayerInput)
        {
            return; // Exit the method if input is blocked
        }
        UseMove(move3id, move3power, move3accuracy, move3name);
    }
    public void Move4()
    {
        // Check if player input is allowed
        if (!canPlayerInput)
        {
            return; // Exit the method if input is blocked
        }
        UseMove(move4id, move4power, move4accuracy, move4name);
    }
    public void UseMove(int moveNumber, int movePower, float moveAccuracy, string moveName)
    {
        canPlayerInput = false;
        StartCoroutine(PerformMoveRoutine(moveNumber, movePower, moveAccuracy, moveName));
    }
    public void CatchDingo()
    {
        if (!canPlayerInput)
        {
            return; // Exit the method if input is blocked
        }
        if (trainerbattle)
        {
            dialogBox2.gameObject.SetActive(true);
            //StartCoroutine(dialogBox2.TypeDialog("You cannot catch Dingos that belong to someone else"));
            autoHider.Bingo();
            return; // Exit the method if input is blocked
        }
        canPlayerInput = false;
        StartCoroutine(CatchAnimation());
    }
    private IEnumerator PerformMoveRoutine(int moveNumber, int movePower, float moveAccuracy, string moveName)
    {
        CashEffectGameObject.SetActive(false);
        CashEffectGameObject2.SetActive(false);
        int opponentMoveNumber = DetermineOpponentMove();
        DingoID dingo = DingoDatabase.GetDingoByID(dingostatID);
        DingoMove opponentMove = DingoDatabase.GetMoveByID(opponentMoveNumber, dingo);
        DingoID playerdingo = DingoDatabase.GetDingoByID(playerstatID);
        DingoMove playerMove = DingoDatabase.GetMoveByID(moveNumber, playerdingo);
        if (playerpreviousmoveID == moveNumber)
        {
            dialogBox2.gameObject.SetActive(true);
            //StartCoroutine(dialogBox2.TypeDialog("Using the same move consecutively causes any special effects to not work"));
            autoHider.Bingo();
        }
        else
        {
            switch (moveName)
            {
                case "Sneaky Shell":
                case "Cosmic Shield":
                case "Sweet Shield":
                    playerstatDEF += 1000;
                    break;
            }
        }
        if (opponentpreviousmoveID == opponentMoveNumber)
        {
            Debug.Log("Opponent used " + opponentMove.Name + " for the second time");
        }
        else
        {
            switch (opponentMove.Name)
            {
                case "Sneaky Shell":
                case "Cosmic Shield":
                case "Sweet Shield":
                    dingostatDEF += 1000;
                    break;
            }
        }
        yield return StartCoroutine(DecideHighPriorityAnimationCoroutine(moveName));
        yield return StartCoroutine(DecideHighPriorityOpponentAnimationCoroutine(opponentMove.Name));
        bool playerGoesFirst = playerstatSPD > dingostatSPD;
        if (playerGoesFirst)
        {
            yield return StartCoroutine(PlayerAttackFirst(moveNumber, movePower, moveAccuracy, moveName, opponentMoveNumber, opponentMove.Type, playerMove.Type, dingo.Type));
        }
        else
        {
            yield return StartCoroutine(PlayerAttackSecond(moveNumber, movePower, moveAccuracy, moveName, opponentMoveNumber, opponentMove.Type, playerMove.Type, dingo.Type));
        }
        StartCoroutine(StopEffects());
        if (playerpreviousmoveID == moveNumber)
        {
            playerpreviousmoveID = 99;
        }
        else
        {
            switch (moveName)
            {
                case "Sneaky Shell":
                case "Cosmic Shield":
                case "Sweet Shield":
                    playerstatDEF -= 1000;
                    break;
            }
        }
        if (opponentpreviousmoveID == opponentMoveNumber)
        {
            opponentpreviousmoveID = 99;
        }
        else
        {
            switch (opponentMove.Name)
            {
                case "Sneaky Shell":
                case "Cosmic Shield":
                case "Sweet Shield":
                    dingostatDEF -= 1000;
                    break;
            }
        }
        playerpreviousmoveID = moveNumber;
        opponentpreviousmoveID = opponentMoveNumber;
    }
    private static readonly Dictionary<string, int> TypeToIntMapping = new Dictionary<string, int>
    {
        { "Lightning", 0 },
        { "Water", 1 },
        { "Fire", 2 },
        { "Ice", 3 },
        { "Nature", 4 },
        { "Ground", 5 },
        { "Dark", 6 },
        { "Abnormal", 7 },
        { "Spirit", 8 }
    };
    public float CalculateTypeModifier(string attackerType, string defenderType)
    {
        // Ignore types "Light", "Finance", and "Wind"
        if (attackerType == "Light" || attackerType == "Finance" || attackerType == "Wind" ||
            defenderType == "Light" || defenderType == "Finance" || defenderType == "Wind")
        {
            return 1f; // Return neutral effectiveness if either type is "Light", "Finance", or "Wind"
        }

        // Convert attackerType and defenderType strings to corresponding integers
        if (!TypeToIntMapping.TryGetValue(attackerType, out int attackerInt))
        {
            Debug.LogError("Invalid attacker type: " + attackerType);
            return 1f; // Default to neutral effectiveness if attacker type is invalid
        }

        if (!TypeToIntMapping.TryGetValue(defenderType, out int defenderInt))
        {
            Debug.LogError("Invalid defender type: " + defenderType);
            return 1f; // Default to neutral effectiveness if defender type is invalid
        }

        // Get the effectiveness multiplier from the DingoTypeEffectivenessCalculator
        float effectiveness = DingoTypeEffectivenessCalculator.GetEffectiveness((DingoType)attackerInt, (DingoType)defenderInt);

        // Return the effectiveness multiplier
        return effectiveness;
    }
    public IEnumerator PerformMove(int moveNumber, int movePower, float moveAccuracy, string moveName, int attackerattack, int defenderdefense, string moveType, string defendertype, List<StatusEffect> attackerEffects, List<StatusEffect> defenderEffects, List<EnvironmentEffect> environmentEffects)
    {
        Status3.text = Status2.text;
        Status2.text = Status.text;
        attackerEffects = new List<StatusEffect>();
        defenderEffects = new List<StatusEffect>();
        environmentEffects = new List<EnvironmentEffect>();
        float accuracy = moveAccuracy / 100f; // Convert accuracy from percentage to decimal

        bool moveHits = Random.Range(0f, 1f) <= accuracy;
        bool isRainActive = false;
        foreach (var environmentEffect in environmentEffects)
        {
            if (environmentEffect.Name == "Rain")
            {
                isRainActive = true;
                break;
            }
        }

        // Apply attacker's attack and defender's defense along with any bonuses
        int attackerAttackWithBonus = attackerattack + CalculateAttackBonus(attackerEffects);

        // Boost attacker's attack if "Rain" effect is active and moveType is "water"
        if (isRainActive && moveType == "water")
        {
            attackerAttackWithBonus = Mathf.RoundToInt(attackerAttackWithBonus * 1.5f);
        }

        int defenderDefenseWithBonus = defenderdefense + CalculateDefenseBonus(defenderEffects);

        if (movePower > 0)
        {
            if (moveHits)
            {
                // Apply attacker's attack and defender's defense along with any bonuses
                attackerAttackWithBonus = attackerattack + CalculateAttackBonus(attackerEffects);
                defenderDefenseWithBonus = defenderdefense + CalculateDefenseBonus(defenderEffects);

                // Calculate damage
                float modifier = (((attackerAttackWithBonus - defenderDefenseWithBonus) / 100f) + 1);
                float effectiveness = CalculateTypeModifier(moveType, defendertype);
                int damageAmount = Mathf.Max(Mathf.RoundToInt((modifier * movePower) * effectiveness), 1);
                Debug.Log(moveName + " is type " + moveType + ". It did x" + effectiveness + " against " + defendertype);
                DefenderHP -= damageAmount; // Apply damage

                string bingo = moveName + " did " + damageAmount + " damage.";
                if(effectiveness == 0.5f)
                {
                    bingo = moveName + " did " + damageAmount + " with x" + effectiveness + " damage against " + defendertype;
                }
                if (effectiveness == 2f)
                {
                    bingo = moveName + " did " + damageAmount + " with x" + effectiveness + " damage against " + defendertype;
                }

                //yield return StartCoroutine(dialogBox.TypeDialog(bingo));
            }
            else
            {
                //yield return StartCoroutine(dialogBox.TypeDialog(moveName + " missed!"));
            }
        }
        else
        {
            //yield return StartCoroutine(dialogBox.TypeDialog(moveName + " used a special effect."));
        }
        yield return null;
    }
    private IEnumerator PlayerAttackFirst(int moveNumber, int movePower, float moveAccuracy, string moveName, int enemyMoveNumber, string opponentmoveType, string playermoveType, string opponentType)
    {
        DefenderHP = currentHealth;
        if (moveName == "Aero Slicer")
        {
            int numberOfHits = Random.Range(1, 5); // Randomly determine the number of hits between 2 and 6
            for (int i = 0; i < numberOfHits; i++)
            {
                yield return StartCoroutine(DecideAnimationCoroutine(moveName));
                yield return StartCoroutine(PerformMove(moveNumber, movePower, moveAccuracy, moveName, playerstatATK, dingostatDEF, playermoveType, opponentType, playerEffects, opponentEffects, environmentEffects));
                currentHealth = DefenderHP;
                UpdateUI();
                yield return new WaitForSeconds(0.4f);
            }
        }
        yield return StartCoroutine(DecideAnimationCoroutine(moveName));
        yield return StartCoroutine(PerformMove(moveNumber, movePower, moveAccuracy, moveName, playerstatATK, dingostatDEF, playermoveType, opponentType, playerEffects, opponentEffects, environmentEffects));
        currentHealth = DefenderHP;
        UpdateUI();
        if (currentHealth <= 0)
        {
            SavePlayerDingo();
            if (trainerbattle)
            {
                yield return StartCoroutine(RightMoveoutPosition());
                DingoID opponentDingo = NextDingo(previousrandomIndex2);
                SetOpponentDingo(opponentDingo);
                yield return StartCoroutine(RightMovetoPosition());
            }
            else
            {
                Loader.Load(Loader.Scene.SampleScene);
            }
        }
        yield return new WaitForSeconds(1f);
        //yield return StartCoroutine(DecideOpponentAnimationCoroutine(moveName));
        CashEffectGameObject.SetActive(false);
        yield return StartCoroutine(AttackBack(enemyMoveNumber, opponentmoveType));
        UpdateUI();
        if (playerHealth <= 0)
        {
            SavePlayerDingo();
            ListDingos(); // Example scene reload
            DingoMenu.SetActive(true);
        }
        else
        {
            canPlayerInput = true;
        }
    }
    private IEnumerator PlayerAttackSecond(int moveNumber, int movePower, float moveAccuracy, string moveName, int enemyMoveNumber, string opponentmoveType, string playermoveType, string opponentType)
    {
        //yield return StartCoroutine(DecideOpponentAnimationCoroutine(moveName));
        yield return StartCoroutine(AttackBack(enemyMoveNumber, opponentmoveType));
        UpdateUI();
        yield return new WaitForSeconds(1f);
        if (playerHealth <= 0)
        {
            SavePlayerDingo();
            ListDingos(); // Example scene reload
            DingoMenu.SetActive(true);
            yield break;
        }

        DefenderHP = currentHealth;
        if (moveName == "Aero Slicer")
        {
            int numberOfHits = Random.Range(1, 5); // Randomly determine the number of hits between 2 and 6
            for (int i = 0; i < numberOfHits; i++)
            {
                yield return StartCoroutine(DecideAnimationCoroutine(moveName));
                yield return StartCoroutine(PerformMove(moveNumber, movePower, moveAccuracy, moveName, playerstatATK, dingostatDEF, playermoveType, opponentType, playerEffects, opponentEffects, environmentEffects));
                currentHealth = DefenderHP;
                UpdateUI();
                yield return new WaitForSeconds(0.4f);
            }
        }
        yield return StartCoroutine(DecideAnimationCoroutine(moveName));
        yield return StartCoroutine(PerformMove(moveNumber, movePower, moveAccuracy, moveName, playerstatATK, dingostatDEF, playermoveType, opponentType, playerEffects, opponentEffects, environmentEffects));
        currentHealth = DefenderHP;
        UpdateUI();
        if (currentHealth <= 0)
        {
            SavePlayerDingo();
            if (trainerbattle)
            {
                yield return StartCoroutine(RightMoveoutPosition());
                DingoID opponentDingo = NextDingo(previousrandomIndex2);
                SetOpponentDingo(opponentDingo);
                yield return StartCoroutine(RightMovetoPosition());
            }
            else
            {
                Loader.Load(Loader.Scene.SampleScene);
            }
        }
        canPlayerInput = true;
    }
    IEnumerator LeftMovetoPosition()
    {
        Vector3 originalPosition = new Vector3(-5.35f, -0.5f, 0);
        Vector3 outofboundsPosition = new Vector3(-13, 0, 0);
        float startTime = Time.time;
        while (Time.time - startTime < (moveSpeed * 2f))
        {
            float fracJourney = (Time.time - startTime) / (moveSpeed * 2f);
            playerImage.transform.localPosition = Vector3.Lerp(outofboundsPosition, originalPosition, fracJourney);
            yield return null;
        }
    }
    IEnumerator RightMovetoPosition()
    {
        Vector3 originalPosition = new Vector3(5.35f, -0.5f, 0);
        Vector3 outofboundsPosition = new Vector3(13, 0, 0);
        float startTime = Time.time;
        while (Time.time - startTime < (moveSpeed * 2f))
        {
            float fracJourney = (Time.time - startTime) / (moveSpeed * 2f);
            dingoImage.transform.localPosition = Vector3.Lerp(outofboundsPosition, originalPosition, fracJourney);
            yield return null;
        }
    }
    IEnumerator LeftMoveoutPosition()
    {
        Vector3 originalPosition = new Vector3(-5.35f, -0.5f, 0);
        Vector3 outofboundsPosition = new Vector3(-13, 0, 0);
        float startTime = Time.time;
        while (Time.time - startTime < (moveSpeed * 2f))
        {
            float fracJourney = (Time.time - startTime) / (moveSpeed * 2f);
            playerImage.transform.localPosition = Vector3.Lerp(originalPosition, outofboundsPosition, fracJourney);
            yield return null;
        }
    }
    IEnumerator RightMoveoutPosition()
    {
        Vector3 originalPosition = new Vector3(5.35f, -0.5f, 0);
        Vector3 outofboundsPosition = new Vector3(13, 0, 0);
        float startTime = Time.time;
        while (Time.time - startTime < (moveSpeed * 2f))
        {
            float fracJourney = (Time.time - startTime) / (moveSpeed * 2f);
            dingoImage.transform.localPosition = Vector3.Lerp(originalPosition, outofboundsPosition, fracJourney);
            yield return null;
        }
    }
    IEnumerator AttackBack(int moveNumber, string movetype)
    {
        DingoID dingo = DingoDatabase.GetDingoByID(dingostatID);
        DingoMove moveinfo = DingoDatabase.GetMoveByID(moveNumber, dingo);
        isAttackBackRunning = true;
        canPlayerInput = false;
        if (moveinfo.Name == "Aero Slicer")
        {
            int numberOfHits = Random.Range(1, 5); // Randomly determine the number of hits between 2 and 6
            for (int i = 0; i < numberOfHits; i++)
            {
                yield return StartCoroutine(DecideOpponentAnimationCoroutine(moveinfo.Name));
                switch (moveNumber)
                {
                    case 0:
                        DefenderHP = playerHealth;
                        yield return StartCoroutine(PerformMove(1, dingo1power, dingo1accuracy, dingo1name, dingostatATK, playerstatDEF, movetype, playerstatType, opponentEffects, playerEffects, environmentEffects));
                        playerHealth = DefenderHP;
                        break;
                    case 1:
                        DefenderHP = playerHealth;
                        yield return StartCoroutine(PerformMove(2, dingo2power, dingo2accuracy, dingo2name, dingostatATK, playerstatDEF, movetype, playerstatType, opponentEffects, playerEffects, environmentEffects));
                        playerHealth = DefenderHP;
                        break;
                    case 2:
                        DefenderHP = playerHealth;
                        yield return StartCoroutine(PerformMove(3, dingo3power, dingo3accuracy, dingo3name, dingostatATK, playerstatDEF, movetype, playerstatType, opponentEffects, playerEffects, environmentEffects));
                        playerHealth = DefenderHP;
                        break;
                    case 3:
                        DefenderHP = playerHealth;
                        yield return StartCoroutine(PerformMove(4, dingo4power, dingo4accuracy, dingo4name, dingostatATK, playerstatDEF, movetype, playerstatType, opponentEffects, playerEffects, environmentEffects));
                        playerHealth = DefenderHP;
                        break;
                    default:
                        Debug.LogError("Invalid move selected!");
                        break;
                }
                UpdateUI();
                yield return new WaitForSeconds(0.4f);
            }
        }
        yield return StartCoroutine(DecideOpponentAnimationCoroutine(moveinfo.Name));
        yield return new WaitForSeconds(0.4f);
        switch (moveNumber)
        {
            case 0:
                DefenderHP = playerHealth;
                yield return StartCoroutine(PerformMove(1, dingo1power, dingo1accuracy, dingo1name, dingostatATK, playerstatDEF, movetype, playerstatType, opponentEffects, playerEffects, environmentEffects));
                playerHealth = DefenderHP;
                break;
            case 1:
                DefenderHP = playerHealth;
                yield return StartCoroutine(PerformMove(2, dingo2power, dingo2accuracy, dingo2name, dingostatATK, playerstatDEF, movetype, playerstatType, opponentEffects, playerEffects, environmentEffects));
                playerHealth = DefenderHP;
                break;
            case 2:
                DefenderHP = playerHealth;
                yield return StartCoroutine(PerformMove(3, dingo3power, dingo3accuracy, dingo3name, dingostatATK, playerstatDEF, movetype, playerstatType, opponentEffects, playerEffects, environmentEffects));
                playerHealth = DefenderHP;
                break;
            case 3:
                DefenderHP = playerHealth;
                yield return StartCoroutine(PerformMove(4, dingo4power, dingo4accuracy, dingo4name, dingostatATK, playerstatDEF, movetype, playerstatType, opponentEffects, playerEffects, environmentEffects));
                playerHealth = DefenderHP;
                break;
            default:
                Debug.LogError("Invalid move selected!");
                break;
        }
        isAttackBackRunning = false;
    }
    private int DetermineOpponentMove()
    {
        // Generate a random number between 0 and 3
        int randomMove = Random.Range(0, 4);
        return randomMove;
    }
    public List<StatusEffect> CheckMoveForStatusEffect(string movename, List<StatusEffect> attackereffects, List<StatusEffect> defendereffects, List<EnvironmentEffect> environmenteffects)
    {
        attackereffects = new List<StatusEffect>();
        defendereffects = new List<StatusEffect>();
        environmenteffects = new List<EnvironmentEffect>();
        // Now it's safe to add status effects to attackereffects and defendereffects
        // Example logic to determine if the move applies a status effect based on move ID and accuracy
        switch (movename)
        {
            case "Rain":
                environmenteffects.Add(DingoDatabase.Rain);
                break;
            default:
                break;
        }

        return attackereffects;

    }
    private int CalculateAttackBonus(List<StatusEffect> effects)
    {
        int attackBonus = 0;
        foreach (StatusEffect effect in effects)
        {
            // Add logic here to determine attack bonus based on status effects
        }
        return attackBonus;
    }
    private int CalculateDefenseBonus(List<StatusEffect> effects)
    {
        int defenseBonus = 0;
        foreach (StatusEffect effect in effects)
        {
            // Add logic here to determine defense bonus based on status effects
        }
        return defenseBonus;
    }
    private void UpdateUI()
    {
        healthBar.SetHealth(currentHealth);
        healthBar2.SetHealth(playerHealth);
        playerHP.text = ("HP: " + playerHealth + "/" + maxplayerHealth);
        dingoHP.text = "HP: " + currentHealth + "/" + maxHealth;
    }
    public void EvaluateStatusEffects(List<StatusEffect> StatusEffects)
    {
        for (int i = StatusEffects.Count - 1; i >= 0; i--)
        {
            StatusEffect effect = StatusEffects[i];

            // Check if the current status effect is ShellShield
            if (effect.ID == 0)
            {
                Debug.Log("goo");
            }
            // Add cases for other status effects as needed

            // Decrease the duration of the status effect
            effect.Duration--;

            // If the duration reaches 0, remove the status effect
            if (effect.Duration <= 0)
            {
                StatusEffects.RemoveAt(i);
                Debug.Log(effect.Name + " status effect has worn off.");
            }
        }
    }
    public void EvaluateEnvironmentEffects(List<EnvironmentEffect> EnvironmentEffects)
    {
        for (int i = EnvironmentEffects.Count - 1; i >= 0; i--)
        {
            EnvironmentEffect effect = EnvironmentEffects[i];

            // Check if the current effect is Rain
            if (effect.ID == 0)
            {
                // Handle the Rain effect here
                // For example, you can apply its effect on the game environment
                ApplyRainEffect();
            }
            // Add cases for other environment effects as needed

            // Decrease the duration of the effect
            effect.Duration--;

            // If the duration reaches 0, remove the effect
            if (effect.Duration <= 0)
            {
                EnvironmentEffects.RemoveAt(i);
                Debug.Log(effect.Name + " effect has worn off.");
            }
        }
    }
    public GameObject Rain;
    private void ApplyRainEffect()
    {
        Debug.Log("bingo");
        Rain.SetActive(true);
    }
    private void ApplyCampfireEffect()
    {
        //Campfire.SetActive(true);
    }
    public void SavePlayerDingo()
    {
        // Check if slotIndex2 is defined
        if (slotIndex2 >= 0)
        {
            // Load existing Dingos data if it exists
            JSONArray jsonDingos;
            string filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
            if (File.Exists(filePath))
            {
                string existingData = File.ReadAllText(filePath);
                jsonDingos = JSON.Parse(existingData) as JSONArray;
                if (jsonDingos == null)
                {
                    // If parsing fails, create a new JSONArray
                    Debug.LogWarning("Failed to parse existing Dingos data. Creating a new JSONArray.");
                    jsonDingos = new JSONArray();
                }
            }
            else
            {
                // If file doesn't exist, create a new JSONArray
                Debug.LogWarning("Dingos data file not found. Creating a new JSONArray.");
                jsonDingos = new JSONArray();
            }

            // Debug information
            Debug.Log("Loaded existing Dingos data from file: " + filePath);
            Debug.Log("Number of existing Dingos: " + jsonDingos.Count);

            // Create JSON object for the new Dingo
            JSONObject jsonDingo = new JSONObject();
            playerstatXP = playerstatXP + 10;
            if (playerstatXP >= playerstatMaxXP)
            {
                // Level up the Dingo
                playerstatLevel++;
                CalculateNextLevelMaxXP(playerstatLevel);

                jsonDingo.Add("SkillPoints", jsonDingo["SkillPoints"]+ 5);
                // Reset XP to 0
                playerstatXP = 0;

                Debug.Log("Dingo leveled up to level " + playerstatLevel);
            }

            DingoID dingo = new DingoID(playerstatID, playerstatName, playerstatType, playerstatDescription, playerHealth, playerstatATK, playerstatDEF, playerstatSPD, playerstatSprite, maxplayerHealth, playerstatXP, playerstatMaxXP, playerstatLevel);
            // Debug information
            Debug.Log("New Dingo created with ID: " + slotIndex2);

            // Add properties of the Dingo object to the JSON object
            jsonDingo.Add("ID", slotIndex2);
            jsonDingo.Add("DingoID", dingo.ID);
            jsonDingo.Add("Name", dingo.Name);
            jsonDingo.Add("Type", dingo.Type);
            jsonDingo.Add("Description", dingo.Description);
            jsonDingo.Add("CurrentHealth", dingo.HP);
            jsonDingo.Add("ATK", dingo.Attack);
            jsonDingo.Add("DEF", dingo.Defense);
            jsonDingo.Add("SPD", dingo.Speed);
            jsonDingo.Add("Sprite", dingo.Sprite);
            jsonDingo.Add("MaxHealth", dingo.MaxHP);
            jsonDingo.Add("XP", dingo.XP);
            jsonDingo.Add("MaxXP", dingo.MaxXP);
            jsonDingo.Add("Level", dingo.Level);
            jsonDingo.Add("Move1ID", move1id);
            jsonDingo.Add("Move2ID", move2id);
            jsonDingo.Add("Move3ID", move3id);
            jsonDingo.Add("Move4ID", move4id);

            // Overwrite the specified slot with the new Dingo object
            if (jsonDingos.Count > slotIndex2)
            {
                jsonDingos[slotIndex2] = jsonDingo;
            }
            else
            {
                // Add the current Dingo object to the JSON array
                jsonDingos.Add(jsonDingo);
            }

            // Debug information
            Debug.Log("Dingo added to JSON array.");

            // Convert the JSON array to a string
            string jsonString = jsonDingos.ToString();

            // Write the JSON data to the file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(jsonString);
            }

            Debug.Log("Dingos data saved to: " + filePath);
        }
        else
        {
            Debug.LogError("slotIndex2 is not defined or invalid.");
        }
    }
    public void Display()
    {
        Debug.Log(slotIndex2);
    }    
    private void Use2nd(int dingoIndex)
    {
        if (jsonDingos != null && dingoIndex >= 0 && dingoIndex < jsonDingos.Count)
        {
            try
            {
                // Get the selected Dingo data from the JSON array
                JSONNode dingoData = jsonDingos[dingoIndex];
                JSONObject dingo = dingoData.AsObject;

                // Extract relevant information from the Dingo JSON object
                int dingoID = dingo["DingoID"];
                string dingoName = dingo["Name"];

                // Example: Load the Dingo with ID and name
                Debug.Log("Loading Dingo: ID = " + dingoID + ", Name = " + dingoName);
                LoadPlayerDingoFromFile(dingoIndex);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading Dingo: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Invalid Dingo index: " + dingoIndex);
        }
        slotIndex2 = dingoIndex;
    }
    public void CalculateNextLevelMaxXP(int currentLevel)
    {
        // Define a simple leveling curve
        // This could be replaced with a more complex formula if needed
        int baseXP = 100; // Base XP required to reach level 1
        int increasePerLevel = 50; // Increase in XP required per level

        // Calculate the total XP required to reach the current level
        int totalXPRequired = baseXP + (increasePerLevel * (currentLevel - 1));

        playerstatMaxXP = totalXPRequired;
    }
    public void SaveDingo()
    {
        int slotIndex = 1; // Initialize slotIndex to 1

        // Load existing Dingos data if it exists
        JSONArray jsonDingos;
        string filePath = Path.Combine(Application.persistentDataPath, "dingos.json");
        if (File.Exists(filePath))
        {
            string existingData = File.ReadAllText(filePath);
            jsonDingos = JSON.Parse(existingData) as JSONArray;
            if (jsonDingos == null)
            {
                // If parsing fails, create a new JSONArray
                Debug.LogWarning("Failed to parse existing Dingos data. Creating a new JSONArray.");
                jsonDingos = new JSONArray();
            }
            else
            {
                // Iterate over existing Dingos to find the highest slot index
                foreach (JSONNode dingoData in jsonDingos)
                {
                    JSONObject dingoObj = dingoData.AsObject;
                    if (dingoObj.HasKey("ID"))
                    {
                        int id = dingoObj["ID"].AsInt;
                        slotIndex = Mathf.Max(slotIndex, id); // Update slotIndex if higher ID found
                    }
                }
                // Increment slotIndex by 1 to get the next available slot
                slotIndex++;
            }
        }
        else
        {
            // If file doesn't exist, create a new JSONArray
            Debug.LogWarning("Dingos data file not found. Creating a new JSONArray.");
            jsonDingos = new JSONArray();
        }

        // Debug information
        Debug.Log("Loaded existing Dingos data from file: " + filePath);
        Debug.Log("Number of existing Dingos: " + jsonDingos.Count);

        // Create JSON object for the new Dingo
        JSONObject jsonDingo = new JSONObject();
        DingoID dingo = new DingoID(dingostatID, dingostatName, dingostatType, dingostatDescription, currentHealth, dingostatATK, dingostatDEF, dingostatSPD, dingostatSprite, maxHealth, dingostatXP, dingostatMaxXP, dingostatLevel);

        // Debug information
        Debug.Log("New Dingo created with ID: " + slotIndex);

        // Add properties of the Dingo object to the JSON object
        jsonDingo.Add("ID", slotIndex);
        jsonDingo.Add("DingoID", dingo.ID);
        jsonDingo.Add("Name", dingo.Name);
        jsonDingo.Add("Type", dingo.Type);
        jsonDingo.Add("Description", dingo.Description);
        jsonDingo.Add("CurrentHealth", dingo.HP);
        jsonDingo.Add("ATK", dingo.Attack);
        jsonDingo.Add("DEF", dingo.Defense);
        jsonDingo.Add("SPD", dingo.Speed);
        jsonDingo.Add("Sprite", dingo.Sprite);
        jsonDingo.Add("MaxHealth", dingo.MaxHP);
        jsonDingo.Add("XP", dingo.XP);
        jsonDingo.Add("MaxXP", dingo.MaxXP);
        jsonDingo.Add("Level", dingo.Level);
        jsonDingo.Add("Move1ID", dingo1id);
        jsonDingo.Add("Move2ID", dingo2id);
        jsonDingo.Add("Move3ID", dingo3id);
        jsonDingo.Add("Move4ID", dingo4id);

        // Add the current Dingo object to the JSON array
        jsonDingos.Add(jsonDingo);

        // Debug information
        Debug.Log("Dingo added to JSON array.");

        // Convert the JSON array to a string
        string jsonString = jsonDingos.ToString();

        // Write the JSON data to the file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.Write(jsonString);
        }

        Debug.Log("Dingos data saved to: " + filePath);
        Loader.Load(Loader.Scene.SampleScene);
    }
    public void HealYourself()
    {
        maxplayerHealth = 2000;
        playerHealth = maxplayerHealth;
        healthBar2.SetHealth(playerHealth);
        // Update player's health bar or perform appropriate actions
        playerHP.text = "HP: " + playerHealth + "/" + maxplayerHealth;
    }
    public List<DingoID> loaddingos = new List<DingoID>();
    public void ReloadBattle()
    {
        loaddingos = new List<DingoID>(DingoDatabase.marshmellow);
        Loader.Load(Loader.Scene.Battle, loaddingos);
    }
    private Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        // B(t) = (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2

        float oneMinusT = 1f - t;
        float tSquared = t * t;
        float oneMinusTSquared = oneMinusT * oneMinusT;

        Vector3 result =
            oneMinusTSquared * p0 +
            2f * oneMinusT * t * p1 +
            tSquared * p2;

        return result;
    }
    IEnumerator DecideAnimationCoroutine(string movename)
    {
        dingoImage.sortingLayerName = "Default";
        dingoImage.sortingOrder = 0;
        playerImage.sortingOrder = 2;
        Vector3 originalPosition = playerImage.transform.localPosition;
        if (movename == "Ice Punch")
        {
            // Calculate target position to the right
            Vector3 targetPosition = originalPosition + Vector3.right * moveDistance;

            // Move playerImage smoothly to the right
            float startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed / 2f);
                playerImage.transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, fracJourney);
                yield return null;
            }
            Sprite punchLoad = Resources.Load<Sprite>("BattleMoves/punch_1");
            attackImage2.sprite = punchLoad;
            attackImage2.enabled = true;
            yield return new WaitForSeconds(0.2f);
            punchLoad = Resources.Load<Sprite>("BattleMoves/punch_2");
            attackImage2.sprite = punchLoad;
            yield return new WaitForSeconds(0.2f);
            attackImage2.enabled = false;
            Sprite snowflakeLoad = Resources.Load<Sprite>("BattleMoves/snowflake");
            attackImage1.sprite = snowflakeLoad;
            attackImage1.enabled = true;
            IceEffect.Play();
            yield return new WaitForSeconds(0.4f);
            attackImage1.enabled = false;
            // Move playerImage smoothly back to original position
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                playerImage.transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, fracJourney);
                yield return null;
            }
        }
        else if (movename == "Sugar Slam")
        {
            // Phase 1: Move to the right
            Vector3 targetPosition = originalPosition + Vector3.right * moveDistance;
            float startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                playerImage.transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, fracJourney);
                yield return null;
            }

            // Phase 2: Jump
            Vector3 jumpTargetPosition = targetPosition + Vector3.up * moveDistance;
            startTime = Time.time;
            while (Time.time - startTime < jumpSpeed)
            {
                float jumpFracJourney = (Time.time - startTime) / jumpSpeed;
                playerImage.transform.localPosition = Vector3.Lerp(targetPosition, jumpTargetPosition, jumpFracJourney);
                yield return null;
            }

            // Phase 3: Landing
            Vector3 landPosition = dingoImage.transform.localPosition;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, -90f);
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed / 2f)
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed / 2f);
                playerImage.transform.localPosition = Vector3.Lerp(jumpTargetPosition, landPosition, fracJourney);

                // Smoothly rotate playerImage
                playerImage.transform.localRotation = Quaternion.Slerp(Quaternion.identity, targetRotation, fracJourney);

                yield return null;
            }
            MarshmellowEffect.Play();

            // Phase 4: Return to original position
            startTime = Time.time;
            Quaternion targetRotation2 = Quaternion.Euler(0f, 0f, 0f);
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                playerImage.transform.localPosition = Vector3.Lerp(landPosition, originalPosition, fracJourney);
                playerImage.transform.localRotation = Quaternion.Slerp(targetRotation, targetRotation2, fracJourney);
                yield return null;
            }
        }
        else if (movename == "Aero Slicer")
        {
            Vector3 startPosition = new Vector3(-11, -0.5f, 0);
            float startTime = Time.time;
            Sprite punchLoad = Resources.Load<Sprite>("BattleMoves/aeroslicer1");
            attackImage3.sprite = punchLoad;
            attackImage3.enabled = true;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                attackImage3.transform.localPosition = Vector3.Lerp(startPosition, dingoImage.transform.localPosition, fracJourney);
                yield return null;
            }
            attackImage3.enabled = false;
        }
        else if (movename == "Air Strike")
        {
            Vector3 targetPosition = dingoImage.transform.localPosition;
            float heightOffset = 5f; // Adjust this value as needed
            Vector3 verticalMidpoint = originalPosition + Vector3.up * heightOffset;

            float startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed);
                playerImage.transform.localPosition = Vector3.Lerp(originalPosition, verticalMidpoint, fracJourney);
                yield return null;
            }

            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float normalizedTime = (Time.time - startTime) / moveSpeed;
                // Apply an ease-in function for the horizontal movement
                float horizontalEaseValue = Mathf.Pow(normalizedTime, 3);

                // Interpolate between vertical midpoint and target position
                Vector3 horizontalPosition = Vector3.Lerp(verticalMidpoint, targetPosition, horizontalEaseValue);

                // Set the position of the player's image
                playerImage.transform.localPosition = horizontalPosition;

                yield return null;
            }

            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 22);
                playerImage.transform.localPosition = Vector3.Lerp(playerImage.transform.localPosition, originalPosition, fracJourney);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                playerImage.transform.localPosition = Vector3.Lerp(playerImage.transform.localPosition, originalPosition, fracJourney);
                yield return null;
            }
        }
        else if (movename == "Cloud Burst")
        {
            environmentEffects.Add(DingoDatabase.Rain);
            EvaluateEnvironmentEffects(environmentEffects);
            Vector3 startpoint = new Vector3(-13, -1, 1);
            Vector3 endpoint = new Vector3(13, -1, 1);
            float startTime = Time.time;
            Sprite punchLoad = Resources.Load<Sprite>("BattleMoves/cloud");
            attackImage4.sprite = punchLoad;
            attackImage4.enabled = true;
            while (Time.time - startTime < (moveSpeed * 5))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 5);
                attackImage4.transform.localPosition = Vector3.Lerp(startpoint, endpoint, fracJourney);
                yield return null;
            }
        }
        else if (movename == "Luminous Burst")
        {
            float startTime = Time.time;
            Sprite punchLoad = Resources.Load<Sprite>("BattleMoves/light");
            Sprite punchLoad2 = Resources.Load<Sprite>("BattleMoves/illumination");
            attackImage6.sprite = punchLoad2;
            attackImage6.enabled = true;
            Vector3 finalscale = new Vector3(1, 1, 1);
            Vector3 finalposition = new Vector3(-0.07f, -0.15f, 0);
            Vector3 finalscale2 = new Vector3(2.7f, 1, 1);
            while (Time.time - startTime < (moveSpeed * 3))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 3);
                attackImage6.transform.localScale = Vector3.Lerp(Vector3.zero, finalscale, fracJourney);
                yield return null;
            }
            attackImage5.sprite = punchLoad;
            attackImage5.enabled = true;
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed);
                attackImage5.transform.localScale = Vector3.Lerp(Vector3.zero, finalscale2, fracJourney);
                attackImage5.transform.localPosition = Vector3.Lerp(originalPosition, finalposition, fracJourney);
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
            attackImage5.enabled = false;
            attackImage6.enabled = false;
        }
        else if (movename == "Gooey Glare")
        {
            //unfinished
            opponentEffects.Add(DingoDatabase.Goo);
            EvaluateStatusEffects(opponentEffects);
            float startTime = Time.time;
            Sprite punchLoad = Resources.Load<Sprite>("BattleMoves/light");
            Sprite punchLoad2 = Resources.Load<Sprite>("BattleMoves/illumination");
            Sprite glare = Resources.Load<Sprite>("BattleMoves/gooeyglare");

            attackImage6.sprite = punchLoad2;

            attackImage6.enabled = true;
            Vector3 finalposition = new Vector3(-0.07f, -0.5f, 0);
            Vector3 finalscale2 = new Vector3(2.7f, 0.2f, 1);
            Vector3 finalscale = new Vector3(1, 1, 1);

            while (Time.time - startTime < (moveSpeed * 3))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 3);
                attackImage6.transform.localScale = Vector3.Lerp(Vector3.zero, finalscale, fracJourney);

                yield return null;
            }
            playerImage.sprite = glare;
            attackImage5.sprite = punchLoad;
            originalPosition.y += 0.33f;
            finalposition.y += 0.33f;
            attackImage5.enabled = true;
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed);
                attackImage5.transform.localScale = Vector3.Lerp(Vector3.zero, finalscale2, fracJourney);
                attackImage5.transform.localPosition = Vector3.Lerp(originalPosition, finalposition, fracJourney);
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
            playerImage.sprite = Resources.Load<Sprite>("marshmellow");
            attackImage5.enabled = false;
            attackImage6.enabled = false;
        }
        else if (movename == "Sugar Spin")
        {
            Vector3 Bingo = new Vector3(0, 2.5f, 2.05347633f);
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);
            // Phase 1: Move to the right
            float startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                playerImage.transform.localPosition = BezierCurve(originalPosition, Bingo, dingoImage.transform.localPosition, fracJourney);
                playerImage.transform.Rotate(Vector3.forward, 10);
                yield return null;
            }

            MarshmellowEffect.Play();
            // Phase 3: Landing
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed);
                playerImage.transform.localPosition = Vector3.Lerp(playerImage.transform.localPosition, originalPosition, fracJourney);

                // Smoothly rotate playerImage
                playerImage.transform.localRotation = Quaternion.Slerp(playerImage.transform.localRotation, targetRotation, fracJourney);

                yield return null;
            }

        }
        else if (movename == "Toasty Tumble")
        {
            Vector3 bingoPosition = new Vector3(-12, 5, 2.05347633f);
            Vector3 targetPosition = new Vector3(4.9f, -0.5f, 2.05347633f);
            Vector3 currentVelocity = Vector3.zero;
            float smoothTime = 0.2f; // Adjust this value as needed
            float startTime = Time.time;
            attackImage8.enabled = true;
            while (Time.time - startTime < (1.2f * moveSpeed))
            {

                float fracJourney = (Time.time - startTime) / (1.2f * moveSpeed);
                attackImage8.transform.localPosition = Vector3.Lerp(playerImage.transform.localPosition, targetPosition, fracJourney);
                attackImage8.transform.Rotate(Vector3.forward, -2f);
                yield return null;

            }
            while (Time.time - startTime < (2f * moveSpeed))
            {
                attackImage8.transform.localPosition = Vector3.SmoothDamp(
                    attackImage8.transform.localPosition,
                    bingoPosition,
                    ref currentVelocity,
                    smoothTime
                );
                attackImage8.transform.Rotate(Vector3.forward, 0.3f);
                yield return null;
            }
            attackImage8.enabled = false;
        }
        else if (movename == "Toasted Toss")
        {
            Vector3 bingoPosition = new Vector3(-12, 5, 2.05347633f);
            Vector3 targetPosition = new Vector3(4.9f, -0.5f, 2.05347633f);
            Vector3 currentVelocity = Vector3.zero;
            float smoothTime = 0.2f; // Adjust this value as needed
            float startTime = Time.time;
            attackImage11.enabled = true;
            while (Time.time - startTime < (2f * moveSpeed))
            {

                float fracJourney = (Time.time - startTime) / (2f * moveSpeed);
                attackImage11.transform.localPosition = Vector3.Lerp(playerImage.transform.localPosition, targetPosition, fracJourney);
                attackImage11.transform.Rotate(Vector3.forward, -2f);
                yield return null;

            }
            while (Time.time - startTime < (4f * moveSpeed))
            {
                attackImage11.transform.localPosition = Vector3.SmoothDamp(
                    attackImage11.transform.localPosition,
                    bingoPosition,
                    ref currentVelocity,
                    smoothTime
                );
                attackImage11.transform.Rotate(Vector3.forward, 0.3f);
                yield return null;
            }
            attackImage11.enabled = false;
        }
        else if (movename == "Mallow Munch")
        {
            yield return MallowMunchAnimation();
        }
        else if (movename == "Roasty Beam")
        {
            MarshmellowGameObjectEffect3.SetActive(true);
            MarshmellowEffect3.Play();

            yield return new WaitForSeconds(2.0f);
            MarshmellowGameObjectEffect3.SetActive(false);
        }
        else if (movename == "Marshmallow Melt")
        {
            Vector3 Bingo = new Vector3(-1, 3, 0);
            attackImage12.enabled = true;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, -70f);
            Vector3 halfscale = new Vector3(0.5f, 0.5f, 1);
            Vector3 finalscale = new Vector3(1, 1, 1);
            // Phase 1: Move to the right
            float startTime = Time.time;
            while (Time.time - startTime < (moveSpeed / 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed / 2);
                attackImage12.transform.localScale = Vector3.Lerp(Vector3.zero, halfscale, fracJourney);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                float fracJourney2 = (Time.time - startTime) / (moveSpeed / 2);
                playerImage.transform.localPosition = Vector3.Lerp(originalPosition, dingoImage.transform.localPosition, fracJourney);
                playerImage.transform.localRotation = Quaternion.Slerp(Quaternion.identity, targetRotation, fracJourney);
                attackImage12.transform.localScale = Vector3.Lerp(halfscale, finalscale, fracJourney2);
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            Quaternion targetRotation2 = Quaternion.Euler(0f, 0f, 0f);
            startTime = Time.time;
            while (Time.time - startTime < (2 * moveSpeed))
            {
                float fracJourney = (Time.time - startTime) / (2 * moveSpeed);
                playerImage.transform.localPosition = BezierCurve(dingoImage.transform.localPosition, Bingo, originalPosition, fracJourney);
                playerImage.transform.Rotate(Vector3.forward, 0.7f);
                attackImage12.transform.localScale = Vector3.Lerp(finalscale, Vector3.zero, fracJourney);
                yield return null;
            }
            attackImage12.enabled = false;
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed / 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed / 2);
                playerImage.transform.localRotation = Quaternion.Slerp(playerImage.transform.localRotation, targetRotation2, fracJourney);

                yield return null;
            }
            yield return new WaitForSeconds(0.4f);
        }
        else if (movename == "Squishy Frenzy")
        {
            Vector3 squishyscale = new Vector3(1, 0.5f, 1);
            Vector3 squishyposition = new Vector3(-5.356f, -1.262f, 0);
            Vector3 originalscale = playerImage.transform.localScale;
            Vector3 cornerposition = new Vector3(-7.66f, 3.88f, 0);
            Vector3 cornerposition2 = new Vector3(-8.28f, 4.39f, 0);
            Vector3 Bingo = new Vector3(-1, 2.75f, 0);
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, 230f);
            float startTime = Time.time;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);
                playerImage.transform.localScale = Vector3.Lerp(originalscale, squishyscale, fracJourney);
                playerImage.transform.localPosition = Vector3.Lerp(originalPosition, squishyposition, fracJourney);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                playerImage.transform.localScale = Vector3.Lerp(squishyscale, originalscale, fracJourney);
                playerImage.transform.localPosition = Vector3.Lerp(squishyposition, cornerposition, fracJourney);
                playerImage.transform.localRotation = Quaternion.Slerp(Quaternion.identity, targetRotation, fracJourney);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);
                playerImage.transform.localScale = Vector3.Lerp(originalscale, squishyscale, fracJourney);
                playerImage.transform.localPosition = Vector3.Lerp(cornerposition, cornerposition2, fracJourney);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                playerImage.transform.localScale = Vector3.Lerp(squishyscale, originalscale, fracJourney);
                playerImage.transform.localPosition = Vector3.Lerp(cornerposition2, dingoImage.transform.localPosition, fracJourney);
                yield return null;
            }
            Quaternion targetRotation2 = Quaternion.Euler(0f, 0f, 0f);
            startTime = Time.time;
            while (Time.time - startTime < (2 * moveSpeed))
            {
                float fracJourney = (Time.time - startTime) / (2 * moveSpeed);
                playerImage.transform.localPosition = BezierCurve(dingoImage.transform.localPosition, Bingo, originalPosition, fracJourney);
                playerImage.transform.Rotate(Vector3.forward, 1.4f);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed / 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed / 2);
                playerImage.transform.localRotation = Quaternion.Slerp(playerImage.transform.localRotation, targetRotation2, fracJourney);

                yield return null;
            }
        }
        else if (movename == "Galactic Blast")
        {
            Vector3 finalscale = new Vector3(0.5f, 0.5f, 1);
            Vector3 finalscale2 = new Vector3(1, 1, 1);
            Vector3 finalscale3 = new Vector3(1.5f, 1.5f, 1);
            Vector3 bingo = new Vector3(6, -0.5f, 0);
            Vector3 bingo2 = new Vector3(4, -0.5f, 0);
            attackImage13.enabled = true;
            float startTime = Time.time;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);
                attackImage13.transform.localScale = Vector3.Lerp(Vector3.zero, finalscale, fracJourney);
                attackImage13.transform.Rotate(Vector3.forward, 0.1f);
                attackImage13.transform.localPosition = Vector3.Lerp(playerImage.transform.localPosition, bingo2, fracJourney);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed * 4))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 4);
                attackImage13.transform.localScale = Vector3.Lerp(finalscale, finalscale2, fracJourney);
                attackImage13.transform.Rotate(Vector3.forward, 0.3f);
                attackImage13.transform.localPosition = Vector3.Lerp(bingo2, bingo, fracJourney);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed * 4))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 4);
                attackImage13.transform.localScale = Vector3.Lerp(finalscale2, finalscale3, fracJourney);
                attackImage13.transform.Rotate(Vector3.forward, 0.5f);
                attackImage13.transform.localPosition = Vector3.Lerp(bingo, bingo2, fracJourney);
                yield return null;
            }
            attackImage13.enabled = false;
        }
        else if (movename == "Shooting Star")
        {
            ShootingstarGameObjectEffect.SetActive(true);
            ShootingstarEffect.Play();
            yield return new WaitForSeconds(1.7f);
            ShootingstarGameObjectEffect.SetActive(false);
        }
        else if (movename == "Supernova Surge")
        {
            Vector3 originalscale = new Vector3(1, 1, 1);
            Vector3 finalscale = new Vector3(2, 2, 1);
            Vector3 bingo = new Vector3(-13, -0.5f, 0);
            Vector3 bingo2 = new Vector3(0, -0.5f, 0);
            Sprite supernova = Resources.Load<Sprite>("BattleMoves/supernova");
            Sprite supernova2 = Resources.Load<Sprite>("BattleMoves/star");
            attackImage14.transform.localScale = originalscale;
            float startTime = Time.time;
            attackImage14.enabled = true;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);
                attackImage14.transform.localPosition = BezierCurve(bingo, playerImage.transform.localPosition, bingo2, fracJourney);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                attackImage14.transform.localPosition = Vector3.Lerp(bingo2, dingoImage.transform.localPosition, fracJourney);
                yield return null;
            }
            attackImage14.sprite = supernova;
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);

                attackImage14.transform.localScale = Vector3.Lerp(Vector3.zero, finalscale, fracJourney);
                yield return null;
            }
            attackImage14.sprite = supernova2;
            attackImage14.enabled = false;
        }
        else if (movename == "Stellar Dance")
        {
            float startTime = Time.time;
            Quaternion targetRotation = playerImage.transform.localRotation;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                playerImage.transform.Rotate(Vector3.up, 2f);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);

                playerImage.transform.localRotation = Quaternion.Lerp(playerImage.transform.localRotation, targetRotation, fracJourney);
                yield return null;
            }
        }
        else if (movename == "ATM Withdrawal")
        {
            float startTime = Time.time;
            Vector3 orignalposition = playerImage.transform.localPosition;
            Vector3 moveposition = new Vector3(-3.2f, -1.3f, 0);
            Sprite trustfundbaby = Resources.Load<Sprite>("trustfundbaby");
            Sprite trustfundbaby2 = Resources.Load<Sprite>("trustfundbabycreditcard");
            Sprite atm = Resources.Load<Sprite>("BattleMoves/atm");
            Sprite atm1 = Resources.Load<Sprite>("BattleMoves/atm1");
            Sprite atm2 = Resources.Load<Sprite>("BattleMoves/atm2");
            attackImage15.enabled = true;
            attackImage15.sprite = atm;
            playerImage.sprite = trustfundbaby2;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);
                playerImage.transform.localPosition = Vector3.Lerp(orignalposition, moveposition, fracJourney);
                yield return null;
            }
            playerImage.sortingOrder = 10;
            attackImage15.sprite = atm1;
            playerImage.sprite = trustfundbaby;
            CashEffectGameObject.SetActive(true);
            CashEffect.Play();
            yield return new WaitForSeconds(1f);
            attackImage15.sprite = atm2;
            yield return new WaitForSeconds(1f);
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);

                playerImage.transform.localPosition = Vector3.Lerp(moveposition, orignalposition, fracJourney);
                yield return null;
            }
            attackImage15.enabled = false;

        }
        else if (movename == "Lay Offs")
        {
            Vector3 agentBingoOriginalPosition = new Vector3(-13, 0, 0);
            Vector3 Bingo = new Vector3(-4.4f, 2, 0);
            Vector3 Bingo2 = new Vector3(-3f, 2, 0);
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);
            int randomIndex = Random.Range(0, 3);
            float[] possibleValues = { 0.5f, 1f, 1.5f };
            float randomFloatValue = possibleValues[randomIndex];
            Vector3 newPosition = new Vector3(-1, -1.2f, 0);
            int randomNumber = Random.Range(1, 3);
            int randomNumber2 = Random.Range(1, 4);
            // Phase 1: Move to the right
            float startTime = Time.time;
            while (Time.time - startTime < (randomNumber2 * moveSpeed))
            {
                float fracJourney = (Time.time - startTime) / (randomNumber2 * moveSpeed);
                bingoImage.transform.localPosition = BezierCurve(agentBingoOriginalPosition, Bingo, newPosition, fracJourney);
                bingoImage.transform.Rotate(Vector3.forward, randomFloatValue);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed / 100))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed / 100);

                bingoImage.transform.localRotation = Quaternion.Lerp(bingoImage.transform.localRotation, targetRotation, fracJourney);
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            Status3.text = Status2.text;
            Status2.text = Status.text;
            float randomValue = Random.value;
            int randomPercentage = Mathf.RoundToInt(randomValue * 100);
            //string bingo = "Agent Bingo gained $1000 from cutting costs";
            StartCoroutine(Yapping());
            //yield return StartCoroutine(dialogBox.TypeDialog(bingo));
            startTime = Time.time;
            while (Time.time - startTime < (randomNumber * moveSpeed))
            {
                float fracJourney = (Time.time - startTime) / (randomNumber * moveSpeed);
                bingoImage.transform.localPosition = BezierCurve(newPosition, Bingo2, agentBingoOriginalPosition, fracJourney);
                bingoImage.transform.Rotate(Vector3.forward, -randomFloatValue);
                yield return null;
            }
            yield return new WaitForSeconds(1f);
            canPlayerInput = true;

        }
        dingoImage.sortingLayerName = "Default";
        dingoImage.sortingOrder = 1;
        playerImage.sortingOrder = 1;

    }

    private bool isFrame0 = true; // To track which frame is currently displayed
    private float frameSwitchInterval = 0.2f; // Time between frame switches
    IEnumerator Yapping()
    {
        float startTime = Time.time;
        float lastSwitchTime = startTime;
        Sprite frame0 = Resources.Load<Sprite>("image0");
        Sprite frame1 = Resources.Load<Sprite>("image1");
        bingoImage.sprite = frame0;
        while (Time.time - startTime < 3)
        {
            if (Time.time - lastSwitchTime >= frameSwitchInterval)
            {
                // Switch frame
                isFrame0 = !isFrame0;
                bingoImage.sprite = isFrame0 ? frame0 : frame1;

                // Update the last switch time
                lastSwitchTime = Time.time;
            }
            yield return null;
        }
    }
    IEnumerator DecideOpponentAnimationCoroutine(string movename)
    {
        dingoImage.sortingLayerName = "Default";
        dingoImage.sortingOrder = 2;
        playerImage.sortingOrder = 0;
        Vector3 originalPosition = dingoImage.transform.localPosition;
        if (movename == "Ice Punch")
        {
            // Calculate target position to the right
            Vector3 targetPosition = originalPosition + Vector3.left * moveDistance;

            // Move dingoImage smoothly to the right
            float startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed / 2f);
                dingoImage.transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, fracJourney);
                yield return null;
            }
            Sprite punchLoad = Resources.Load<Sprite>("BattleMoves/punch_1");
            attackImage102.sprite = punchLoad;
            attackImage102.enabled = true;
            yield return new WaitForSeconds(0.2f);
            punchLoad = Resources.Load<Sprite>("BattleMoves/punch_2");
            attackImage102.sprite = punchLoad;
            yield return new WaitForSeconds(0.2f);
            attackImage102.enabled = false;
            Sprite snowflakeLoad = Resources.Load<Sprite>("BattleMoves/snowflake");
            attackImage101.sprite = snowflakeLoad;
            attackImage101.enabled = true;
            IceEffect2.Play();
            yield return new WaitForSeconds(0.4f);
            attackImage101.enabled = false;

            // Move dingoImage smoothly back to original position
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                dingoImage.transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, fracJourney);
                yield return null;
            }
        }
        else if (movename == "Sugar Slam")
        {


            // Phase 1: Move to the left
            Vector3 targetPosition = originalPosition + Vector3.left * moveDistance;
            float startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                dingoImage.transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, fracJourney);
                yield return null;
            }

            // Phase 2: Jump
            Vector3 jumpTargetPosition = targetPosition + Vector3.up * moveDistance;
            startTime = Time.time;
            while (Time.time - startTime < jumpSpeed)
            {
                float jumpFracJourney = (Time.time - startTime) / jumpSpeed;
                dingoImage.transform.localPosition = Vector3.Lerp(targetPosition, jumpTargetPosition, jumpFracJourney);
                yield return null;
            }

            // Phase 3: Landing
            Vector3 landPosition = playerImage.transform.localPosition;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, 90f);
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed / 2f)
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed / 2f);
                dingoImage.transform.localPosition = Vector3.Lerp(jumpTargetPosition, landPosition, fracJourney);

                // Smoothly rotate playerImage
                dingoImage.transform.localRotation = Quaternion.Slerp(Quaternion.identity, targetRotation, fracJourney);

                yield return null;
            }
            MarshmellowEffect2.Play();
            // Phase 4: Return to original position
            startTime = Time.time;
            Quaternion targetRotation2 = Quaternion.Euler(0f, 0f, 0f);
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                dingoImage.transform.localPosition = Vector3.Lerp(landPosition, originalPosition, fracJourney);
                dingoImage.transform.localRotation = Quaternion.Slerp(targetRotation, targetRotation2, fracJourney);
                yield return null;
            }

        }
        else if (movename == "Aero Slicer")
        {
            Vector3 startPosition = new Vector3(11, -0.5f, 0);
            float startTime = Time.time;
            Sprite punchLoad = Resources.Load<Sprite>("BattleMoves/aeroslicer1");
            attackImage3.sprite = punchLoad;
            attackImage3.enabled = true;
            attackImage3.flipX = true;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                attackImage3.transform.localPosition = Vector3.Lerp(startPosition, playerImage.transform.localPosition, fracJourney);
                yield return null;
            }
            attackImage3.enabled = false;
            attackImage3.flipX = false;
        }
        else if (movename == "Air Strike")
        {
            Vector3 targetPosition = playerImage.transform.localPosition;
            float heightOffset = 5f;
            Vector3 verticalMidpoint = originalPosition + Vector3.up * heightOffset;

            float startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 22);
                dingoImage.transform.localPosition = Vector3.Lerp(dingoImage.transform.localPosition, verticalMidpoint, fracJourney);
                yield return null;
            }

            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float normalizedTime = (Time.time - startTime) / moveSpeed;
                // Apply an ease-in function for the horizontal movement
                float horizontalEaseValue = Mathf.Pow(normalizedTime, 3);

                // Interpolate between vertical midpoint and target position
                Vector3 horizontalPosition = Vector3.Lerp(verticalMidpoint, targetPosition, horizontalEaseValue);

                // Set the position of the player's image
                dingoImage.transform.localPosition = horizontalPosition;

                yield return null;
            }

            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 22);
                dingoImage.transform.localPosition = Vector3.Lerp(dingoImage.transform.localPosition, originalPosition, fracJourney);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                dingoImage.transform.localPosition = Vector3.Lerp(dingoImage.transform.localPosition, originalPosition, fracJourney);
                yield return null;
            }
        }
        else if (movename == "Cloud Burst")
        {
            environmentEffects.Add(DingoDatabase.Rain);
            EvaluateEnvironmentEffects(environmentEffects);
            float startTime = Time.time;
            Sprite punchLoad = Resources.Load<Sprite>("BattleMoves/cloud");
            Vector3 startpoint = new Vector3(13, -1, 1);
            Vector3 endpoint = new Vector3(-13, -1, 1);
            attackImage4.sprite = punchLoad;
            attackImage4.enabled = true;
            while (Time.time - startTime < (moveSpeed * 5))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 5);
                attackImage4.transform.localPosition = Vector3.Lerp(startpoint, endpoint, fracJourney);
                yield return null;
            }
        }
        else if (movename == "Luminous Burst")
        {
            float startTime = Time.time;
            Sprite punchLoad = Resources.Load<Sprite>("BattleMoves/light");
            Sprite punchLoad2 = Resources.Load<Sprite>("BattleMoves/illumination");
            attackImage106.sprite = punchLoad2;
            attackImage106.enabled = true;
            Vector3 finalscale = new Vector3(1, 1, 1);
            Vector3 finalposition = new Vector3(0.07f, -0.15f, 0);
            Vector3 finalscale2 = new Vector3(-2.7f, 1, 1);
            while (Time.time - startTime < (moveSpeed * 3))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 3);
                attackImage106.transform.localScale = Vector3.Lerp(Vector3.zero, finalscale, fracJourney);
                yield return null;
            }
            attackImage5.sprite = punchLoad;
            attackImage5.enabled = true;
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed);
                attackImage5.transform.localScale = Vector3.Lerp(Vector3.zero, finalscale2, fracJourney);
                attackImage5.transform.localPosition = Vector3.Lerp(originalPosition, finalposition, fracJourney);
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
            attackImage5.enabled = false;
            attackImage106.enabled = false;
        }
        else if (movename == "Gooey Glare")
        {
            //unfinished
            playerEffects.Add(DingoDatabase.Goo);
            EvaluateStatusEffects(playerEffects);
            float startTime = Time.time;
            Sprite punchLoad = Resources.Load<Sprite>("BattleMoves/light");
            Sprite punchLoad2 = Resources.Load<Sprite>("BattleMoves/illumination");
            Sprite glare = Resources.Load<Sprite>("BattleMoves/gooeyglare");

            attackImage106.sprite = punchLoad2;

            attackImage106.enabled = true;
            Vector3 finalposition = new Vector3(0.07f, -0.5f, 0);
            Vector3 finalscale2 = new Vector3(-2.7f, 0.2f, 1);
            Vector3 finalscale = new Vector3(1, 1, 1);

            while (Time.time - startTime < (moveSpeed * 3))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 3);
                attackImage106.transform.localScale = Vector3.Lerp(Vector3.zero, finalscale, fracJourney);

                yield return null;
            }
            dingoImage.sprite = glare;
            attackImage5.sprite = punchLoad;
            attackImage5.enabled = true;
            originalPosition.y += 0.33f;
            finalposition.y += 0.33f;
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed);
                attackImage5.transform.localScale = Vector3.Lerp(Vector3.zero, finalscale2, fracJourney);
                attackImage5.transform.localPosition = Vector3.Lerp(originalPosition, finalposition, fracJourney);
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
            dingoImage.sprite = Resources.Load<Sprite>("marshmellow");
            attackImage5.enabled = false;
            attackImage106.enabled = false;
        }
        else if (movename == "Sugar Spin")
        {
            Vector3 Bingo = new Vector3(0, 2.5f, 2.05347633f);
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);
            // Phase 1: Move to the right
            float startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                dingoImage.transform.localPosition = BezierCurve(originalPosition, Bingo, playerImage.transform.localPosition, fracJourney);
                dingoImage.transform.Rotate(Vector3.forward, 10);
                yield return null;
            }

            MarshmellowEffect2.Play();
            // Phase 3: Landing
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed);
                dingoImage.transform.localPosition = Vector3.Lerp(dingoImage.transform.localPosition, originalPosition, fracJourney);

                // Smoothly rotate dingoImage
                dingoImage.transform.localRotation = Quaternion.Slerp(dingoImage.transform.localRotation, targetRotation, fracJourney);

                yield return null;
            }

        }
        else if (movename == "Toasty Tumble")
        {
            Vector3 bingoPosition = new Vector3(12, 5, 2.05347633f);
            Vector3 targetPosition = new Vector3(-4.9f, -0.5f, 2.05347633f);
            Vector3 currentVelocity = Vector3.zero;
            float smoothTime = 0.2f; // Adjust this value as needed
            float startTime = Time.time;
            attackImage8.enabled = true;
            while (Time.time - startTime < (1.2f * moveSpeed))
            {

                float fracJourney = (Time.time - startTime) / (1.2f * moveSpeed);
                attackImage8.transform.localPosition = Vector3.Lerp(dingoImage.transform.localPosition, targetPosition, fracJourney);
                attackImage8.transform.Rotate(Vector3.forward, 2f);
                yield return null;

            }
            while (Time.time - startTime < (2f * moveSpeed))
            {
                attackImage8.transform.localPosition = Vector3.SmoothDamp(
                    attackImage8.transform.localPosition,
                    bingoPosition,
                    ref currentVelocity,
                    smoothTime
                );
                attackImage8.transform.Rotate(Vector3.forward, -0.3f);
                yield return null;
            }
            attackImage8.enabled = false;
        }
        else if (movename == "Toasted Toss")
        {
            Vector3 bingoPosition = new Vector3(12, 5, 2.05347633f);
            Vector3 targetPosition = new Vector3(-4.9f, -0.5f, 2.05347633f);
            Vector3 currentVelocity = Vector3.zero;
            float smoothTime = 0.2f; // Adjust this value as needed
            float startTime = Time.time;
            attackImage11.enabled = true;
            while (Time.time - startTime < (2f * moveSpeed))
            {

                float fracJourney = (Time.time - startTime) / (2f * moveSpeed);
                attackImage11.transform.localPosition = Vector3.Lerp(dingoImage.transform.localPosition, targetPosition, fracJourney);
                attackImage11.transform.Rotate(Vector3.forward, 2f);
                yield return null;

            }
            while (Time.time - startTime < (4f * moveSpeed))
            {
                attackImage11.transform.localPosition = Vector3.SmoothDamp(
                    attackImage11.transform.localPosition,
                    bingoPosition,
                    ref currentVelocity,
                    smoothTime
                );
                attackImage11.transform.Rotate(Vector3.forward, -0.3f);
                yield return null;
            }

            attackImage11.enabled = false;
        }
        else if (movename == "Mallow Munch")
        {
            yield return MallowMunchAnimation2();
        }
        else if (movename == "Roasty Beam")
        {
            MarshmellowGameObjectEffect103.SetActive(true);
            MarshmellowEffect103.Play();

            yield return new WaitForSeconds(2.0f);
            MarshmellowGameObjectEffect103.SetActive(false);
        }
        else if (movename == "Marshmallow Melt")
        {
            Vector3 Bingo = new Vector3(1, 3, 0);
            attackImage112.enabled = true;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, 70f);
            Vector3 halfscale = new Vector3(0.5f, 0.5f, 1);
            Vector3 finalscale = new Vector3(1, 1, 1);
            // Phase 1: Move to the right
            float startTime = Time.time;
            while (Time.time - startTime < (moveSpeed / 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed / 2);
                attackImage112.transform.localScale = Vector3.Lerp(Vector3.zero, halfscale, fracJourney);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                float fracJourney2 = (Time.time - startTime) / (moveSpeed / 2);
                dingoImage.transform.localPosition = Vector3.Lerp(originalPosition, playerImage.transform.localPosition, fracJourney);
                dingoImage.transform.localRotation = Quaternion.Slerp(Quaternion.identity, targetRotation, fracJourney);
                attackImage112.transform.localScale = Vector3.Lerp(halfscale, finalscale, fracJourney2);
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            Quaternion targetRotation2 = Quaternion.Euler(0f, 0f, 0f);
            startTime = Time.time;
            while (Time.time - startTime < (2 * moveSpeed))
            {
                float fracJourney = (Time.time - startTime) / (2 * moveSpeed);
                dingoImage.transform.localPosition = BezierCurve(playerImage.transform.localPosition, Bingo, originalPosition, fracJourney);
                dingoImage.transform.Rotate(Vector3.forward, 0.7f);
                attackImage112.transform.localScale = Vector3.Lerp(finalscale, Vector3.zero, fracJourney);
                yield return null;
            }
            attackImage112.enabled = false;
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed / 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed / 2);
                dingoImage.transform.localRotation = Quaternion.Slerp(dingoImage.transform.localRotation, targetRotation2, fracJourney);

                yield return null;
            }
            yield return new WaitForSeconds(0.4f);
        }
        else if (movename == "Squishy Frenzy")
        {
            Vector3 squishyscale = new Vector3(1, 0.5f, 1);
            Vector3 squishyposition = new Vector3(5.356f, -1.262f, 0);
            Vector3 originalscale = playerImage.transform.localScale;
            Vector3 cornerposition = new Vector3(7.66f, 3.88f, 0);
            Vector3 cornerposition2 = new Vector3(8.28f, 4.39f, 0);
            Vector3 Bingo = new Vector3(1, 2.75f, 0);
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, -230f);
            float startTime = Time.time;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);
                dingoImage.transform.localScale = Vector3.Lerp(originalscale, squishyscale, fracJourney);
                dingoImage.transform.localPosition = Vector3.Lerp(originalPosition, squishyposition, fracJourney);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                dingoImage.transform.localScale = Vector3.Lerp(squishyscale, originalscale, fracJourney);
                dingoImage.transform.localPosition = Vector3.Lerp(squishyposition, cornerposition, fracJourney);
                dingoImage.transform.localRotation = Quaternion.Slerp(Quaternion.identity, targetRotation, fracJourney);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);
                dingoImage.transform.localScale = Vector3.Lerp(originalscale, squishyscale, fracJourney);
                dingoImage.transform.localPosition = Vector3.Lerp(cornerposition, cornerposition2, fracJourney);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                dingoImage.transform.localScale = Vector3.Lerp(squishyscale, originalscale, fracJourney);
                dingoImage.transform.localPosition = Vector3.Lerp(cornerposition2, playerImage.transform.localPosition, fracJourney);
                yield return null;
            }
            Quaternion targetRotation2 = Quaternion.Euler(0f, 0f, 0f);
            startTime = Time.time;
            while (Time.time - startTime < (2 * moveSpeed))
            {
                float fracJourney = (Time.time - startTime) / (2 * moveSpeed);
                dingoImage.transform.localPosition = BezierCurve(playerImage.transform.localPosition, Bingo, originalPosition, fracJourney);
                dingoImage.transform.Rotate(Vector3.forward, -1.4f);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed / 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed / 2);
                dingoImage.transform.localRotation = Quaternion.Slerp(dingoImage.transform.localRotation, targetRotation2, fracJourney);

                yield return null;
            }
        }
        else if (movename == "Galactic Blast")
        {
            Vector3 finalscale = new Vector3(0.5f, 0.5f, 1);
            Vector3 finalscale2 = new Vector3(1, 1, 1);
            Vector3 finalscale3 = new Vector3(1.5f, 1.5f, 1);
            Vector3 bingo = new Vector3(-6, -0.5f, 0);
            Vector3 bingo2 = new Vector3(-4, -0.5f, 0);
            attackImage13.enabled = true;
            float startTime = Time.time;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);
                attackImage13.transform.localScale = Vector3.Lerp(Vector3.zero, finalscale, fracJourney);
                attackImage13.transform.Rotate(Vector3.forward, -0.1f);
                attackImage13.transform.localPosition = Vector3.Lerp(dingoImage.transform.localPosition, bingo2, fracJourney);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed * 4))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 4);
                attackImage13.transform.localScale = Vector3.Lerp(finalscale, finalscale2, fracJourney);
                attackImage13.transform.Rotate(Vector3.forward, -0.3f);
                attackImage13.transform.localPosition = Vector3.Lerp(bingo2, bingo, fracJourney);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed * 4))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 4);
                attackImage13.transform.localScale = Vector3.Lerp(finalscale2, finalscale3, fracJourney);
                attackImage13.transform.Rotate(Vector3.forward, -0.5f);
                attackImage13.transform.localPosition = Vector3.Lerp(bingo, bingo2, fracJourney);
                yield return null;
            }
            attackImage13.enabled = false;
        }
        else if (movename == "Shooting Star")
        {
            ShootingstarGameObjectEffect2.SetActive(true);
            ShootingstarEffect2.Play();
            yield return new WaitForSeconds(1.7f);
            ShootingstarGameObjectEffect2.SetActive(false);

        }
        else if (movename == "Supernova Surge")
        {
            Vector3 originalscale = new Vector3(1, 1, 1);
            Vector3 finalscale = new Vector3(2, 2, 1);
            Vector3 bingo = new Vector3(13, -0.5f, 0);
            Vector3 bingo2 = new Vector3(0, -0.5f, 0);
            Sprite supernova = Resources.Load<Sprite>("BattleMoves/supernova");
            Sprite supernova2 = Resources.Load<Sprite>("BattleMoves/star");
            attackImage14.transform.localScale = originalscale;
            float startTime = Time.time;
            attackImage14.enabled = true;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);
                attackImage14.transform.localPosition = BezierCurve(bingo, dingoImage.transform.localPosition, bingo2, fracJourney);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                attackImage14.transform.localPosition = Vector3.Lerp(bingo2, playerImage.transform.localPosition, fracJourney);
                yield return null;
            }
            attackImage14.sprite = supernova;
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);

                attackImage14.transform.localScale = Vector3.Lerp(Vector3.zero, finalscale, fracJourney);
                yield return null;
            }
            attackImage14.sprite = supernova2;
            attackImage14.enabled = false;
        }
        else if (movename == "Stellar Dance")
        {
            float startTime = Time.time;
            Quaternion targetRotation = dingoImage.transform.localRotation;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                dingoImage.transform.Rotate(Vector3.up, -2f);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);

                dingoImage.transform.localRotation = Quaternion.Lerp(dingoImage.transform.localRotation, targetRotation, fracJourney);
                yield return null;
            }
        }
        else if (movename == "ATM Withdrawal")
        {
            float startTime = Time.time;
            Vector3 orignalposition = dingoImage.transform.localPosition;
            Vector3 moveposition = new Vector3(3.2f, -1.3f, 0);
            Sprite trustfundbaby = Resources.Load<Sprite>("trustfundbaby");
            Sprite trustfundbaby2 = Resources.Load<Sprite>("trustfundbabycreditcard");
            Sprite atm = Resources.Load<Sprite>("BattleMoves/atm");
            Sprite atm1 = Resources.Load<Sprite>("BattleMoves/atm1");
            Sprite atm2 = Resources.Load<Sprite>("BattleMoves/atm2");
            attackImage115.enabled = true;
            attackImage115.sprite = atm;
            dingoImage.sprite = trustfundbaby2;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);
                dingoImage.transform.localPosition = Vector3.Lerp(orignalposition, moveposition, fracJourney);
                yield return null;
            }
            dingoImage.sortingOrder = 10;
            attackImage115.sprite = atm1;
            dingoImage.sprite = trustfundbaby;
            CashEffectGameObject2.SetActive(true);
            CashEffect2.Play();
            yield return new WaitForSeconds(1f);
            attackImage115.sprite = atm2;
            yield return new WaitForSeconds(1f);
            startTime = Time.time;
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);

                dingoImage.transform.localPosition = Vector3.Lerp(moveposition, orignalposition, fracJourney);
                yield return null;
            }
            attackImage115.enabled = false;

        }
        playerImage.sortingLayerName = "Default";
        dingoImage.sortingOrder = 1;
        playerImage.sortingOrder = 1;
    }
    IEnumerator CatchAnimation()
    {
        Vector3 originalPosition = new Vector3(-13, 0, 0);
        Vector3 Bingo = new Vector3(-2.4f, 2, 0);
        Vector3 Bingo2 = new Vector3(-3, 2, 0);
        int randomIndex = Random.Range(0, 3);
        float[] possibleValues = { 0.5f, 1f, 1.5f };
        float randomFloatValue = possibleValues[randomIndex];
        Vector3 newPosition = new Vector3(5.35f, -1.2f, 0);
        int randomNumber = Random.Range(1, 3);
        int randomNumber2 = Random.Range(1, 4);
        // Phase 1: Move to the right
        float startTime = Time.time;
        while (Time.time - startTime < (randomNumber2 * moveSpeed))
        {
            float fracJourney = (Time.time - startTime) / (randomNumber2 * moveSpeed);
            bingoImage.transform.localPosition = BezierCurve(originalPosition, Bingo, newPosition, fracJourney);
            bingoImage.transform.Rotate(Vector3.forward, randomFloatValue);
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);

        Status3.text = Status2.text;
        Status2.text = Status.text;
        float randomValue = Random.value;
        int randomPercentage = Mathf.RoundToInt(randomValue * 100);
        string bingo = "Agent Bingo rolled a " + randomPercentage + " you need 70 or higher.";
        //yield return StartCoroutine(dialogBox.TypeDialog(bingo));
        if (randomValue < 0.7f)
        {
            //Miss
            startTime = Time.time;
            while (Time.time - startTime < (randomNumber * moveSpeed))
            {
                float fracJourney = (Time.time - startTime) / (randomNumber * moveSpeed);
                bingoImage.transform.localPosition = BezierCurve(newPosition, Bingo2, originalPosition, fracJourney);
                bingoImage.transform.Rotate(Vector3.forward, -randomFloatValue);
                yield return null;
            }
        }
        else
        {
            CatchText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            SaveDingo();
        }
        yield return new WaitForSeconds(1f);
        canPlayerInput = true;
    }
    IEnumerator MallowMunchAnimation()
    {
        Sprite munch = Resources.Load<Sprite>("BattleMoves/marshmelloweat");
        Sprite munch2 = Resources.Load<Sprite>("marshmellow");
        attackImage9.enabled = true;
        yield return new WaitForSeconds(0.4f);
        string[] munchSpriteNames = { "mellow4", "mellow5", "mellow6", "mellow7", "mellow8" };
        foreach (string spriteName in munchSpriteNames)
        {
            attackImage9.sprite = Resources.Load<Sprite>("BattleMoves/" + spriteName);
            playerImage.sprite = munch;
            yield return new WaitForSeconds(0.2f);
            playerHealth += 20;
            UpdateUI();
            playerImage.sprite = munch2;
            yield return new WaitForSeconds(0.2f);
        }
        attackImage9.enabled = false;
        playerImage.sprite = munch;
        yield return new WaitForSeconds(0.2f);
        playerHealth += 20;
        UpdateUI();
        playerImage.sprite = munch2;
        yield return new WaitForSeconds(0.2f);
        attackImage9.sprite = Resources.Load<Sprite>("BattleMoves/mellow3");
    }
    IEnumerator MallowMunchAnimation2()
    {
        Sprite munch = Resources.Load<Sprite>("BattleMoves/marshmelloweat");
        Sprite munch2 = Resources.Load<Sprite>("marshmellow");
        attackImage109.enabled = true;
        yield return new WaitForSeconds(0.4f);
        string[] munchSpriteNames = { "mellow4", "mellow5", "mellow6", "mellow7", "mellow8" };
        foreach (string spriteName in munchSpriteNames)
        {
            attackImage109.sprite = Resources.Load<Sprite>("BattleMoves/" + spriteName);
            dingoImage.sprite = munch;
            yield return new WaitForSeconds(0.2f);
            currentHealth += 20;
            UpdateUI();
            dingoImage.sprite = munch2;
            yield return new WaitForSeconds(0.2f);
        }
        attackImage109.enabled = false;
        dingoImage.sprite = munch;
        yield return new WaitForSeconds(0.2f);
        playerHealth += 20;
        UpdateUI();
        dingoImage.sprite = munch2;
        yield return new WaitForSeconds(0.2f);
        attackImage109.sprite = Resources.Load<Sprite>("BattleMoves/mellow3");
    }
    IEnumerator DecideHighPriorityAnimationCoroutine(string movename)
    {
        if (movename == "Cosmic Shield")
        {
            float startTime = Time.time;
            Sprite punchLoad2 = Resources.Load<Sprite>("BattleMoves/cosmicshield");
            attackImage7.sprite = punchLoad2;
            attackImage7.enabled = true;
            Vector3 finalscale = new Vector3(1, 1, 1);
            while (Time.time - startTime < (moveSpeed * 3))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 3);
                attackImage7.transform.localScale = Vector3.Lerp(Vector3.zero, finalscale, fracJourney);
                yield return null;
            }
        }
        if (movename == "Sweet Shield")
        {
            float startTime = Time.time;
            attackImage10.enabled = true;
            Vector3 finalscale = new Vector3(1.5f, 1.5f, 1);
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);
                attackImage10.transform.localScale = Vector3.Lerp(Vector3.zero, finalscale, fracJourney);
                yield return null;
            }
            StartCoroutine(ContinuousRotation());
        }
        yield return new WaitForSeconds(1f);
    }
    IEnumerator ContinuousRotation()
    {
        while (true)  // Loop continuously
        {
            attackImage10.transform.Rotate(Vector3.forward, -0.4f);
            yield return null;  // Wait for the next frame
        }
    }
    IEnumerator ContinuousRotation2()
    {
        while (true)  // Loop continuously
        {
            attackImage110.transform.Rotate(Vector3.forward, -0.4f);
            yield return null;  // Wait for the next frame
        }
    }
    IEnumerator StopEffects()
    {
        float startTime = Time.time;
        StopCoroutine(ContinuousRotation());
        StopCoroutine(ContinuousRotation2());
        while (Time.time - startTime < (moveSpeed * 5))
        {
            float fracJourney = (Time.time - startTime) / (moveSpeed * 5);
            attackImage7.transform.localScale = Vector3.Lerp(attackImage7.transform.localScale, Vector3.zero, fracJourney);
            attackImage10.transform.localScale = Vector3.Lerp(attackImage10.transform.localScale, Vector3.zero, fracJourney);
            attackImage107.transform.localScale = Vector3.Lerp(attackImage107.transform.localScale, Vector3.zero, fracJourney);
            yield return null;
        }
        attackImage7.enabled = false;
        attackImage107.enabled = false;
        attackImage10.enabled = false;
        yield return null;
    }
    IEnumerator DecideHighPriorityOpponentAnimationCoroutine(string movename)
    {
        if (movename == "Cosmic Shield")
        {
            float startTime = Time.time;
            Sprite punchLoad2 = Resources.Load<Sprite>("BattleMoves/cosmicshield");
            attackImage107.sprite = punchLoad2;
            attackImage107.enabled = true;
            Vector3 finalscale = new Vector3(1, 1, 1);
            while (Time.time - startTime < (moveSpeed * 5))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 5);
                attackImage107.transform.localScale = Vector3.Lerp(Vector3.zero, finalscale, fracJourney);
                yield return null;
            }
        }
        if (movename == "Sweet Shield")
        {
            float startTime = Time.time;
            attackImage110.enabled = true;
            Vector3 finalscale = new Vector3(1.5f, 1.5f, 1);
            while (Time.time - startTime < (moveSpeed * 2))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 2);
                attackImage110.transform.localScale = Vector3.Lerp(Vector3.zero, finalscale, fracJourney);
                yield return null;
            }
            StartCoroutine(ContinuousRotation2());
        }
        yield return new WaitForSeconds(1f);
    }

    public void AnimationsDisplay()
    {
        string inputText = inputField.text;
        StartCoroutine(playbothanimations(inputText));
    }
    IEnumerator playbothanimations(string movename)
    {
        yield return DecideAnimationCoroutine(movename);
        yield return DecideOpponentAnimationCoroutine(movename);
        yield return DecideHighPriorityAnimationCoroutine(movename);
        yield return DecideHighPriorityOpponentAnimationCoroutine(movename);
        yield return null;
    }
    public void PlayAllAnimations()
    {
        StartCoroutine(AllAnimations());
    }
    IEnumerator AllAnimations()
    {
        yield return DecideAnimationCoroutine("Ice Punch");
        yield return DecideAnimationCoroutine("Sugar Slam");
        yield return DecideAnimationCoroutine("Aero Slicer");
        yield return DecideAnimationCoroutine("Aero Slicer");
        yield return DecideAnimationCoroutine("Air Strike");
        yield return DecideAnimationCoroutine("Cloud Burst");
        yield return DecideAnimationCoroutine("Luminous Burst");
        yield return DecideAnimationCoroutine("Gooey Glare");
        yield return DecideAnimationCoroutine("Sugar Spin");
        yield return DecideAnimationCoroutine("Toasty Tumble");
        yield return DecideAnimationCoroutine("Toasted Toss");
        yield return DecideAnimationCoroutine("Mallow Munch");
        yield return DecideAnimationCoroutine("Roasty Beam");
        yield return DecideAnimationCoroutine("Marshmallow Melt");
        yield return DecideAnimationCoroutine("Squishy Frenzy");
        yield return DecideAnimationCoroutine("Galactic Blast");
        yield return DecideAnimationCoroutine("Shooting Star");
        yield return DecideAnimationCoroutine("Supernova Surge");
        yield return DecideAnimationCoroutine("Stellar Dance");
        yield return DecideAnimationCoroutine("ATM Withdrawal");
        yield return DecideAnimationCoroutine("Lay Offs");
        CashEffectGameObject.SetActive(false);
        yield return DecideHighPriorityAnimationCoroutine("Cosmic Shield");
        yield return StopEffects();
        yield return DecideHighPriorityAnimationCoroutine("Sweet Shield");
        yield return StopEffects();
    }
}
