using UnityEngine;

public enum CardType
{
    Draw,
    DrawBoth,
    AddMove,
}

[CreateAssetMenu(menuName = "Card/Create Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardType cardType;
    public int value = 1;   // Œø‰Ê—Êi’Ç‰ÁˆÚ“®”‚È‚Çj

    public virtual bool Resolve(Vector2Int targetPos)
    {
        switch (cardType)
        {
            case CardType.Draw:
                for (int i = 0; i < value; i++)
                {
                    DeckManager.Instance.DrawCard();
                }
                break;

            case CardType.DrawBoth:
                for (int i = 0; i < value; i++)
                {
                    DeckManager.Instance.DrawCard();
                    EnemyDeckManager.Instance.DrawCard();
                }
                break;

            case CardType.AddMove:
                TurnManager.Instance.AddExtraMove(value);
                Debug.Log("ˆÚ“®‰ñ” +1");
                break;
        }

        return true;
    }

}
