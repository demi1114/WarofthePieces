using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AbilityEffect
{
    Draw,
    AddReserve,
    DestroyReserve,
    TransformReserve,
    DestroyDeck,
    DestroyBoard,
    TransformBoard,
    AddMove,
    Buff,
    Debuff,
    ReturnToReserve
}

public enum AbilityZone
{
    Deck,
    Hand,
    Reserve,
    Board
}

public enum AbilityFilter
{
    None,
    Race,
    Attribute,
    SpecificPiece
}

public enum AbilityTarget
{
    Self,
    Enemy,
    Both
}

public enum AbilityRange
{
    None,
    All,
    Random,
    Count
}

[CreateAssetMenu(menuName = "Ability/Unified Ability")]
public class UnifiedAbility : Ability
{
    [Header("Effect")]
    public AbilityEffect effect;

    [Header("Zone")]
    public AbilityZone zone;

    [Header("Target")]
    public AbilityTarget target;

    [Header("Filter")]
    public AbilityFilter filter;

    public PieceRace race;
    public PieceAttribute attribute;
    public List<PieceData> specificPieces;

    [Header("Range")]
    public AbilityRange range;

    [Header("Target Count")]
    public int targetCount = 1;

    [Header("Effect Value")]
    public int effectValue = 1;

    [Header("Transform Target")]
    public PieceData transformTarget;

    public override void OnCardUse(AbilityContext context)
    {
        // PieceëŒè€Ç∂Ç·Ç»Ç¢å¯â 
        if (effect == AbilityEffect.Draw)
        {
            for (int i = 0; i < effectValue; i++)
                DeckManager.Instance.DrawCard(context.owner);

            return;
        }

        if (effect == AbilityEffect.DestroyDeck)
        {
            DeckManager.Instance.RemoveTopCards(context.owner, effectValue);
            return;
        }

        if (effect == AbilityEffect.AddMove)
        {
            TurnManager.Instance.AddExtraMove(context.owner, effectValue);
            return;
        }

        // ===== PieceëŒè€å¯â  =====

        List<Piece> targets = GetTargets(context);

        if (range == AbilityRange.Random)
            targets = targets.OrderBy(x => Random.value).Take(targetCount).ToList();

        if (range == AbilityRange.Count)
            targets = targets.Take(targetCount).ToList();

        foreach (var piece in targets)
            ExecuteEffect(piece, context);
    }

    List<Piece> GetTargets(AbilityContext context)
    {
        List<Piece> pieces = new List<Piece>();

        List<int> owners = GetTargetOwners(context.owner);

        if (zone == AbilityZone.Board)
        {
            foreach (var owner in owners)
                pieces.AddRange(BoardManager.Instance.GetPiecesByOwner(owner));
        }

        pieces = pieces.Where(FilterPiece).ToList();

        return pieces;
    }

    List<int> GetTargetOwners(int owner)
    {
        List<int> list = new List<int>();

        if (target == AbilityTarget.Self)
            list.Add(owner);

        if (target == AbilityTarget.Enemy)
            list.Add(1 - owner);

        if (target == AbilityTarget.Both)
        {
            list.Add(owner);
            list.Add(1 - owner);
        }

        return list;
    }

    bool FilterPiece(Piece piece)
    {
        switch (filter)
        {
            case AbilityFilter.None:
                return true;

            case AbilityFilter.Race:
                return piece.data.race == race;

            case AbilityFilter.Attribute:
                return piece.data.attribute == attribute;

            case AbilityFilter.SpecificPiece:
                return specificPieces != null && specificPieces.Contains(piece.data);
        }

        return true;
    }

    void ExecuteEffect(Piece piece, AbilityContext context)
    {
        switch (effect)
        {
            case AbilityEffect.DestroyBoard:
                piece.Die();
                break;

            case AbilityEffect.Buff:
                piece.AddPermanentPower(effectValue);
                break;

            case AbilityEffect.Debuff:
                piece.AddPermanentPower(-effectValue);
                break;

            case AbilityEffect.TransformBoard:
                BoardManager.Instance.ReplacePiece(piece, transformTarget);
                break;

            case AbilityEffect.ReturnToReserve:
                BoardManager.Instance.ReturnPieceToReserve(piece);
                break;
        }
    }
}