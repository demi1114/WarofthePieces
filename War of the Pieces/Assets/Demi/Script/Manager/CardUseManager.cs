using UnityEngine;

public class CardUseManager : MonoBehaviour
{
    public static CardUseManager Instance;

    private CardData pendingCard;
    private int pendingHandIndex = -1;
    private int pendingOwner = -1;   // ← 追加

    private void Awake() => Instance = this;

    public void StartCardUse(CardData card, int handIndex, int owner)
    {
        pendingCard = card;
        pendingHandIndex = handIndex;
        pendingOwner = owner;   // ← 保存

        Debug.Log($"{card.cardName} 使用待機中 (owner:{owner})");
    }

    public void ResolveCard(Vector2Int targetPosition)
    {
        if (pendingCard == null) return;

        bool success = pendingCard.Resolve(pendingOwner, targetPosition); // ← 修正

        if (success)
            DeckManager.Instance.RemoveCardFromHand(pendingOwner, pendingHandIndex);

        pendingCard = null;
        pendingHandIndex = -1;
        pendingOwner = -1;
    }

    public void CancelCardUse()
    {
        pendingCard = null;
        pendingHandIndex = -1;
        pendingOwner = -1;

        Debug.Log("カード使用キャンセル");
    }

    public bool IsWaitingForTarget() => pendingCard != null;
}