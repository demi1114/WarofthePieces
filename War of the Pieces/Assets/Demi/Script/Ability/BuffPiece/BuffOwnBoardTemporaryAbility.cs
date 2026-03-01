using UnityEngine;

//このターンのみの種族バフ
[CreateAssetMenu(menuName = "Ability/Buff Own Board Temporary")]
public class BuffOwnBoardTemporaryAbility : Ability
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
                piece.AddTemporaryPower(amount);
            }
        }
    }
}