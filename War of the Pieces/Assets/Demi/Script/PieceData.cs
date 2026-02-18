using UnityEngine;
public enum PieceAttribute
{
    Fire,
    Wood,
    Water
}

[CreateAssetMenu(menuName = "Game/PieceData")]

public class PieceData : ScriptableObject
{
    public string pieceName;
    public PieceAttribute attribute;
    public Color color;
    public MovePattern movePattern;
}
