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
}