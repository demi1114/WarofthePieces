using UnityEngine;

//相手の駒ロスト
[CreateAssetMenu(menuName = "Ability/Lose Selected Enemy Piece")]
public class LoseSelectedEnemyPieceAbility : Ability
{
    public override void OnCardUse(AbilityContext context)
    {
        if (!context.hasTargetPosition) return;

        Piece target = BoardManager.Instance.GetPieceAt(context.targetPosition);
        if (target == null) return;

        if (target.owner == context.owner) return;

        BoardManager.Instance.RemovePiece(target);

        Debug.Log("相手の駒をロストしました");
        VictoryManager.Instance.CheckAfterAction();
    }
}