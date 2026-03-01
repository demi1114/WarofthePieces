using UnityEngine;

[CreateAssetMenu(menuName = "Ability/LoseBothDeck")]
public class LoseBothDeckAbility : Ability
{
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        DeckManager.Instance.RemoveTopCards(0, amount);
        DeckManager.Instance.RemoveTopCards(1, amount);
    }
}