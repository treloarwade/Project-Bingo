using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PokemonMoves; // Assuming you've defined PokemonMoves namespace

public class MoveNameDisplay : MonoBehaviour
{
    public Text moveNameText; // Reference to the Text component

    void Start()
    {
        // Accessing a move from the MoveDatabase
        PokemonMove move = MoveDatabase.Moves[1]; // Accessing the first move (index 0)

        // Using the Name of the move
        string moveName = move.Name;

        // Assigning moveName to the Text component's text property
        moveNameText.text = moveName;
    }
}
