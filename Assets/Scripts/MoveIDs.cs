using System;
using System.Collections.Generic;

namespace PokemonMoves
{
    public class PokemonMove
    {
        public string Name { get; set; }
        public int Damage { get; set; }
        public string Type { get; set; }

        public PokemonMove(string name, int damage, string type)
        {
            Name = name;
            Damage = damage;
            Type = type;
        }
    }

    public static class MoveDatabase
    {
        public static List<PokemonMove> Moves { get; } = new List<PokemonMove>
        {
            new PokemonMove("Thunderbolt", 90, "Electric"),
            new PokemonMove("Flamethrower", 95, "Fire"),
            new PokemonMove("Ice Beam", 90, "Ice"),
            new PokemonMove("Earthquake", 100, "Ground"),
            // Add more moves here
        };
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Accessing moves from the MoveDatabase
            PokemonMove thunderbolt = MoveDatabase.Moves[0]; // Accessing the first move (index 0)
            PokemonMove flamethrower = MoveDatabase.Moves[1]; // Accessing the second move (index 1)
        }
    }
}
