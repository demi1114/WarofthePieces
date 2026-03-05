using UnityEngine;
using System.Collections.Generic;

public class Piece : MonoBehaviour
{
    public int owner;  // 0=プレイヤー, 1=敵
    public PieceData data;

    public int BasePower { get; private set; }
    public int CurrentPower { get; private set; }
    private int permanentModifier = 0;
    private int temporaryModifier = 0;

    private bool isDead = false;

    public void Initialize(PieceData pieceData, int owner)
    {
        this.data = pieceData;
        this.owner = owner;

        BasePower = pieceData.basePower;

        permanentModifier = 0;
        temporaryModifier = 0;
        isDead = false;

        RecalculatePower();
    }

    public void TriggerAbilities(PieceAbilityTrigger timing)
    {
        if (data == null) return;
        if (data.abilities == null) return;

        foreach (var entry in data.abilities)
        {
            if (entry == null) continue;
            if (entry.ability == null) continue;
            if (entry.triggerTiming != timing) continue;

            AbilityContext context = new AbilityContext
            {
                owner = owner,
                sourcePiece = this
            };

            entry.ability.OnCardUse(context);
        }
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
        if (isDead) return;

        CurrentPower = BasePower + permanentModifier + temporaryModifier;

        if (CurrentPower <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{data.pieceName} がパワー0で死亡");

        TriggerAbilities(PieceAbilityTrigger.OnDeath);
        BoardManager.Instance.RemovePiece(this);

        VictoryManager.Instance.CheckAfterAction();
    }

    public List<Vector2Int> GetMovablePositions(Vector2Int currentPos, int boardSize)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        if (data.movePatterns == null) return result;

        foreach (var pattern in data.movePatterns)
        {
            if (pattern == null) continue;

            var moves = pattern.GetMoves(currentPos, owner, boardSize);

            foreach (var m in moves)
            {
                if (!result.Contains(m))
                    result.Add(m);
            }
        }

        return result;
    }

    public PieceAttribute GetAttribute() => data.attribute;
}