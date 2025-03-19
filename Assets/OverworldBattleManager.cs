using UnityEngine;
using System.Collections.Generic;
using DingoSystem;
using Unity.Netcode;
using System.Collections;
using UnityEngine.UI;
using System.ComponentModel;

public class OverworldBattleManager : NetworkBehaviour
{
    [Header("Player Positions")]
    public GameObject Slot1, Slot2, TrainerPosition1, TrainerPosition2;
    [Header("Opponent Positions")]
    public GameObject Opponent1, Opponent2, OpponentTrainerPosition1, OpponentTrainerPosition2;
    private Dictionary<int, GameObject> entityInPosition = new Dictionary<int, GameObject>();
    public Dictionary<int, bool> positionOccupied = new Dictionary<int, bool>();
    private Camera mainCamera;
    private HashSet<ulong> activePlayers = new HashSet<ulong>();
    public NetworkVariable<bool> player1HasChosenMove = new NetworkVariable<bool>();
    public NetworkVariable<bool> player2HasChosenMove = new NetworkVariable<bool>();
    private Button targetButton1, targetButton2, bothTargetsButton;
    public bool IsReady = false;
}
