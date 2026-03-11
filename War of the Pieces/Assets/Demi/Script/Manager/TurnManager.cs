using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [Header("Turn Settings")]
    public float turnTime = 60f;
    public int baseMoveCount = 1;

    [Header("UI")]
    public Button endTurnButton;

    private float timer;
    public bool isPlayerTurn = true;
    private int remainingMoves;

    private void Awake() => Instance = this;

    private void Start() => StartTurn();

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) EndTurn();
    }

    public void StartTurn()
    {
        timer = turnTime;
        remainingMoves = baseMoveCount;

        if (isPlayerTurn)
        {
            // プレイヤーターン処理
            DeckManager.Instance.DrawCard(0);

            // ボタン制御
            if (endTurnButton != null)
                endTurnButton.interactable = true;
        }
        else
        {
            // 敵ターン開始
            if (endTurnButton != null)
                endTurnButton.interactable = false;

            EnemyTurnController.Instance.ExecuteTurn();
        }

        TriggerPieceAbilities(PieceAbilityTrigger.OnTurnStart);
    }

    public void EndTurn()
    {
        TriggerPieceAbilities(PieceAbilityTrigger.OnTurnEnd);
        BoardManager.Instance.CancelSelection();
        // ここ追加：ターン終了時に一時バフをリセット
        ResetTemporaryBuffs(isPlayerTurn ? 0 : 1);

        isPlayerTurn = !isPlayerTurn;
        StartTurn();
    }


    // 移動関連
    public bool CanMove(int owner)
    {
        if (owner != GetCurrentTurnOwner())
            return false;

        return remainingMoves > 0;
    }

    public void ConsumeMove()
    {
        if (remainingMoves <= 0) return;

        remainingMoves--;
    }

    public int GetCurrentTurnOwner()
    {
        return isPlayerTurn ? 0 : 1;
    }
    public void AddExtraMove(int owner, int amount)
    {
        if (owner != GetCurrentTurnOwner()) return;
        if (amount <= 0) return;

        remainingMoves += amount;
    }
    private void ResetTemporaryBuffs(int owner)
    {
        var pieces = BoardManager.Instance.GetPiecesByOwner(owner);

        foreach (var piece in pieces)
            piece.ResetTemporaryPower();
    }
    public int GetRemainingMoves()
    {
        return remainingMoves;
    }

    public void TriggerPieceAbilities(PieceAbilityTrigger timing)
    {
        int owner = GetCurrentTurnOwner();

        var pieces = BoardManager.Instance.GetPiecesByOwner(owner);

        foreach (var piece in pieces)
        {
            piece.TriggerAbilities(timing);
        }
    }
}