using UnityEngine;
using System.Collections.Generic;

//“G‚Ìƒ‰ƒ“ƒ_ƒ€‚È‹î‚ğ•Ïg
[CreateAssetMenu(menuName = "Ability/Transform Random Enemy Pieces")]
public class TransformRandomEnemyPiecesAbility : Ability
{
    public PieceData transformTo;
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        int enemyOwner = 1 - context.owner;

        var pieces = BoardManager.Instance.GetPiecesByOwner(enemyOwner);

        if (pieces.Count == 0) return;

        int count = Mathf.Min(amount, pieces.Count);

        for (int i = 0; i < count; i++)
        {
            var randomPiece = pieces[Random.Range(0, pieces.Count)];

            BoardManager.Instance.ReplacePiece(randomPiece, transformTo);

            pieces.Remove(randomPiece); // d•¡–h~
        }
    }
}