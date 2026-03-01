using UnityEngine;

//‰i‘±Ží‘°ƒoƒt
[CreateAssetMenu(menuName = "Ability/Buff Own Board Permanent")]
public class BuffOwnBoardPermanentAbility : Ability
{
    public PieceAttribute targetAttribute;
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        var pieces = BoardManager.Instance.GetPiecesByOwner(context.owner);

        foreach (var piece in pieces)
        {
            if (piece.GetAttribute() == targetAttribute)
            {
                piece.AddPermanentPower(amount);
            }
        }
    }
}