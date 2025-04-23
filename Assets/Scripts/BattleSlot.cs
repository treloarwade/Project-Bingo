using DingoSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleSlot
{
    public NetworkDingo Dingo { get; set; }
    public int SlotIndex { get; set; }
    public bool IsPlayer { get; set; }

    // Stat modifiers
    public float AttackMod { get; set; } = 1f;
    public float DefenseMod { get; set; } = 1f;
    public float SpeedMod { get; set; } = 1f;
    public bool TempStatsModified => SpeedMod != 1f;


    // List of status effects
    public List<StatusEffect> StatusEffects { get; } = new List<StatusEffect>();

    public BattleSlot(NetworkDingo dingo, int slotIndex, bool isPlayer)
    {
        Dingo = dingo;
        SlotIndex = slotIndex;
        IsPlayer = isPlayer;
    }

    public void ResetTempStats()
    {
        AttackMod = DefenseMod = SpeedMod = 1f;
        StatusEffects.Clear();
    }
    public int GetModifiedStat(StatusEffect.StatType statType, int baseValue)
    {
        float modifier = 1f;

        // Multiply all relevant modifiers
        foreach (var status in StatusEffects)
        {
            if (status.StatModifiers.TryGetValue(statType, out float statMod))
            {
                modifier *= statMod;
            }
        }

        return Mathf.RoundToInt(baseValue * modifier);
    }
    public int GetModifiedAttack() => GetModifiedStat(StatusEffect.StatType.Attack, Dingo.attack.Value);
    public int GetModifiedDefense() => GetModifiedStat(StatusEffect.StatType.Defense, Dingo.defense.Value);
    public int GetModifiedSpeed() => GetModifiedStat(StatusEffect.StatType.Speed, Dingo.speed.Value);
    // Add this to your BattleSlot class
    public bool HasStatus(string status)
    {
        return StatusEffects.Any(se => se.Name == status.ToLower());
    }
}