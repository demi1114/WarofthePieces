using UnityEngine;

[CreateAssetMenu(menuName = "Ability/LoseEnemyDeck")]
public class LoseEnemyDeckAbility : Ability
{
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        int targetOwner = 1 - context.owner;

        DeckManager.Instance.RemoveTopCards(targetOwner, amount);
    }
}