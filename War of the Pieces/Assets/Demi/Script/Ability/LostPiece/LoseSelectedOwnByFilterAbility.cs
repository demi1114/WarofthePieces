using UnityEngine;

//特定種族または属性の自分の駒を選んでロスト
[CreateAssetMenu(menuName = "Ability/Lose Selected Own By Filter")]
public class LoseSelectedOwnByFilterAbility : Ability
{
    public FilterType filterType;

    public PieceRace targetRace;
    public PieceAttribute targetAttribute;

    public override void OnCardUse(AbilityContext context)
    {
        if (!context.hasTargetPosition) return;

        Piece target = BoardManager.Instance.GetPieceAt(context.targetPosition);
        if (target == null) return;

        if (target.owner != context.owner) return;

        bool match = false;

        if (filterType == FilterType.Race &&
            target.data.race == targetRace)
            match = true;

        if (filterType == FilterType.Attribute &&
            target.data.attribute == targetAttribute)
            match = true;

        if (!match) return;

        BoardManager.Instance.RemovePiece(target);

        Debug.Log("条件一致の自駒をロスト");
        VictoryManager.Instance.CheckAfterAction();
    }
}