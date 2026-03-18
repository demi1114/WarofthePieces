using UnityEngine;

public class CardUseManager : MonoBehaviour
{
    public static CardUseManager Instance;

    private CardData pendingCard;
    private int pendingHandIndex = -1;
    private int pendingOwner = -1;
    bool waitingForTarget = false;

    private void Awake() => Instance = this;

    public void StartCardUse(CardData card, int handIndex, int owner)
    {
        pendingCard = card;
        pendingHandIndex = handIndex;
        pendingOwner = owner;

        waitingForTarget = false;

        foreach (var ability in card.abilities)
        {
            if (ability.targetType == AbilityTargetType.Select)
            {
                waitingForTarget = true;
                break;
            }
        }

        waitingForTarget = true;

        Debug.Log("盤面クリック待ち");
    }

    public void ResolveCard(Vector2Int targetPosition)
    {
        if (pendingCard == null) return;

        Piece targetPiece =
            BoardManager.Instance.GetPieceAt(targetPosition);

        // 🔥 Select能力が1つでもあるかチェック
        bool hasSelect = false;

        foreach (var ab in pendingCard.abilities)
        {
            if (ab.targetType == AbilityTargetType.Select)
            {
                hasSelect = true;
                break;
            }
        }

        // 🔥 Selectカードのチェック
        if (hasSelect)
        {
            if (targetPiece == null)
            {
                Debug.Log("対象がいないためキャンセル");
                return;
            }

            bool valid = false;

            foreach (var ab in pendingCard.abilities)
            {
                // 🔥 Selectだけチェック
                if (ab.targetType != AbilityTargetType.Select)
                    continue;

                if (ab.IsValidTarget(targetPiece, pendingOwner))
                {
                    valid = true;
                    break;
                }
            }

            if (!valid)
            {
                Debug.Log("対象条件不一致");
                return;
            }
        }

        // ===== 実行 =====
        AbilityContext context = new AbilityContext
        {
            owner = pendingOwner,
            sourceCard = pendingCard,
            hasTargetPosition = true,
            targetPosition = targetPosition,
            targetPiece = targetPiece
        };

        foreach (var ab in pendingCard.abilities)
        {
            if (ab.targetType == AbilityTargetType.Select)
            {
                // 無効ならスキップ
                if (!ab.IsValidTarget(targetPiece, pendingOwner))
                    continue;
            }

            ab.OnCardUse(context);
        }

        DeckManager.Instance.RemoveCardFromHand(
            pendingOwner,
            pendingHandIndex);

        pendingCard = null;
        waitingForTarget = false;
    }

    public void CancelCardUse()
    {
        pendingCard = null;
        pendingHandIndex = -1;
        pendingOwner = -1;

        waitingForTarget = false;

        Debug.Log("カード使用キャンセル");
    }

    public bool IsWaitingForTarget()
    {
        return waitingForTarget;
    }
}