using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [Header("Turn Settings")]
    public float turnTime = 60f;
    public int baseMoveCount = 1;

    [Header("UI")]
    public Button endTurnButton;   // ← 追加

    private float timer;
    public bool isPlayerTurn = true;

    private int remainingMoves;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartTurn();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            EndTurn();
        }
    }

    public void StartTurn()
    {
        timer = turnTime;
        remainingMoves = baseMoveCount;

        if (isPlayerTurn)
        {
            DeckManager.Instance.DrawCard();
        }

        // ★ ボタン制御
        if (endTurnButton != null)
        {
            endTurnButton.interactable = isPlayerTurn;
        }

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateTurn(isPlayerTurn);
            GameUIManager.Instance.UpdateMoves(remainingMoves);
        }
    }

    public void EndTurn()
    {
        Debug.Log("ターン終了");

        isPlayerTurn = !isPlayerTurn;

        StartTurn();
    }

    // ★ ボタン専用メソッド
    public void OnClickEndTurnButton()
    {
        if (!isPlayerTurn)
            return;

        EndTurn();
    }

    public float GetRemainingTime()
    {
        return timer;
    }

    // 移動関連
    public bool CanMove()
    {
        return isPlayerTurn && remainingMoves > 0;
    }

    public void ConsumeMove()
    {
        if (!isPlayerTurn) return;
        if (remainingMoves <= 0) return;

        remainingMoves--;

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateMoves(remainingMoves);
        }
    }

    public void AddExtraMove(int amount)
    {
        if (!isPlayerTurn) return;

        remainingMoves += amount;

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateMoves(remainingMoves);
        }
    }

    public int GetRemainingMoves()
    {
        return remainingMoves;
    }

    public void ForceEndTurn()
    {
        EndTurn();
    }
}
