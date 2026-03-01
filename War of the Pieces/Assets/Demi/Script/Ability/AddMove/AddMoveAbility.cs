using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Add Move")]
public class AddMoveAbility : Ability
{
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        // このカードを使った側に移動回数を追加
        TurnManager.Instance.AddExtraMove(context.owner, amount);
    }
}