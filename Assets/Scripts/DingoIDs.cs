using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DingoSystem
{

    // Class representing a move
    public class DingoMove
    {
        public int MoveID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Power { get; set; }
        public int Accuracy { get; set; }
        public string Description { get; set; }

        // Constructor
        public DingoMove(int moveid, string name, string type, int power, int accuracy, string description)
        {
            MoveID = moveid;
            Name = name;
            Type = type;
            Power = power;
            Accuracy = accuracy;
            Description = description;
        }
    }
    public class StatusEffect
    {
        public int ID { get; set; }
        public int Duration { get; set; } // Duration of the status effect in turns
        public string Name { get; set; }
        public int Value { get; set; }

        public StatusEffect(int id, int duration, string name, int value)
        {
            ID = id;
            Duration = duration;
            Name = name;
            Value = value;
        }
    }
    public class EnvironmentEffect
    {
        public int ID { get; set; }
        public int Duration { get; set; } // Duration of the status effect in turns
        public string Name { get; set; }

        public EnvironmentEffect(int id, int duration, string name)
        {
            ID = id;
            Duration = duration;
            Name = name;
        }
    }



    [System.Serializable]
    public class DingoID
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Speed { get; set; }
        public string Sprite { get; set; }
        public int MaxHP { get; set; }
        public int XP { get; set; }
        public int MaxXP { get; set; }
        public int Level { get; set; }

        public List<DingoMove> Moves { get; } = new List<DingoMove>(); // List to hold moves

        // Constructor
        public DingoID(int id, string name, string type, string description, int hp, int attack, int defense, int speed, string sprite, int maxHP, int xp, int maxXP, int level)
        {
            ID = id;
            Name = name;
            Type = type;
            Description = description;
            HP = hp;
            Attack = attack;
            Defense = defense;
            Speed = speed;
            Sprite = sprite;
            MaxHP = maxHP;
            XP = xp;
            MaxXP = maxXP;
            Level = level;

            Moves = new List<DingoMove>(); // Initialize the list of moves
        }

        // Method to add a move
        public void AddMove(DingoMove move)
        {
            Moves.Add(move);
        }

        // Method to remove a move
        public void RemoveMove(DingoMove move)
        {
            Moves.Remove(move);
        }
    }
}
