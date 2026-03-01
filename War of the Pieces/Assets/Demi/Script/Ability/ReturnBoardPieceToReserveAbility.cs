using UnityEngine;
using System.Collections.Generic;

//åªç›ÅAã@î\ÇµÇƒÇ¢Ç»Ç¢
[CreateAssetMenu(menuName = "Ability/Return Board To Reserve")]
public class ReturnBoardPieceToReserveAbility : Ability
{
    public int amount = 1;

    [Header("Target Settings")]
    public bool targetOpponent = false;
    public bool randomTarget = true;

    [Header("Filter")]
    public bool useFilter = false;
    public PieceAttribute filterAttribute;

    public override void OnCardUse(AbilityContext context)
    {
        int targetOwner = targetOpponent
            ? (context.owner == 0 ? 1 : 0)
            : context.owner;

        List<Piece> pieces = BoardManager.Instance.GetPiecesByOwner(targetOwner);

        if (useFilter)
            pieces = pieces.FindAll(p => p.data.attribute == filterAttribute);

        if (pieces.Count == 0) return;

        int returnCount = Mathf.Min(amount, pieces.Count);

        for (int i = 0; i < returnCount; i++)
        {
            int index = randomTarget
                ? UnityEngine.Random.Range(0, pieces.Count)
                : 0;

            Piece target = pieces[index];

            BoardManager.Instance.ReturnPieceToReserve(target);

            pieces.RemoveAt(index); // èdï°ñhé~
        }
    }
}