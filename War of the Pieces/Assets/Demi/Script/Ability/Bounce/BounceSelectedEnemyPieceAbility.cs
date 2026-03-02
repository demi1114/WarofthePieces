using UnityEngine;

//選んだ相手の駒をバウンス
[CreateAssetMenu(menuName = "Ability/Bounce Selected Enemy Piece")]
public class BounceSelectedEnemyPieceAbility : Ability
{
    public override void OnCardUse(AbilityContext context)
    {
        if (!context.hasTargetPosition) return;

        Piece target = BoardManager.Instance.GetPieceAt(context.targetPosition);
        if (target == null) return;

        if (target.owner == context.owner) return;

        BoardManager.Instance.ReturnPieceToReserve(target);

        Debug.Log("相手の駒を手駒に戻しました");
    }
}