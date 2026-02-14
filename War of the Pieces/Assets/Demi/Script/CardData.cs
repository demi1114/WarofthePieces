using UnityEngine;

public enum CardType
{
    Draw,
    Move,
    Spawn
}

[CreateAssetMenu(menuName = "Card/Create Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardType cardType;

    public virtual bool Resolve(Vector2Int targetPos)
    {
        Debug.Log($"{cardName} ‰ğŒˆi‰¼j");
        return true;
    }
}
