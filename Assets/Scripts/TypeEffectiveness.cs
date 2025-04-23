using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DingoType
{
    Lightning,
    Water,
    Fire,
    Ice,
    Grass,
    Dark,
    Abnormal,
    Light,
    Finance,
    Physical,
    Wind
}

public class DingoTypeEffectivenessCalculator
{
    private static readonly Dictionary<DingoType, Dictionary<DingoType, float>> TypeChart = new Dictionary<DingoType, Dictionary<DingoType, float>>
    {
    { DingoType.Lightning, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 0.5f },
            { DingoType.Water, 2 },
            { DingoType.Fire, 1 },
            { DingoType.Ice, 1 },
            { DingoType.Grass, 1 },
            { DingoType.Dark, 1 },
            { DingoType.Abnormal, 1 },
            { DingoType.Physical, 1 },
            { DingoType.Finance, 1 },
            { DingoType.Light, 1 },
            { DingoType.Wind, 1 }

        }
    },
    { DingoType.Water, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 0.5f },
            { DingoType.Water, 0.5f },
            { DingoType.Fire, 2 },
            { DingoType.Ice, 1 },
            { DingoType.Grass, 1 },
            { DingoType.Dark, 1 },
            { DingoType.Abnormal, 1 },
            { DingoType.Physical, 1 },
            { DingoType.Finance, 1 },
            { DingoType.Light, 1 },
            { DingoType.Wind, 1 }
        }
    },
    { DingoType.Fire, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 1 },
            { DingoType.Water, 0.5f },
            { DingoType.Fire, 0.5f },
            { DingoType.Ice, 2 },
            { DingoType.Grass, 1 },
            { DingoType.Dark, 1 },
            { DingoType.Abnormal, 1 },
            { DingoType.Physical, 1 },
            { DingoType.Finance, 1 },
            { DingoType.Light, 1 },
            { DingoType.Wind, 1 }
        }
    },
    { DingoType.Ice, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 1 },
            { DingoType.Water, 1 },
            { DingoType.Fire, 0.5f },
            { DingoType.Ice, 0.5f },
            { DingoType.Grass, 2 },
            { DingoType.Dark, 1 },
            { DingoType.Abnormal, 1 },
            { DingoType.Physical, 1 },
            { DingoType.Finance, 1 },
            { DingoType.Light, 1 },
            { DingoType.Wind, 1 }
        }
    },
    { DingoType.Grass, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 1 },
            { DingoType.Water, 1 },
            { DingoType.Fire, 1 },
            { DingoType.Ice, 0.5f },
            { DingoType.Grass, 0.5f },
            { DingoType.Dark, 1 },
            { DingoType.Abnormal, 1 },
            { DingoType.Physical, 1 },
            { DingoType.Finance, 1 },
            { DingoType.Light, 1 },
            { DingoType.Wind, 1 }
        }
    },
    { DingoType.Dark, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 1 },
            { DingoType.Water, 1 },
            { DingoType.Fire, 1 },
            { DingoType.Ice, 1 },
            { DingoType.Grass, 1 },
            { DingoType.Dark, 0.5f },
            { DingoType.Abnormal, 0.5f },
            { DingoType.Physical, 1 },
            { DingoType.Finance, 1 },
            { DingoType.Light, 1 },
            { DingoType.Wind, 1 }
        }
    },
    { DingoType.Abnormal, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 1 },
            { DingoType.Water, 1 },
            { DingoType.Fire, 1 },
            { DingoType.Ice, 1 },
            { DingoType.Grass, 1 },
            { DingoType.Dark, 2 },
            { DingoType.Abnormal, 0.5f },
            { DingoType.Physical, 1 },
            { DingoType.Finance, 1 },
            { DingoType.Light, 1 },
            { DingoType.Wind, 1 }
        }
    },
    { DingoType.Physical, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 1 },
            { DingoType.Water, 1 },
            { DingoType.Fire, 1 },
            { DingoType.Ice, 1 },
            { DingoType.Grass, 1 },
            { DingoType.Dark, 1 },
            { DingoType.Abnormal, 1 },
            { DingoType.Physical, 0.5f },
            { DingoType.Finance, 1 },
            { DingoType.Light, 1 },
            { DingoType.Wind, 1 }
        }
    },
    { DingoType.Wind, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 1 },
            { DingoType.Water, 1 },
            { DingoType.Fire, 1 },
            { DingoType.Ice, 1 },
            { DingoType.Grass, 1 },
            { DingoType.Dark, 1 },
            { DingoType.Abnormal, 1 },
            { DingoType.Physical, 1 },
            { DingoType.Finance, 1 },
            { DingoType.Light, 1 },
            { DingoType.Wind, 0.5f }
        }
    },
    { DingoType.Finance, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 1 },
            { DingoType.Water, 1 },
            { DingoType.Fire, 1 },
            { DingoType.Ice, 1 },
            { DingoType.Grass, 1 },
            { DingoType.Dark, 1 },
            { DingoType.Abnormal, 1 },
            { DingoType.Physical, 1 },
            { DingoType.Finance, 1 },
            { DingoType.Light, 1 },
            { DingoType.Wind, 1 }
        }
    },

    { DingoType.Light, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 1 },
            { DingoType.Water, 1 },
            { DingoType.Fire, 1 },
            { DingoType.Ice, 1 },
            { DingoType.Grass, 1 },
            { DingoType.Dark, 1 },
            { DingoType.Abnormal, 1 },
            { DingoType.Physical, 1 },
            { DingoType.Finance, 1 },
            { DingoType.Light, 0.5f },
            { DingoType.Wind, 1 }
        }
    }
    };

    public static float GetEffectiveness(DingoType attacker, DingoType defender)
    {
        if (!TypeChart.ContainsKey(attacker) || !TypeChart[attacker].ContainsKey(defender))
        {
            throw new ArgumentException("Invalid Dingo types.");
        }

        return TypeChart[attacker][defender];
    }
}

