using UnityEngine;

//ŒÅ’è‚ÌŽè‹î’Ç‰Á
[CreateAssetMenu(menuName = "Ability/Add Reserve Piece")]
public class AddReserveAbility : Ability
{
    public PieceData piece;
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        Debug.Log("Ability owner = " + context.owner);
        if (piece == null) return;

        for (int i = 0; i < amount; i++)
        {
            ReserveManager.Instance.AddPiece(context.owner, piece);
        }
    }
}