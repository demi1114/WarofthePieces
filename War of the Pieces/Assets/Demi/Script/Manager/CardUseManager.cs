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

        Debug.Log("カード使用待機中（盤面クリック待ち）");

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

        bool success = true;

        foreach (var ab in pendingCard.abilities)
        {
            // Selectチェック
            if (ab.targetType == AbilityTargetType.Select)
            {
                if (targetPiece == null ||
                    !ab.IsValidTarget(targetPiece, pendingOwner))
                {
                    success = false;
                    break;
                }
            }

            // 実行
            bool result = ab.OnCardUse(context);

            if (!result)
            {
                success = false;
                break;
            }
        }

        if (success)
        {
            DeckManager.Instance.RemoveCardFromHand(
                pendingOwner,
                pendingHandIndex);
        }
        else
        {
            Debug.Log("カード発動失敗 → 消費しない");
        }

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