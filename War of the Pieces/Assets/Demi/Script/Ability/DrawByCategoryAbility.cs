using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Draw By Category")]
public class DrawByCategoryAbility : Ability
{
    public CardCategory category;
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        for (int i = 0; i < amount; i++)
        {
            if (context.owner == 0)
                DeckManager.Instance.DrawCardByCategory(category);
            else
                EnemyDeckManager.Instance.DrawCardByCategory(category);
        }
    }
}