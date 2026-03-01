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
    private int extraMovesThisTurn = 0;

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

            BoardManager.Instance.ExecuteEnemyTurn();
        }

        GameUIManager.Instance?.UpdateTurn(isPlayerTurn);
        GameUIManager.Instance?.UpdateMoves(remainingMoves);
    }

    public void EndTurn()
    {
        // ここ追加：ターン終了時に一時バフをリセット
        ResetTemporaryBuffs(isPlayerTurn ? 0 : 1);

        extraMovesThisTurn = 0;
        isPlayerTurn = !isPlayerTurn;
        StartTurn();
    }

    public void OnClickEndTurnButton()
    {
        if (isPlayerTurn) EndTurn();
    }

    // 移動関連
    public bool CanMove() => isPlayerTurn && remainingMoves > 0;

    public void ConsumeMove()
    {
        if (!isPlayerTurn || remainingMoves <= 0) return;
        remainingMoves--;
        GameUIManager.Instance?.UpdateMoves(remainingMoves);
    }

    public void AddExtraMove(int amount)
    {
        extraMovesThisTurn += amount;
        remainingMoves += amount;

        GameUIManager.Instance?.UpdateMoves(remainingMoves);
    }
    private void ResetTemporaryBuffs(int owner)
    {
        var pieces = BoardManager.Instance.GetPiecesByOwner(owner);

        foreach (var piece in pieces)
            piece.ResetTemporaryPower();
    }
    public int GetRemainingMoves() => remainingMoves;
    public float GetRemainingTime() => timer;

    public void ForceEndTurn() => EndTurn();
}