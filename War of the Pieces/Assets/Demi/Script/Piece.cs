using UnityEngine;
using System.Collections.Generic;

public class Piece : MonoBehaviour
{
    public int owner;  // 0=プレイヤー, 1=敵
    public PieceData data;

    public List<Ability> abilities;

    public int BasePower { get; private set; }
    public int CurrentPower { get; private set; }

    public void Initialize(PieceData pieceData, int owner)
    {
        this.data = pieceData;
        this.owner = owner;

        BasePower = pieceData.basePower;
        CurrentPower = BasePower;
    }

    public void Transform(PieceData newData) // 変身処理
    {
        if (newData == null) return;

        data = newData;

        // 基礎パワーを新しい駒に合わせる
        BasePower = newData.basePower;

        // 一時強化はリセットする設計にする（安全）
        CurrentPower = BasePower;

        Debug.Log($"駒が {newData.pieceName} に変身しました");
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

   /* public void TriggerTurnStart()
    {
        AbilityContext context = new AbilityContext
        {
            owner = owner,
            sourcePiece = this
        };

        foreach (var ability in data.abilities)
        {
            ability.OnTurnStart(context);
        }
    }*/
}