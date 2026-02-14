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

    public virtual void Use()
    {
        Debug.Log($"{cardName} を使用");

        switch (cardType)
        {
            case CardType.Draw:
                DeckManager.Instance.DrawCard();
                break;

            case CardType.Move:
                Debug.Log("移動カード使用（仮）");
                break;

            case CardType.Spawn:
                Debug.Log("スポーンカード使用（仮）");
                break;
        }
    }
}
