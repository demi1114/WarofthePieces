using UnityEngine;

//特定カードサーチ
[CreateAssetMenu(menuName = "Ability/Draw By Category")]
public class DrawByCategoryAbility : Ability
{
    public CardCategory category;
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        for (int i = 0; i < amount; i++)
        {
            DeckManager.Instance.DrawCardByCategory(context.owner, category);
        }
    }
}