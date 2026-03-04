using System.Collections.Generic;
using UnityEngine;

public enum CardCategory
{
    AddMove,
    AddReserve,
    Bounce,
    BuffPiece,
    Draw,
    LostDeck,
    LostPiece,
    LostReserve,
    Transform
}

[CreateAssetMenu(menuName = "Card/Create Card")]
public class CardData : ScriptableObject
{
    public string cardName;

    [Header("Abilities")]
    public List<Ability> abilities = new List<Ability>();

    public CardCategory category;
    public PieceAttribute targetPieceAttribute;
    [Header("Effect Settings")]
    public int amount = 1;        // 強化量・弱体量・ドロー枚数
    public int targetCount = 1;   // 対象数（複数効果用）
    [Header("Piece Add Settings")]
    public PieceData specificPiece;          // 特定の駒
    public List<PieceData> candidatePieces;  // ランダム候補リスト

    public virtual bool Resolve(int owner, Vector2Int targetPos)
    {
        AbilityContext context = new AbilityContext
        {
            owner = owner,
            hasTargetPosition = true,
            targetPosition = targetPos,
            sourceCard = this
        };

        foreach (var ability in abilities)
        {
            ability.OnCardUse(context);
        }
        return true;
    }

}
