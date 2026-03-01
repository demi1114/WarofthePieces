using UnityEngine;

//“G‚Ìè‹îƒ‰ƒ“ƒ_ƒ€íœ
[CreateAssetMenu(menuName = "Ability/LoseEnemyReservePieces")]
public class LoseEnemyReservePiecesAbility : Ability
{
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        int targetOwner = 1 - context.owner;

        for (int i = 0; i < amount; i++)
        {
            ReserveManager.Instance.RemoveRandomPiece(targetOwner);
        }
    }
}