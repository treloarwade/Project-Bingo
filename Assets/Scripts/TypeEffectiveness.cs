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
    Nature,
    Ground,
    Dark,
    Abnormal,
    Spirit
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
            { DingoType.Nature, 1 },
            { DingoType.Ground, 0.5f },
            { DingoType.Dark, 1 },
            { DingoType.Abnormal, 1 },
            { DingoType.Spirit, 1 }
        }
    },
    { DingoType.Water, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 0.5f },
            { DingoType.Water, 0.5f },
            { DingoType.Fire, 2 },
            { DingoType.Ice, 1 },
            { DingoType.Nature, 1 },
            { DingoType.Ground, 1 },
            { DingoType.Dark, 1 },
            { DingoType.Abnormal, 1 },
            { DingoType.Spirit, 1 }
        }
    },
    { DingoType.Fire, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 1 },
            { DingoType.Water, 0.5f },
            { DingoType.Fire, 0.5f },
            { DingoType.Ice, 2 },
            { DingoType.Nature, 1 },
            { DingoType.Ground, 1 },
            { DingoType.Dark, 1 },
            { DingoType.Abnormal, 1 },
            { DingoType.Spirit, 1 }
        }
    },
    { DingoType.Ice, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 1 },
            { DingoType.Water, 1 },
            { DingoType.Fire, 0.5f },
            { DingoType.Ice, 0.5f },
            { DingoType.Nature, 2 },
            { DingoType.Ground, 1 },
            { DingoType.Dark, 1 },
            { DingoType.Abnormal, 1 },
            { DingoType.Spirit, 1 }
        }
    },
    { DingoType.Nature, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 1 },
            { DingoType.Water, 1 },
            { DingoType.Fire, 1 },
            { DingoType.Ice, 0.5f },
            { DingoType.Nature, 0.5f },
            { DingoType.Ground, 2 },
            { DingoType.Dark, 1 },
            { DingoType.Abnormal, 1 },
            { DingoType.Spirit, 1 }
        }
    },
    { DingoType.Ground, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 2 },
            { DingoType.Water, 1 },
            { DingoType.Fire, 1 },
            { DingoType.Ice, 1 },
            { DingoType.Nature, 0.5f },
            { DingoType.Ground, 0.5f },
            { DingoType.Dark, 1 },
            { DingoType.Abnormal, 1 },
            { DingoType.Spirit, 1 }
        }
    },
    { DingoType.Dark, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 1 },
            { DingoType.Water, 1 },
            { DingoType.Fire, 1 },
            { DingoType.Ice, 1 },
            { DingoType.Nature, 1 },
            { DingoType.Ground, 1 },
            { DingoType.Dark, 0.5f },
            { DingoType.Abnormal, 0.5f },
            { DingoType.Spirit, 2 }
        }
    },
    { DingoType.Abnormal, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 1 },
            { DingoType.Water, 1 },
            { DingoType.Fire, 1 },
            { DingoType.Ice, 1 },
            { DingoType.Nature, 1 },
            { DingoType.Ground, 1 },
            { DingoType.Dark, 2 },
            { DingoType.Abnormal, 0.5f },
            { DingoType.Spirit, 0.5f }
        }
    },
    { DingoType.Spirit, new Dictionary<DingoType, float>
        {
            { DingoType.Lightning, 1 },
            { DingoType.Water, 1 },
            { DingoType.Fire, 1 },
            { DingoType.Ice, 1 },
            { DingoType.Nature, 1 },
            { DingoType.Ground, 1 },
            { DingoType.Dark, 0.5f },
            { DingoType.Abnormal, 2 },
            { DingoType.Spirit, 0.5f }
        }
    }
        // Add other type interactions here...
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

