using System.Collections.Generic;
using UnityEngine;

public enum PieceRace
{
    Attack,
    Beast,
    Spirit,
    Undead,
    Angel,
    Slime
}
public enum PieceAttribute
{
    Human,
    Fairy,
    Machine,
    Dragon,
    Demon,
    God,
    Bystander,
    Hope
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