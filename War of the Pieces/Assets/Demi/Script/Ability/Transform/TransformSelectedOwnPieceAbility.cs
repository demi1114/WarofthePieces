using UnityEngine;

//©•ª‚Ì‹î‚ğw’è‚µ‚Ä•Ïg
[CreateAssetMenu(menuName = "Ability/Transform Selected Own Piece")]
public class TransformSelectedOwnPieceAbility : Ability
{
    public PieceData transformTo;

    public override void OnCardUse(AbilityContext context)
    {
        if (!context.hasTargetPosition) return;

        Piece target = BoardManager.Instance.GetPieceAt(context.targetPosition);
        if (target == null) return;

        if (target.owner != context.owner) return;

        BoardManager.Instance.ReplacePiece(target, transformTo);
    }
}