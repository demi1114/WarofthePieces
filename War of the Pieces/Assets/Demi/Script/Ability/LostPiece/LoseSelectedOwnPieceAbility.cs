using UnityEngine;

//自分の駒ロスト
[CreateAssetMenu(menuName = "Ability/Lose Selected Own Piece")]
public class LoseSelectedOwnPieceAbility : Ability
{
    public override void OnCardUse(AbilityContext context)
    {
        if (!context.hasTargetPosition) return;

        Piece target = BoardManager.Instance.GetPieceAt(context.targetPosition);
        if (target == null) return;

        if (target.owner != context.owner) return;

        BoardManager.Instance.RemovePiece(target);

        Debug.Log("自分の駒をロストしました");
    }
}