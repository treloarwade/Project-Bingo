using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace DingoSystem
{

    // Class representing a move
    public class DingoMove
    {
        public int MoveID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Priority { get; set; }
        public int Power { get; set; }
        public int Accuracy { get; set; }
        public string StatusEffect { get; set; }
        public string EnvironmentEffect { get; set; }
        public string Description { get; set; }

        // Constructor
        public DingoMove(int moveid, string name, string type, int priority, int power, int accuracy, string statusEffect, string environmentEffect, string description)
        {
            MoveID = moveid;
            Name = name;
            Type = type;
            Priority = priority;
            Power = power;
            Accuracy = accuracy;
            StatusEffect = statusEffect;
            EnvironmentEffect = environmentEffect;
            Description = description;
        }
    }
    // In DingoIDs.cs - Enhanced StatusEffect class
    public class StatusEffect
    {
        public int ID { get; set; }
        public int Duration { get; set; }
        public string Name { get; set; }
        public float SkipTurnChance { get; set; } = 0f;
        public int DamagePerTurn { get; set; } = 0;
        public Dictionary<StatType, float> StatModifiers { get; } = new Dictionary<StatType, float>();

        public enum StatType { Attack, Defense, Speed, Accuracy, Evasion }

        public StatusEffect(int id, int duration, string name,
                           float skipTurnChance = 0f,
                           int damagePerTurn = 0)
        {
            ID = id;
            Duration = duration;
            Name = name;
            SkipTurnChance = skipTurnChance;
            DamagePerTurn = damagePerTurn;
        }

        public StatusEffect WithStatModifier(StatType stat, float modifier)
        {
            StatModifiers[stat] = modifier;
            return this;
        }

        // Factory method for sadlook
        public static StatusEffect CreateSadLookEffect(int duration = 3)
        {
            return new StatusEffect(200, duration, "sadlook")
                .WithStatModifier(StatType.Attack, 0.8f)
                .WithStatModifier(StatType.Defense, 0.8f)
                .WithStatModifier(StatType.Speed, 0.8f);
        }

        // Factory method for goo effect
        public static StatusEffect CreateGooEffect(int duration = 3)
        {
            return new StatusEffect(100, duration, "goo", 0.2f)
                .WithStatModifier(StatType.Speed, 0.8f);
        }
        public static StatusEffect CreateMarketAnalysisEffect()
        {
            return new StatusEffect(300, 999, "marketanalysis");
        }

        // Helper method to get a modifier (returns 1f if not found)
        public float GetStatModifier(StatType stat)
        {
            return StatModifiers.TryGetValue(stat, out var modifier) ? modifier : 1f;
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
