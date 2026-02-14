using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public float turnTime = 60f;

    private float timer;
    public bool isPlayerTurn = true;

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

        if (isPlayerTurn)
        {
            DeckManager.Instance.DrawCard();
        }

        Debug.Log(isPlayerTurn ? "プレイヤーターン開始" : "敵ターン開始");

        BoardManager.Instance.ResetMoveFlag();
    }

    public void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;

        StartTurn();
    }

    public float GetRemainingTime()
    {
        return timer;
    }
}
