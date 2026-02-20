using UnityEngine;

public class CardUseManager : MonoBehaviour
{
    public static CardUseManager Instance;

    private CardData pendingCard;
    private int pendingHandIndex = -1;

    private void Awake() => Instance = this;

    public void StartCardUse(CardData card, int handIndex)
    {
        pendingCard = card;
        pendingHandIndex = handIndex;
        Debug.Log($"{card.cardName} 使用待機中");
    }

    public void ResolveCard(Vector2Int targetPosition)
    {
        if (pendingCard == null) return;

        bool success = pendingCard.Resolve(targetPosition);

        DeckManager.Instance.RemoveCardFromHand(pendingHandIndex);

        pendingCard = null;
        pendingHandIndex = -1;
    }

    public void CancelCardUse()
    {
        pendingCard = null;
        pendingHandIndex = -1;
        Debug.Log("カード使用キャンセル");
    }

    public bool IsWaitingForTarget() => pendingCard != null;
}