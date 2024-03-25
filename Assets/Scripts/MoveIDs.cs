using System;
using System.Collections.Generic;

namespace DingoMoves
{
    public class DingoMove
    {
        public string Name { get; set; }
        public int Damage { get; set; }
        public string Type { get; set; }

        public DingoMove(string name, int damage, string type)
        {
            Name = name;
            Damage = damage;
            Type = type;
        }
    }

    public static class MoveDatabase
    {
        public static List<DingoMove> Moves { get; } = new List<DingoMove>
        {
            new DingoMove("Thunderbolt", 90, "Electric"),
            new DingoMove("Flamethrower", 95, "Fire"),
            new DingoMove("Ice Beam", 90, "Ice"),
            new DingoMove("Earthquake", 100, "Ground"),
            // Add more moves here
        };
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Accessing moves from the MoveDatabase
            DingoMove thunderbolt = MoveDatabase.Moves[0]; // Accessing the first move (index 0)
            DingoMove flamethrower = MoveDatabase.Moves[1]; // Accessing the second move (index 1)
        }
    }
}
