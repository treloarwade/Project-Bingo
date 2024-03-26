using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DingoSystem;

public static class DingoDatabase
{
    // Define properties for each Dingo type
    public static DingoID BingoStar { get; } = new DingoID("BingoStar", "Light", "BingoStar flies around the sky in search of powerful opponents.", 78, 84, 78, 100, "star");
    public static DingoID Bean { get; } = new DingoID("Bean", "Nature", "Bean has a grudge against certain people.", 78, 84, 78, 100, "bean");

    // Add more base Dingos as needed
}

