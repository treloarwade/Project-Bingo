using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DingoSystem;
using System.IO;
using SimpleJSON;
using System.Runtime.CompilerServices;
using System.Net.NetworkInformation;
using System.Linq.Expressions;
using TMPro;


public class BattleManager : MonoBehaviour
{
    private static List<DingoID> randomDingos;
    public int minRandomStat = -5; // Minimum value for randomizing stats
    public int maxRandomStat = 5;  // Maximum value for randomizing stats
    public int dingoID;
    public GameObject DingoMenu;
    public Text Status;
    public Text Status2;
    public Text Status3;
    public Text Status4;
    public Text Status5;
    public Text dingoType;
    public UnityEngine.UI.Image dingoImage;
    public Text dingoName;
    public Text dingoATK;
    public Text dingoDEF;
    public Text dingoSPD;
    public Text dingoHP;
    public int playerID;
    public Text playerType;
    public UnityEngine.UI.Image playerImage;
    public UnityEngine.UI.Image attackImage1;
    public UnityEngine.UI.Image attackImage2;
    public UnityEngine.UI.Image attackImage3;
    public UnityEngine.UI.Image attackImage4;
    public UnityEngine.UI.Image attackImage101;
    public UnityEngine.UI.Image attackImage102;
    public UnityEngine.UI.Image attackImage103;
    public UnityEngine.UI.Image attackImage104;
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
    private int slotIndex2;
    // Constructor
    public BattleManager()
    {
        // Initialize playerEffects list
        playerEffects = new List<StatusEffect>();
        opponentEffects = new List<StatusEffect>();
        environmentEffects = new List<EnvironmentEffect>();
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
    public static DingoID GetRandomDingos()
    {
        // Check if the list of dingos is not null and not empty
        if (randomDingos != null && randomDingos.Count > 0)
        {
            // Generate a random index within the range of the list
            int randomIndex = Random.Range(0, randomDingos.Count);
            // Return the dingo at the random index
            return randomDingos[randomIndex];
        }
        else
        {
            // If the list is null or empty, return null
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
        DingoID opponentDingo = GetRandomDingos();

        opponentDingo.Attack += UnityEngine.Random.Range(minRandomStat, maxRandomStat);
        opponentDingo.Defense += UnityEngine.Random.Range(minRandomStat, maxRandomStat);
        opponentDingo.Speed += UnityEngine.Random.Range(minRandomStat, maxRandomStat);
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
        Use2nd(dingoIndex);
    }
    public void Move1()
    {
        // Check if player input is allowed
        if (!canPlayerInput)
        {
            return; // Exit the method if input is blocked
        }
        UseMove(1, move1power, move1accuracy, move1name);
    }
    public void Move2()
    {
        // Check if player input is allowed
        if (!canPlayerInput)
        {
            return; // Exit the method if input is blocked
        }
        UseMove(2, move2power, move2accuracy, move2name);
    }
    public void Move3()
    {
        // Check if player input is allowed
        if (!canPlayerInput)
        {
            return; // Exit the method if input is blocked
        }
        UseMove(3, move3power, move3accuracy, move3name);
    }
    public void Move4()
    {
        // Check if player input is allowed
        if (!canPlayerInput)
        {
            return; // Exit the method if input is blocked
        }
        UseMove(4, move4power, move4accuracy, move4name);
    }
    public void UseMove(int moveNumber, int movePower, float moveAccuracy, string moveName)
    {
        canPlayerInput = false;
        StartCoroutine(PerformMoveRoutine(moveNumber, movePower, moveAccuracy, moveName));
    }
    private IEnumerator PerformMoveRoutine(int moveNumber, int movePower, float moveAccuracy, string moveName)
    {

        int opponentMoveNumber = DetermineOpponentMove();
        DingoID dingo = DingoDatabase.GetDingoByID(dingostatID);
        DingoMove opponentMove = DingoDatabase.GetMoveByID(opponentMoveNumber, dingo);
        DingoID playerdingo = DingoDatabase.GetDingoByID(playerstatID);
        moveNumber--;
        DingoMove playerMove = DingoDatabase.GetMoveByID(moveNumber, playerdingo);

        if (playerpreviousmoveID == moveNumber)
        {
            Debug.Log("Player tried to spam: " + moveName);
        }
        else
        {
            switch (moveName)
            {
                case "Sneaky Shell":
                case "Cosmic Shield":
                    playerstatDEF += 1000;
                    break;
            }
        }
        if (opponentpreviousmoveID == opponentMoveNumber)
        {
            Debug.Log("Opponent tried to spam: " + opponentMove.Name);
        }
        else
        {
            switch (opponentMove.Name)
            {
                case "Sneaky Shell":
                case "Cosmic Shield":
                    dingostatDEF += 1000;
                    break;
            }
        }

        bool playerGoesFirst = playerstatSPD > dingostatSPD;
        if (playerGoesFirst)
        {
            yield return StartCoroutine(PlayerAttackFirst(moveNumber, movePower, moveAccuracy, moveName, opponentMoveNumber, opponentMove.Type, playerMove.Type, dingo.Type));
        }
        else
        {
            yield return StartCoroutine(PlayerAttackSecond(moveNumber, movePower, moveAccuracy, moveName, opponentMoveNumber, opponentMove.Type, playerMove.Type, dingo.Type));
        }

        if (playerpreviousmoveID == moveNumber)
        {
        }
        else
        {
            switch (moveName)
            {
                case "Sneaky Shell":
                case "Cosmic Shield":
                    playerstatDEF -= 1000;
                    break;
            }
        }
        if (opponentpreviousmoveID == opponentMoveNumber)
        {
        }
        else
        {
            switch (opponentMove.Name)
            {
                case "Sneaky Shell":
                case "Cosmic Shield":
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
    private bool writeToStatus4 = true;
    public void PerformMove(int moveNumber, int movePower, float moveAccuracy, string moveName, int attackerattack, int defenderdefense, string moveType, string defendertype, List<StatusEffect> attackerEffects, List<StatusEffect> defenderEffects, List<EnvironmentEffect> environmentEffects)
    {
        attackerEffects = new List<StatusEffect>();
        defenderEffects = new List<StatusEffect>();
        environmentEffects = new List<EnvironmentEffect>();
        float accuracy = moveAccuracy / 100f; // Convert accuracy from percentage to decimal

        bool moveHits = Random.Range(0f, 1f) <= accuracy;
        if (movePower > 0)
        {
            if (moveHits)
            {
                // Apply attacker's attack and defender's defense along with any bonuses
                int attackerAttackWithBonus = attackerattack + CalculateAttackBonus(attackerEffects);
                int defenderDefenseWithBonus = defenderdefense + CalculateDefenseBonus(defenderEffects);

                // Calculate damage
                float modifier = (((attackerAttackWithBonus - defenderDefenseWithBonus) / 100f) + 1);
                float effectiveness = CalculateTypeModifier(moveType, defendertype);
                int damageAmount = Mathf.Max(Mathf.RoundToInt((modifier * movePower) * effectiveness), 1);
                Debug.Log(moveName + " is type " + moveType + ". It did x" + effectiveness + " against " + defendertype);
                if (writeToStatus4)
                {
                    Status4.text = moveName + " is type " + moveType + ". It did x" + effectiveness + " against " + defendertype;
                }
                else
                {
                    Status5.text = moveName + " is type " + moveType + ". It did x" + effectiveness + " against " + defendertype;
                }
                                // Toggle the flag for the next call
                writeToStatus4 = !writeToStatus4;
                DefenderHP -= damageAmount; // Apply damage

                // Update status text to indicate move did damage
                Status.text = moveName + " did " + damageAmount + " damage.";
                Status.gameObject.SetActive(true); // Set status text to visible
            }
            else
            {
                // Update status text to indicate move missed
                Status.text = moveName + " missed!";
                Status.gameObject.SetActive(true); // Set status text to visible
            }
        }
        else
        {
            Status.text = moveName + " isn't supposed to do damage.";
            Status.gameObject.SetActive(true);
        }
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
                PerformMove(moveNumber, movePower, moveAccuracy, moveName, playerstatATK, dingostatDEF, playermoveType, opponentType, playerEffects, opponentEffects, environmentEffects);
                Status3.text = Status.text;
                Status3.gameObject.SetActive(true);
                currentHealth = DefenderHP;
                UpdateUI();
                yield return new WaitForSeconds(0.4f);
            }
        }
        yield return StartCoroutine(DecideAnimationCoroutine(moveName));
        PerformMove(moveNumber, movePower, moveAccuracy, moveName, playerstatATK, dingostatDEF, playermoveType, opponentType, playerEffects, opponentEffects, environmentEffects);
        Status3.text = Status.text;
        Status3.gameObject.SetActive(true);
        currentHealth = DefenderHP;
        UpdateUI();
        if (currentHealth <= 0)
        {
            SavePlayerDingo();
            Loader.Load(Loader.Scene.SampleScene); // Example scene reload
        }
        yield return new WaitForSeconds(1f);
        //yield return StartCoroutine(DecideOpponentAnimationCoroutine(moveName));
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
                PerformMove(moveNumber, movePower, moveAccuracy, moveName, playerstatATK, dingostatDEF, playermoveType, opponentType, playerEffects, opponentEffects, environmentEffects);
                Status3.text = Status.text;
                Status3.gameObject.SetActive(true);
                currentHealth = DefenderHP;
                UpdateUI();
                yield return new WaitForSeconds(0.4f);
            }
        }
        yield return StartCoroutine(DecideAnimationCoroutine(moveName));
        PerformMove(moveNumber, movePower, moveAccuracy, moveName, playerstatATK, dingostatDEF, playermoveType, opponentType, playerEffects, opponentEffects, environmentEffects);
        Status3.text = Status.text;
        Status3.gameObject.SetActive(true);
        currentHealth = DefenderHP;
        UpdateUI();
        if (currentHealth <= 0)
        {
            SavePlayerDingo();
            Loader.Load(Loader.Scene.SampleScene); // Example scene reload
        }
        canPlayerInput = true;
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
                        PerformMove(1, dingo1power, dingo1accuracy, dingo1name, dingostatATK, playerstatDEF, movetype, playerstatType, opponentEffects, playerEffects, environmentEffects);
                        playerHealth = DefenderHP;
                        Status2.text = Status.text; Status2.gameObject.SetActive(true);
                        break;
                    case 1:
                        DefenderHP = playerHealth;
                        PerformMove(2, dingo2power, dingo2accuracy, dingo2name, dingostatATK, playerstatDEF, movetype, playerstatType, opponentEffects, playerEffects, environmentEffects);
                        playerHealth = DefenderHP;
                        Status2.text = Status.text; Status2.gameObject.SetActive(true);
                        break;
                    case 2:
                        DefenderHP = playerHealth;
                        PerformMove(3, dingo3power, dingo3accuracy, dingo3name, dingostatATK, playerstatDEF, movetype, playerstatType, opponentEffects, playerEffects, environmentEffects);
                        playerHealth = DefenderHP;
                        Status2.text = Status.text; Status2.gameObject.SetActive(true);
                        break;
                    case 3:
                        DefenderHP = playerHealth;
                        PerformMove(4, dingo4power, dingo4accuracy, dingo4name, dingostatATK, playerstatDEF, movetype, playerstatType, opponentEffects, playerEffects, environmentEffects);
                        playerHealth = DefenderHP;
                        Status2.text = Status.text; Status2.gameObject.SetActive(true);
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
                PerformMove(1, dingo1power, dingo1accuracy, dingo1name, dingostatATK, playerstatDEF, movetype, playerstatType, opponentEffects, playerEffects, environmentEffects);
                playerHealth = DefenderHP;
                Status2.text = Status.text; Status2.gameObject.SetActive(true);
                break;
            case 1:
                DefenderHP = playerHealth;
                PerformMove(2, dingo2power, dingo2accuracy, dingo2name, dingostatATK, playerstatDEF, movetype, playerstatType, opponentEffects, playerEffects, environmentEffects);
                playerHealth = DefenderHP;
                Status2.text = Status.text; Status2.gameObject.SetActive(true);
                break;
            case 2:
                DefenderHP = playerHealth;
                PerformMove(3, dingo3power, dingo3accuracy, dingo3name, dingostatATK, playerstatDEF, movetype, playerstatType, opponentEffects, playerEffects, environmentEffects);
                playerHealth = DefenderHP;
                Status2.text = Status.text; Status2.gameObject.SetActive(true);
                break;
            case 3:
                DefenderHP = playerHealth;
                PerformMove(4, dingo4power, dingo4accuracy, dingo4name, dingostatATK, playerstatDEF, movetype, playerstatType, opponentEffects, playerEffects, environmentEffects);
                playerHealth = DefenderHP;
                Status2.text = Status.text; Status2.gameObject.SetActive(true);
                break;
            default:
                Debug.LogError("Invalid move selected!");
                break;
        }
        isAttackBackRunning = false;
    }
    private int moveDistance = 800;
    public float moveSpeed = 0.5f;
    public float jumpSpeed = 0.2f;
    public float animationTime = 0.2f;

