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

public enum AbilityShape
{
    Single,
    Row,
    Column,
    Cross,
    Area
}

[CreateAssetMenu(menuName = "Ability/Unified Ability")]
public class UnifiedAbility : Ability
{
    [SerializeField] PieceDatabase pieceDatabase;

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

    [Header("Shape")]
    public AbilityShape shape = AbilityShape.Single;

    [Header("Fixed Column")]
    public int fixedColumn = -1;

    [Header("Fixed Row")]
    public int fixedRow = -1;

    [Header("Area Size")]
    public int areaRadius = 1;

    [Header("Target Count")]
    public int targetCount = 1;

    [Header("Effect Value")]
    public int effectValue = 1;

    [Header("Transform Target")]
    public PieceData transformTarget;

    [Header("AddReserve Options")]
    public bool addFromRaceRandom = false;

    public override void OnCardUse(AbilityContext context)
    {
        // PieceëŒè€Ç∂Ç·Ç»Ç¢å¯â 
        if (effect == AbilityEffect.Draw)
        {
            for (int i = 0; i < effectValue; i++)
                DeckManager.Instance.DrawCard(context.owner);

            return;
        }

        if (effect == AbilityEffect.AddReserve)
        {
            for (int i = 0; i < effectValue; i++)
            {
                PieceData piece = transformTarget;

                if (addFromRaceRandom)
                {
                    var list = pieceDatabase.GetByRace(race);

                    if (list.Count > 0)
                        piece = list[Random.Range(0, list.Count)];
                }

                ReserveManager.Instance.AddPiece(context.owner, piece);
            }

            return;
        }

        if (effect == AbilityEffect.DestroyReserve)
        {
            foreach (var owner in GetTargetOwners(context.owner))
            {
                var reserveTargets = GetReserveTargets(owner);

                reserveTargets = reserveTargets
                    .OrderBy(x => Random.value)
                    .Take(effectValue)
                    .ToList();

                foreach (var piece in reserveTargets)
                    ReserveManager.Instance.RemovePiece(owner, piece);
            }

            return;
        }

        if (effect == AbilityEffect.TransformReserve)
        {
            foreach (var owner in GetTargetOwners(context.owner))
            {
                var reserveTargets = GetReserveTargets(owner);

                reserveTargets = reserveTargets
                    .OrderBy(x => Random.value)
                    .Take(effectValue)
                    .ToList();

                foreach (var piece in reserveTargets)
                {
                    ReserveManager.Instance.RemovePiece(owner, piece);
                    ReserveManager.Instance.AddPiece(owner, transformTarget);
                }
            }

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

        targets = ApplyShapeFilter(targets, context);

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

    List<PieceData> GetReserveTargets(int owner)
    {
        List<PieceData> list = ReserveManager.Instance.GetReserve(owner);

        switch (filter)
        {
            case AbilityFilter.None:
                return list.ToList();

            case AbilityFilter.Race:
                return list.Where(p => p.race == race).ToList();

            case AbilityFilter.Attribute:
                return list.Where(p => p.attribute == attribute).ToList();

            case AbilityFilter.SpecificPiece:

                if (specificPieces == null)
                    return new List<PieceData>();

                return list.Where(p => specificPieces.Contains(p)).ToList();
        }

        return list.ToList();
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

    List<Piece> ApplyShapeFilter(List<Piece> pieces, AbilityContext context)
    {
        List<Piece> result = new List<Piece>();

        Vector2Int origin = new Vector2Int(-1, -1);

        if (context.sourcePiece != null)
            origin = BoardManager.Instance.FindPiecePosition(context.sourcePiece);

        foreach (var piece in pieces)
        {
            Vector2Int pos =
                BoardManager.Instance.FindPiecePosition(piece);

            switch (shape)
            {
                case AbilityShape.Single:
                    result.Add(piece);
                    break;

                case AbilityShape.Row:

                    if (fixedRow >= 0)
                    {
                        if (pos.y == fixedRow)
                            result.Add(piece);
                    }
                    else if (origin.y != -1)
                    {
                        if (pos.y == origin.y)
                            result.Add(piece);
                    }

                    break;

                case AbilityShape.Column:

                    if (fixedColumn >= 0)
                    {
                        if (pos.x == fixedColumn)
                            result.Add(piece);
                    }
                    else if (origin.x != -1)
                    {
                        if (pos.x == origin.x)
                            result.Add(piece);
                    }

                    break;

                case AbilityShape.Cross:

                    if (origin.x != -1)
                    {
                        if (pos.x == origin.x || pos.y == origin.y)
                            result.Add(piece);
                    }

                    break;

                case AbilityShape.Area:

                    if (origin.x != -1)
                    {
                        if (Mathf.Abs(pos.x - origin.x) <= areaRadius &&
                            Mathf.Abs(pos.y - origin.y) <= areaRadius)
                            result.Add(piece);
                    }

                    break;
            }
        }

        return result;
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