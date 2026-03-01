using UnityEngine;

[CreateAssetMenu(menuName = "Ability/LoseOwnDeck")]
public class LoseOwnDeckAbility : Ability
{
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        DeckManager.Instance.RemoveTopCards(context.owner, amount);
    }
}