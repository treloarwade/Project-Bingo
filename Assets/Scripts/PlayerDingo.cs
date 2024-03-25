using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using DingoIDs;
using DingoMoves;

public class DifferentPlayerDingoExample : MonoBehaviour
{
    void Start()
    {
        // Create a list of moves for the different PlayerDingo
        List<DingoMove> differentPlayerMoves = new List<DingoMove>
        {
            MoveDatabase.Moves[2], // Ice Beam
            MoveDatabase.Moves[3]  // Earthquake
        };
    }
}

