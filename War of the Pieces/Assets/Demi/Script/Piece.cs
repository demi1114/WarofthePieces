using UnityEngine;
using System.Collections.Generic;

public class Piece : MonoBehaviour
{
    public int owner;  // 0=プレイヤー, 1=敵
    public PieceData data;

    public List<Ability> abilities;

    public int BasePower { get; private set; }
    public int CurrentPower { get; private set; }

    private int permanentModifier = 0;
    private int temporaryModifier = 0;

    public void Initialize(PieceData pieceData, int owner)
    {
        this.data = pieceData;
        this.owner = owner;

        BasePower = pieceData.basePower;

        permanentModifier = 0;
        temporaryModifier = 0;

        RecalculatePower();
    }

    // 🔄 変身処理
    public void Transform(PieceData newData)
    {
        if (newData == null) return;

        data = newData;
        BasePower = newData.basePower;

        // 一時効果は消す
        temporaryModifier = 0;

        RecalculatePower();

        Debug.Log($"駒が {newData.pieceName} に変身しました");
    }

    // 🟢 永続強化
    public void AddPermanentPower(int amount)
    {
        permanentModifier += amount;
        RecalculatePower();
    }

    // 🟡 一時強化
    public void AddTemporaryPower(int amount)
    {
        temporaryModifier += amount;
        RecalculatePower();
    }

    // 🔁 ターン終了時
    public void ResetTemporaryPower()
    {
        temporaryModifier = 0;
        RecalculatePower();
    }

    // 💡 常にここから計算する
    private void RecalculatePower()
    {
        CurrentPower = BasePower + permanentModifier + temporaryModifier;

        // ここでUI更新など
    }

    public List<Vector2Int> GetMovablePositions(Vector2Int currentPos, int boardSize)
    {
        if (data.movePattern == null) return new List<Vector2Int>();
        return data.movePattern.GetMoves(currentPos, owner, boardSize);
    }

    public PieceAttribute GetAttribute() => data.attribute;
}