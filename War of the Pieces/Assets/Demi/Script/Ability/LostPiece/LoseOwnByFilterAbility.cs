using UnityEngine;
using System.Collections.Generic;

//©•ª‚Ìw’èí‘°‚Ü‚½‚Í‘®«‚Ì‹î‚ğ‘S”j‰ó
[CreateAssetMenu(menuName = "Ability/Lose Own By Filter")]
public class LoseOwnByFilterAbility : Ability
{
    public FilterType filterType;

    public PieceRace targetRace;
    public PieceAttribute targetAttribute;

    public override void OnCardUse(AbilityContext context)
    {
        int owner = context.owner;

        var pieces = BoardManager.Instance.GetPiecesByOwner(owner);

        List<Piece> targets = new List<Piece>();

        foreach (var piece in pieces)
        {
            if (filterType == FilterType.Race &&
                piece.data.race == targetRace)
                targets.Add(piece);

            if (filterType == FilterType.Attribute &&
                piece.data.attribute == targetAttribute)
                targets.Add(piece);
        }

        foreach (var piece in targets)
        {
            piece.Die();
        }

        Debug.Log("©•ª‚ÌğŒˆê’v‹î‚ğ‘S”j‰ó");
        VictoryManager.Instance.CheckAfterAction();
    }
}