using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [Header("Turn Settings")]
    public float turnTime = 60f;
    public int baseMoveCount = 1;   // 基本移動回数

    private float timer;
    public bool isPlayerTurn = true;

    private int remainingMoves;
    private void Awake() // 初期化
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
    public void StartTurn() // ターン制御
    {
        timer = turnTime;

        remainingMoves = baseMoveCount; // 移動回数リセット

        if (isPlayerTurn)
        {
            DeckManager.Instance.DrawCard();
        }

        Debug.Log(isPlayerTurn ? "プレイヤーターン開始" : "敵ターン開始");
        Debug.Log("残り移動回数: " + remainingMoves);
    }

    public void EndTurn()
    {
        Debug.Log("ターン終了");

        isPlayerTurn = !isPlayerTurn;

        StartTurn();
    }

    public float GetRemainingTime()
    {
        return timer;
    }

    // 移動関連
    public bool CanMove() // 移動可能か？
    {
        return remainingMoves > 0;
    }

    public void ConsumeMove() // 移動を消費
    {
        if (remainingMoves <= 0)
            return;

        remainingMoves--;

        Debug.Log("移動消費 残り: " + remainingMoves);
    }

    public void AddExtraMove(int amount) // 追加移動（カード用）
    {
        remainingMoves += amount;
        Debug.Log("追加移動 +" + amount + " 現在: " + remainingMoves);
    }

    public int GetRemainingMoves() // 残り移動取得（UI用）
    {
        return remainingMoves;
    }

    public void ForceEndTurn() // ターン強制終了（UIボタン用に将来使える）
    {
        EndTurn();
    }
}
