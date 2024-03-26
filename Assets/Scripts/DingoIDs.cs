using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DingoSystem
{
    public class DingoID
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Speed { get; set; }
        public string Sprite { get; set; }

        // Constructor
        public DingoID(string name, string type, string description, int hp, int attack, int defense, int speed, string sprite)
        {
            Name = name;
            Type = type;
            Description = description;
            HP = hp;
            Attack = attack;
            Defense = defense;
            Speed = speed;
            Sprite = sprite;
        }
    }

    public class PlayerDingo : DingoID
    {
        // Additional properties for player-owned Dingos
        public string Nickname { get; set; }
        public int Level { get; set; }
        public List<DingoMoves.DingoMove> Moves { get; set; }

        // Constructor
        public PlayerDingo(string name, string type, string description, int hp, int attack, int defense, int speed, string sprite,
                           string nickname, int level, List<DingoMoves.DingoMove> moves)
                           : base(name, type, description, hp, attack, defense, speed, sprite)
        {
            Nickname = nickname;
            Level = level;
            Moves = moves;
        }
    }

    public class BingoStar : DingoID
    {
        // Constructor for BingoStar
        public BingoStar() : base("BingoStar", "Light", "BingoStar flies around the sky in search of powerful opponents.", 78, 84, 78, 100, "star")
        {
            // Additional properties or methods specific to BingoStar can be added here if needed.
        }
    }

    public class Bean : DingoID
    {
        // Constructor for Charizard
        public Bean() : base("Bean", "Nature", "Bean has a grudge against certain people.", 78, 84, 78, 100, "bean")
        {
            // Additional properties or methods specific to Charizard can be added here if needed.
        }
    }
}
