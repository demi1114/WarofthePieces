using UnityEngine;

public enum CardType
{
    Draw,
    AddMove,
    Spawn
}

[CreateAssetMenu(menuName = "Card/Create Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardType cardType;
    public int value = 1;   // 効果量（追加移動数など）

    public virtual bool Resolve(Vector2Int targetPos)
    {
        switch (cardType)
        {
            case CardType.Draw:
                DeckManager.Instance.DrawCard();
                break;

            case CardType.Spawn:
                Debug.Log("スポーンカード（未実装）");
                break;

            case CardType.AddMove:
                TurnManager.Instance.AddExtraMove(value);
                Debug.Log("移動回数 +1");
                break;
        }

        return true;
    }

}
