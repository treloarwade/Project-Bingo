using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Define a class to represent a move
public class Move
{
    public string name;
    public int damage;

    // Constructor
    public Move(string _name, int _damage)
    {
        name = _name;
        damage = _damage;
    }
}

public class MoveList : MonoBehaviour
{
    // List to store moves
    private List<Move> moves;

    void Start()
    {
        // Initialize the list
        moves = new List<Move>();

        // Add moves to the list
        moves.Add(new Move("Tackle", 10));
        moves.Add(new Move("Fire Blast", 50));
        moves.Add(new Move("Thunderbolt", 40));
        moves.Add(new Move("Bingo", 10));
        moves.Add(new Move("Shing", 50));
        moves.Add(new Move("Bingo", 10));
        moves.Add(new Move("Shing", 50));
        moves.Add(new Move("Bingo", 10));
        moves.Add(new Move("Shing", 50));
        moves.Add(new Move("Bingo", 10));
        moves.Add(new Move("Shing", 50));
        moves.Add(new Move("Bingo", 10));
        moves.Add(new Move("Shing", 50));

        // Accessing moves from the list
        Debug.Log("First move: " + moves[0].name + ", Damage: " + moves[0].damage);
        Debug.Log("Second move: " + moves[1].name + ", Damage: " + moves[1].damage);

        // Iterate over moves in the list
        foreach (Move move in moves)
        {
            Debug.Log("Move: " + move.name + ", Damage: " + move.damage);
        }
    }
}
