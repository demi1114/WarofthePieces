using UnityEngine;

public class CardUseManager : MonoBehaviour
{
    public static CardUseManager Instance;

    private CardData pendingCard;
    private int pendingHandIndex = -1;

    private void Awake()
    {
        Instance = this;
    }

    public void StartCardUse(CardData card, int handIndex)
    {
        // すでに待機中でも上書きする
        pendingCard = card;
        pendingHandIndex = handIndex;

        Debug.Log($"{card.cardName} 使用待機中（上書き可）");
    }

    public void ResolveCard(Vector2Int targetPosition)
    {
        if (pendingCard == null)
            return;

        Debug.Log($"{pendingCard.cardName} を {targetPosition} に使用");

        Debug.Log("解決"); // 効果発動（今は仮）
        bool success = pendingCard.Resolve(targetPosition);

        DeckManager.Instance.RemoveCardFromHand(pendingHandIndex); // ★ 手札から削除

        pendingCard = null; // 状態リセット
        pendingHandIndex = -1;
    }

    public void CancelCardUse()
    {
        Debug.Log("カード使用キャンセル");
        ClearPending();
    }

    private void ClearPending()
    {
        pendingCard = null;
        pendingHandIndex = -1;
    }

    public bool IsWaitingForTarget()
    {
        return pendingCard != null;
    }
}
