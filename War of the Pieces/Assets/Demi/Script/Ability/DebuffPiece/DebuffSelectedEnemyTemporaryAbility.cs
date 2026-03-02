using UnityEngine;

//このターンのみ相手選択デバフ
[CreateAssetMenu(menuName = "Ability/Debuff Selected Enemy Temporary")]
public class DebuffSelectedEnemyTemporaryAbility : Ability
{
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        if (!context.hasTargetPosition) return;

        Piece target = BoardManager.Instance.GetPieceAt(context.targetPosition);
        if (target == null) return;

        if (target.owner == context.owner) return;

        target.AddTemporaryPower(-amount);
    }
}