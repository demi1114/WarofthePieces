using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Buff Own Board Permanent By Race")]
public class BuffOwnBoardPermanentByRaceAbility : Ability
{
    public PieceRace targetRace;
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        var pieces = BoardManager.Instance.GetPiecesByOwner(context.owner);

        foreach (var piece in pieces)
        {
            if (piece.data.race == targetRace)
            {
                piece.AddPermanentPower(amount);
            }
        }
    }
}