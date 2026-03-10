using System.Collections.Generic;
using UnityEngine;

public enum PieceRace
{
    Human,
    Animal,
    Spirit,
    Flower,
    Wizard,
    Undead,
    Dragon,
    Machine,
    Sky,
    Sea
}
public enum PieceAttribute
{
    None,
    Fire,
    Water,
    Electric,
    Ground,
    Wind,
    Ice,
    Dark,
    Hope
}
public enum PieceAbilityTrigger
{
    OnTurnStart,
    OnTurnEnd,
    OnDeath
}

[System.Serializable]
public class PieceAbilityEntry
{
    public Ability ability;
    public PieceAbilityTrigger triggerTiming;
}

[CreateAssetMenu(menuName = "Game/PieceData")]
public class PieceData : ScriptableObject
{
    public string pieceName;
    public PieceRace race;
    public PieceAttribute attribute;
    public List<MovePattern> movePatterns;

    [Header("Battle Stats")]
    public int basePower = 1;

    [Header("Visual")]
    public GameObject piecePrefab;

    [Header("Piece Abilities")]
    public List<PieceAbilityEntry> abilities;

    [Header("Description")]
    [TextArea(3, 6)]
    public string description;
}