    IEnumerator DecideAnimationCoroutine(string movename)
    {
        dingoImage.transform.SetAsFirstSibling();
        if (movename == "Ice Punch")
        {
            // Store original position
            Vector3 originalPosition = playerImage.rectTransform.localPosition;
            // Calculate target position to the right
            Vector3 targetPosition = originalPosition + Vector3.right * moveDistance;

            // Move playerImage smoothly to the right
            float startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed / 2f);
                playerImage.rectTransform.localPosition = Vector3.Lerp(originalPosition, targetPosition, fracJourney);
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
            yield return new WaitForSeconds(0.4f);
            attackImage1.enabled = false;
            // Move playerImage smoothly back to original position
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                playerImage.rectTransform.localPosition = Vector3.Lerp(targetPosition, originalPosition, fracJourney);
                yield return null;
            }
        }
        else if (movename == "Sugar Slam")
        {

            // Store original position
            Vector3 originalPosition = playerImage.rectTransform.localPosition;

            // Phase 1: Move to the right
            Vector3 targetPosition = originalPosition + Vector3.right * moveDistance;
            float startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                playerImage.rectTransform.localPosition = Vector3.Lerp(originalPosition, targetPosition, fracJourney);
                yield return null;
            }

            // Phase 2: Jump
            Vector3 jumpTargetPosition = targetPosition + Vector3.up * moveDistance;
            startTime = Time.time;
            while (Time.time - startTime < jumpSpeed)
            {
                float jumpFracJourney = (Time.time - startTime) / jumpSpeed;
                playerImage.rectTransform.localPosition = Vector3.Lerp(targetPosition, jumpTargetPosition, jumpFracJourney);
                yield return null;
            }

            // Phase 3: Landing
            Vector3 landPosition = dingoImage.rectTransform.localPosition;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, -90f);
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed / 2f)
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed / 2f);
                playerImage.rectTransform.localPosition = Vector3.Lerp(jumpTargetPosition, landPosition, fracJourney);

                // Smoothly rotate playerImage
                playerImage.rectTransform.localRotation = Quaternion.Slerp(Quaternion.identity, targetRotation, fracJourney);

                yield return null;
            }

            // Phase 4: Return to original position
            startTime = Time.time;
            Quaternion targetRotation2 = Quaternion.Euler(0f, 0f, 0f);
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                playerImage.rectTransform.localPosition = Vector3.Lerp(landPosition, originalPosition, fracJourney);
                playerImage.rectTransform.localRotation = Quaternion.Slerp(targetRotation, targetRotation2, fracJourney);
                yield return null;
            }
        }
        else if (movename == "Aero Slicer")
        {
            Vector3 originalPosition = playerImage.rectTransform.localPosition;
            float startTime = Time.time;
            Sprite punchLoad = Resources.Load<Sprite>("BattleMoves/aeroslicer1");
            attackImage3.sprite = punchLoad;
            attackImage3.enabled = true;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                attackImage3.rectTransform.localPosition = Vector3.Lerp(originalPosition, dingoImage.rectTransform.localPosition, fracJourney);
                yield return null;
            }
        }
        else if (movename == "Air Strike")
        {
            Vector3 originalPosition = playerImage.rectTransform.localPosition;
            Vector3 targetPosition = dingoImage.rectTransform.localPosition;

            // Define the height the player's image should rise before moving horizontally
            float heightOffset = 500f; // Adjust this value as needed

            // Calculate the midpoint for the vertical movement
            Vector3 verticalMidpoint = originalPosition + Vector3.up * heightOffset;

            float startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 22);
                playerImage.rectTransform.localPosition = Vector3.Lerp(playerImage.rectTransform.localPosition, verticalMidpoint, fracJourney);
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
                playerImage.rectTransform.localPosition = horizontalPosition;

                yield return null;
            }

            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 22);
                playerImage.rectTransform.localPosition = Vector3.Lerp(playerImage.rectTransform.localPosition, originalPosition, fracJourney);
                yield return null;
            }
        }
        else if (movename == "Cloud Burst")
        {
            environmentEffects.Add(DingoDatabase.Rain);
            EvaluateEnvironmentEffects(environmentEffects);
            Vector3 originalPosition = playerImage.rectTransform.localPosition;
            float startTime = Time.time;
            Sprite punchLoad = Resources.Load<Sprite>("BattleMoves/cloud");
            attackImage4.sprite = punchLoad;
            attackImage4.enabled = true;
            while (Time.time - startTime < (moveSpeed * 5))
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed * 5);
                attackImage4.rectTransform.localPosition = Vector3.Lerp(originalPosition, dingoImage.rectTransform.localPosition, fracJourney);
                yield return null;
            }
        }
        yield return new WaitForSeconds(1f); // Adjust the time as needed
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

    IEnumerator DecideOpponentAnimationCoroutine(string movename)
    {
        playerImage.transform.SetAsFirstSibling();
        if (movename == "Ice Punch")
        {
            // Store original position
            Vector3 originalPosition = dingoImage.rectTransform.localPosition;
            // Calculate target position to the right
            Vector3 targetPosition = originalPosition + Vector3.left * moveDistance;

            // Move dingoImage smoothly to the right
            float startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed / 2f);
                dingoImage.rectTransform.localPosition = Vector3.Lerp(originalPosition, targetPosition, fracJourney);
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
            yield return new WaitForSeconds(0.4f);
            attackImage101.enabled = false;
            // Move dingoImage smoothly back to original position
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                dingoImage.rectTransform.localPosition = Vector3.Lerp(targetPosition, originalPosition, fracJourney);
                yield return null;
            }
        }
        else if (movename == "Sugar Slam")
        {

            // Store original position
            Vector3 originalPosition = dingoImage.rectTransform.localPosition;

            // Phase 1: Move to the left
            Vector3 targetPosition = originalPosition + Vector3.left * moveDistance;
            float startTime = Time.time;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                dingoImage.rectTransform.localPosition = Vector3.Lerp(originalPosition, targetPosition, fracJourney);
                yield return null;
            }

            // Phase 2: Jump
            Vector3 jumpTargetPosition = targetPosition + Vector3.up * moveDistance;
            startTime = Time.time;
            while (Time.time - startTime < jumpSpeed)
            {
                float jumpFracJourney = (Time.time - startTime) / jumpSpeed;
                dingoImage.rectTransform.localPosition = Vector3.Lerp(targetPosition, jumpTargetPosition, jumpFracJourney);
                yield return null;
            }

            // Phase 3: Landing
            Vector3 landPosition = playerImage.rectTransform.localPosition;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, 90f);
            startTime = Time.time;
            while (Time.time - startTime < moveSpeed / 2f)
            {
                float fracJourney = (Time.time - startTime) / (moveSpeed / 2f);
                dingoImage.rectTransform.localPosition = Vector3.Lerp(jumpTargetPosition, landPosition, fracJourney);

                // Smoothly rotate playerImage
                dingoImage.rectTransform.localRotation = Quaternion.Slerp(Quaternion.identity, targetRotation, fracJourney);

                yield return null;
            }

            // Phase 4: Return to original position
            startTime = Time.time;
            Quaternion targetRotation2 = Quaternion.Euler(0f, 0f, 0f);
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                dingoImage.rectTransform.localPosition = Vector3.Lerp(landPosition, originalPosition, fracJourney);
                dingoImage.rectTransform.localRotation = Quaternion.Slerp(targetRotation, targetRotation2, fracJourney);
                yield return null;
            }

        }
        else if (movename == "Aero Slicer")
        {
            Vector3 originalPosition = dingoImage.rectTransform.localPosition;
            float startTime = Time.time;
            Sprite punchLoad = Resources.Load<Sprite>("BattleMoves/aeroslicer1");
            attackImage103.sprite = punchLoad;
            attackImage103.enabled = true;
            while (Time.time - startTime < moveSpeed)
            {
                float fracJourney = (Time.time - startTime) / moveSpeed;
                attackImage103.rectTransform.localPosition = Vector3.Lerp(originalPosition, playerImage.rectTransform.localPosition, fracJourney);
                yield return null;
            }
        }
        yield return new WaitForSeconds(1f); // Adjust the time as needed
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
        if (!canPlayerInput)
        {
            return; // Exit the method
        }
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
}