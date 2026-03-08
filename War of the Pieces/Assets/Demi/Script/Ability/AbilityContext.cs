using UnityEngine;

public class AbilityContext
{
    public int owner;
    public Piece sourcePiece;
    public CardData sourceCard; 
    public bool hasTargetPosition;
    public Vector2Int targetPosition;
    public Piece targetPiece;
    public int value;
}