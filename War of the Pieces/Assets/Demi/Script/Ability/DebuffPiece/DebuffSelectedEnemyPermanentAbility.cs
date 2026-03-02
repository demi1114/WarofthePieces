using UnityEngine;

//永続の相手選択デバフ
[CreateAssetMenu(menuName = "Ability/Debuff Selected Enemy Permanent")]
public class DebuffSelectedEnemyPermanentAbility : Ability
{
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        if (!context.hasTargetPosition) return;

        Piece target = BoardManager.Instance.GetPieceAt(context.targetPosition);
        if (target == null) return;

        if (target.owner == context.owner) return;

        target.AddPermanentPower(-amount);
    }
}