using UnityEngine;
using System.Collections.Generic;

public class Piece : MonoBehaviour
{
    public int owner;  // 0=ÉvÉåÉCÉÑÅ[, 1=ìG
    public PieceData data;

    public int BasePower { get; private set; }
    public int CurrentPower { get; private set; }

    public void Initialize(PieceData pieceData, int owner)
    {
        this.data = pieceData;
        this.owner = owner;

        BasePower = pieceData.basePower;
        CurrentPower = BasePower;
    }

    public void AddPermanentPower(int amount)
    {
        BasePower += amount;
        CurrentPower += amount;
    }

    public void AddTemporaryPower(int amount)
    {
        CurrentPower += amount;
    }

    public void ResetTemporaryPower()
    {
        CurrentPower = BasePower;
    }
    public List<Vector2Int> GetMovablePositions(Vector2Int currentPos, int boardSize)
    {
        if (data.movePattern == null) return new List<Vector2Int>();
        return data.movePattern.GetMoves(currentPos, owner, boardSize);
    }

    public PieceAttribute GetAttribute() => data.attribute;
}