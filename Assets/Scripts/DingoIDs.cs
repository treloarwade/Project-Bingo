using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DingoIDs
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
