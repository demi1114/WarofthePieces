using UnityEngine;
using System.Collections.Generic;

//ƒ‰ƒ“ƒ_ƒ€‚È©•ª‚Ì‹î‚ğ•Ïg
[CreateAssetMenu(menuName = "Ability/Transform Random Own Pieces")]
public class TransformRandomOwnPiecesAbility : Ability
{
    public List<PieceData> possibleTransforms;
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        var pieces = BoardManager.Instance.GetPiecesByOwner(context.owner);

        if (pieces.Count == 0 || possibleTransforms.Count == 0)
            return;

        int count = Mathf.Min(amount, pieces.Count);

        for (int i = 0; i < count; i++)
        {
            var randomPiece = pieces[Random.Range(0, pieces.Count)];
            var randomTransform = possibleTransforms[Random.Range(0, possibleTransforms.Count)];

            BoardManager.Instance.ReplacePiece(randomPiece, randomTransform);

            pieces.Remove(randomPiece); // d•¡–h~
        }
    }
}