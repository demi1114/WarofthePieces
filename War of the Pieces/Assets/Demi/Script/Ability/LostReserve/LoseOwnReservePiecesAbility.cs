using UnityEngine;

//©•ª‚Ìè‹îƒ‰ƒ“ƒ_ƒ€íœ
[CreateAssetMenu(menuName = "Ability/LoseOwnReservePieces")]
public class LoseOwnReservePiecesAbility : Ability
{
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        for (int i = 0; i < amount; i++)
        {
            ReserveManager.Instance.RemoveRandomPiece(context.owner);
        }
    }
}