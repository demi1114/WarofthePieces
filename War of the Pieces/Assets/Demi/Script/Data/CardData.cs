using System.Collections.Generic;
using UnityEngine;

public enum CardCategory
{
    AddMove,
    AddReserve,
    Bounce,
    BuffPiece,
    Draw,
    DebuffPiece,
    LostDeck,
    LostPiece,
    LostReserve,
    Transform
}

[CreateAssetMenu(menuName = "Card/Create Card")]
public class CardData : ScriptableObject
{
    public string cardID;
    public string cardName;

    [Header("Description")]
    [TextArea(3, 6)]
    public string description;

    [Header("Abilities")]
    public List<Ability> abilities = new List<Ability>();

    public CardCategory category;

    public virtual bool Resolve(int owner, Vector2Int targetPos)
    {
        AbilityContext context = new AbilityContext
        {
            owner = owner,
            hasTargetPosition = true,
            targetPosition = targetPos,
            sourceCard = this
        };

        foreach (var ability in abilities)
        {
            ability.OnCardUse(context);
        }
        return true;
    }

}
