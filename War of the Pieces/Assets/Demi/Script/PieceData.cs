using UnityEngine;

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
    public PieceAttribute attribute;
    public MovePattern movePattern;
}