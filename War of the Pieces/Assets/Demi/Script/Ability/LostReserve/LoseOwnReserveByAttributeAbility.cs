using UnityEngine;

//©•ª‚Ìè‹î‚Ì“Á’è‘®«ƒ‰ƒ“ƒ_ƒ€íœ
[CreateAssetMenu(menuName = "Ability/LoseOwnReserveByAttribute")]
public class LoseOwnReserveByAttributeAbility : Ability
{
    public PieceAttribute targetAttribute;
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        for (int i = 0; i < amount; i++)
        {
            ReserveManager.Instance.RemoveRandomPieceByAttribute(context.owner, targetAttribute);
        }
    }
}