using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public int owner;
    public PieceData data;

    public void Initialize(PieceData pieceData, int ownerId)
    {
        data = pieceData;
        owner = ownerId;
        ApplyColor();
    }

    private void ApplyColor()
    {
        if (data == null) return;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null) return;

        renderer.material.color = data.color;
    }

    public PieceAttribute GetAttribute()
    {
        return data.attribute;
    }
    public List<Vector2Int> GetMovablePositions(
           Vector2Int currentPos,
           int boardSize)
    {
        if (data == null || data.movePattern == null)
            return new List<Vector2Int>();

        return data.movePattern.GetMoves(
            currentPos,
            owner,
            boardSize
        );
    }
}
