using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Both Draw")]
public class BothDrawAbility : Ability
{
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        for (int i = 0; i < amount; i++)
        {
            DeckManager.Instance.DrawCard();
            EnemyDeckManager.Instance.DrawCard();
        }
    }
}