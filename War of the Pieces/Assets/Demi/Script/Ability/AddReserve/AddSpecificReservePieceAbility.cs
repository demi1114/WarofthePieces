using UnityEngine;

//Ž©•ª‚ÌŽè‹î‚É“Á’è‚Ì‹î‚ð’Ç‰Á
[CreateAssetMenu(menuName = "Ability/AddSpecificReservePiece")]
public class AddSpecificReservePieceAbility : Ability
{
    public PieceData piece;
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        if (piece == null) return;

        for (int i = 0; i < amount; i++)
        {
            ReserveManager.Instance.AddPiece(context.owner, piece);
        }
    }
}