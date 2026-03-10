using UnityEngine;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance;

    private int GetTotalPieceCount(int owner)
    {
        int board = BoardManager.Instance.GetBoardCount(owner);
        int reserve = ReserveManager.Instance.GetReserve(owner).Count;
        return board + reserve;
    }
    private void Awake()
    {
        Instance = this;
    }

    // ˆÚ“®E”z’uŒã‚ÉŒÄ‚Ô
    public void CheckAfterAction()
    {
        CheckAnnihilationVictory();
        CheckInvasionVictory();
        CheckDefeatByInvasion();
    }

    // Ÿr–ÅŸ—˜
    private void CheckAnnihilationVictory()
    {
        int playerTotal = GetTotalPieceCount(0);
        int enemyTotal = GetTotalPieceCount(1);

        if (enemyTotal == 0)
        {
            Debug.Log("Ÿr–ÅŸ—˜I");
            EndGame(0);
        }
        else if (playerTotal == 0)
        {
            Debug.Log("Ÿr–Å”s–k...");
            EndGame(1);
        }
    }

    // N“üŸ—˜i“GwÅ‰œ“’Bj
    private void CheckInvasionVictory()
    {
        int boardSize = BoardManager.Instance.boardSize;

        for (int x = 0; x < boardSize; x++)
        {
            Piece piece =
                BoardManager.Instance.GetPieceAt(
                    new Vector2Int(x, boardSize - 1));

            if (piece != null && piece.owner == 0)
            {
                Debug.Log("N“üŸ—˜I");
                EndGame(0);
                return;
            }
        }
    }

    // N“ü”s–ki©wÅ‰œ‚É“Gj
    private void CheckDefeatByInvasion()
    {
        int boardSize = BoardManager.Instance.boardSize;

        for (int x = 0; x < boardSize; x++)
        {
            Piece piece =
                BoardManager.Instance.GetPieceAt(
                    new Vector2Int(x, 0));

            if (piece != null && piece.owner == 1)
            {
                Debug.Log("N“ü”s–k...");
                EndGame(1);
                return;
            }
        }
    }

    private void EndGame(int winnerOwner)
    {
        Debug.Log("ƒQ[ƒ€I—¹ ŸÒ: " + winnerOwner);

        // ƒ^[ƒ“’â~
        TurnManager.Instance.enabled = false;

        // UI•\¦
        if (winnerOwner == 0)
            GameResultUI.Instance.ShowVictory();
        else
            GameResultUI.Instance.ShowDefeat();
    }
}