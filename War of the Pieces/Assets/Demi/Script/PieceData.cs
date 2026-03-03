using System.Collections.Generic;
using UnityEngine;

public enum PieceRace
{
    Human,
    Animal,
    Fairy,
    Wizard,
    Undead,
    Dragon,
    Machine,
    Sky,
    Sea
}
public enum PieceAttribute
{
    Normal,
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
    OnTurnEnd
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
    public MovePattern movePattern;

    [Header("Battle Stats")]
    public int basePower = 1;

    [Header("Piece Abilities")]
    public List<PieceAbilityEntry> abilities;

    public List<PieceData> allPieces;

    public List<PieceData> GetByRace(PieceRace race)
    {
        return allPieces.FindAll(p => p.race == race);
    }

    public List<PieceData> GetByAttribute(PieceAttribute attribute)
    {
        return allPieces.FindAll(p => p.attribute == attribute);
    }
}