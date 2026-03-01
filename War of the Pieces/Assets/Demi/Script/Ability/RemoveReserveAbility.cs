using UnityEngine;

//åªç›ÅAã@î\ÇµÇƒÇ¢Ç»Ç¢
[CreateAssetMenu(menuName = "Ability/Remove Reserve Piece")]
public class RemoveReserveAbility : Ability
{
    public int amount = 1;
    public bool isRandom = true;
    public bool targetOpponent = false;

    public override void OnCardUse(AbilityContext context)
    {
        int targetOwner = targetOpponent
            ? (context.owner == 0 ? 1 : 0)
            : context.owner;

        for (int i = 0; i < amount; i++)
        {
            if (ReserveManager.Instance.GetReserveCount(targetOwner) == 0)
                return;

            if (isRandom)
                ReserveManager.Instance.RemoveRandomPiece(targetOwner);
            else
                ReserveManager.Instance.RemovePiece(targetOwner, 0);
        }
    }
}