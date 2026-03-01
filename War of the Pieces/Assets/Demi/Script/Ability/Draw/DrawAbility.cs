using UnityEngine;

//使用者がドローする
[CreateAssetMenu(menuName = "Ability/Draw")]
public class DrawAbility : Ability
{
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        for (int i = 0; i < amount; i++)
        {
            if (context.owner == 0)
                DeckManager.Instance.DrawCard();
            else
                EnemyDeckManager.Instance.DrawCard();
        }
    }
}