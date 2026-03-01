using UnityEngine;
using System.Collections.Generic;

//åªç›ÅAã@î\ÇµÇƒÇ¢Ç»Ç¢
[CreateAssetMenu(menuName = "Ability/Add Reserve From Board By Type")]
public class AddReserveFromBoardByTypeAbility : Ability
{
    public PieceAttribute targetAttribute;
    public int amount = 1;

    [Header("Target")]
    public bool targetOpponent = false;
    public bool randomSelect = true;

    public override void OnCardUse(AbilityContext context)
    {
        int targetOwner = targetOpponent
            ? (context.owner == 0 ? 1 : 0)
            : context.owner;

        List<Piece> pieces = BoardManager.Instance.GetPiecesByOwner(targetOwner);

        List<Piece> filtered = pieces.FindAll(p => p.data.attribute == targetAttribute);

        if (filtered.Count == 0) return;

        int addCount = Mathf.Min(amount, filtered.Count);

        for (int i = 0; i < addCount; i++)
        {
            int index = randomSelect
                ? UnityEngine.Random.Range(0, filtered.Count)
                : 0;

            ReserveManager.Instance.AddPiece(context.owner, filtered[index].data);

            filtered.RemoveAt(index);
        }
    }
}