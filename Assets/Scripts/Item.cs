using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="New Item",menuName ="Item/Create New Item")]

public class Item : ScriptableObject
{
    public int id;
    public string Name;
    public string Type;
    public string Description;
    public int HP;
    public int Attack;
    public int Defense;
    public int Speed;
    public Sprite Icon;
    public int Level;
    public int XP;
}
