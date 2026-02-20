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

        if (isPlayerTurn) DeckManager.Instance.DrawCard();

        if (endTurnButton != null) endTurnButton.interactable = isPlayerTurn;

        GameUIManager.Instance?.UpdateTurn(isPlayerTurn);
        GameUIManager.Instance?.UpdateMoves(remainingMoves);
    }

    public void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        StartTurn();
    }

    public void OnClickEndTurnButton()
    {
        if (isPlayerTurn) EndTurn();
    }

    // ˆÚ“®ŠÖ˜A
    public bool CanMove() => isPlayerTurn && remainingMoves > 0;

    public void ConsumeMove()
    {
        if (!isPlayerTurn || remainingMoves <= 0) return;
        remainingMoves--;
        GameUIManager.Instance?.UpdateMoves(remainingMoves);
    }

    public void AddExtraMove(int amount)
    {
        if (!isPlayerTurn) return;
        remainingMoves += amount;
        GameUIManager.Instance?.UpdateMoves(remainingMoves);
    }

    public int GetRemainingMoves() => remainingMoves;
    public float GetRemainingTime() => timer;

    public void ForceEndTurn() => EndTurn();
}