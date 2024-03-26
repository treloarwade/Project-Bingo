using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using DingoSystem;
using DingoMoves; // Assuming you have a namespace for Dingo moves

public class GameManager : MonoBehaviour
{
    void Start()
    {
        // Create a player inventory
        PlayerInventory inventory = new PlayerInventory();

        // Instantiate a level 1 BingoStar with the nickname "Shiny" and default moves
        PlayerDingo shinyBingoStar = new PlayerDingo(
            name: DingoDatabase.BingoStar.Name,
            type: DingoDatabase.BingoStar.Type,
            description: DingoDatabase.BingoStar.Description,
            hp: DingoDatabase.BingoStar.HP,
            attack: DingoDatabase.BingoStar.Attack,
            defense: DingoDatabase.BingoStar.Defense,
            speed: DingoDatabase.BingoStar.Speed,
            sprite: DingoDatabase.BingoStar.Sprite,
            nickname: "Shiny", // Nickname "Shiny"
            level: 1, // Level 1
            moves: new List<DingoMove>() // Initialize moves here if needed
        );

        // Add the shiny BingoStar to the player's inventory
        inventory.AddDingo(shinyBingoStar);
    }
}

