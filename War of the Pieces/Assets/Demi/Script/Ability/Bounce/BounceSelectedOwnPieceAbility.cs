using UnityEngine;

//選んだ自分の駒をバウンス
[CreateAssetMenu(menuName = "Ability/Bounce Selected Own Piece")]
public class BounceSelectedOwnPieceAbility : Ability
{
    public override void OnCardUse(AbilityContext context)
    {
        if (!context.hasTargetPosition) return;

        Piece target = BoardManager.Instance.GetPieceAt(context.targetPosition);
        if (target == null) return;

        if (target.owner != context.owner) return;

        BoardManager.Instance.ReturnPieceToReserve(target);

        Debug.Log("自分の駒を手駒に戻しました");
    }
}