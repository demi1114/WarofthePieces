using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnController : MonoBehaviour
{
    public static EnemyTurnController Instance;

    [Header("Delay Settings")]
    public float actionDelay = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    // 外部から呼ぶ入口
    public void ExecuteTurn()
    {
        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        Debug.Log("敵ターン開始");

        yield return new WaitForSeconds(actionDelay);

        // ① ドロー
        DeckManager.Instance.DrawCard(1);
        yield return new WaitForSeconds(actionDelay);

        // ② 駒配置（2回試行）
        for (int i = 0; i < 2; i++)
        {
            bool placed = TryPlaceRandomEnemyPiece();
            if (!placed) break;

            yield return new WaitForSeconds(actionDelay);
        }

        // ③ 移動（残り移動回数分）
        int moveCount = TurnManager.Instance.GetRemainingMoves();

        for (int i = 0; i < moveCount; i++)
        {
            bool moved = TryRandomMoveEnemyPiece();
            if (!moved) break;

            yield return new WaitForSeconds(actionDelay);
        }

        // ④ カード使用
        CardData card = DeckManager.Instance.GetRandomCardFromHand(1);

        if (card != null)
        {
            Debug.Log("敵カード使用: " + card.cardName);

            CardUseManager.Instance.StartCardUse(card, -1, 1);
            Vector2Int randomPos = new Vector2Int
                (Random.Range(0, BoardManager.Instance.boardSize),
                 Random.Range(0, BoardManager.Instance.boardSize));

            CardUseManager.Instance.ResolveCard(randomPos);

            yield return new WaitForSeconds(actionDelay);
        }

        // ⑤ ターン終了
        TurnManager.Instance.EndTurn();
        Debug.Log("敵ターン終了");
    }

    // 駒配置
    private bool TryPlaceRandomEnemyPiece()
    {
        var reserve = ReserveManager.Instance.GetReserve(1);
        if (reserve.Count == 0) return false;

        int y = BoardManager.Instance.boardSize - 1;

        List<int> emptyX = new List<int>();

        for (int x = 0; x < BoardManager.Instance.boardSize; x++)
        {
            if (BoardManager.Instance.GetPieceAt(new Vector2Int(x, y)) == null)
                emptyX.Add(x);
        }

        if (emptyX.Count == 0) return false;

        int pieceIndex = Random.Range(0, reserve.Count);
        PieceData data = reserve[pieceIndex];

        int chosenX = emptyX[Random.Range(0, emptyX.Count)];

        BoardManager.Instance.SpawnPieceOnBoard(
            data,
            1,
            new Vector2Int(chosenX, y)
        );

        ReserveManager.Instance.RemovePiece(1, pieceIndex);

        Debug.Log("敵駒配置");

        return true;
    }

    // ランダム移動
    private bool TryRandomMoveEnemyPiece()
    {
        List<Piece> enemyPieces =
            BoardManager.Instance.GetPiecesByOwner(1);

        if (enemyPieces.Count == 0) return false;

        Piece piece = enemyPieces[Random.Range(0, enemyPieces.Count)];
        Vector2Int from =
            BoardManager.Instance.FindPiecePosition(piece);

        var movable =
            piece.GetMovablePositions(from,
            BoardManager.Instance.boardSize);

        List<Vector2Int> validMoves = new List<Vector2Int>();

        foreach (var move in movable)
        {
            Piece target =
                BoardManager.Instance.GetPieceAt(move);

            if (target == null || target.owner != 1)
                validMoves.Add(move);
        }

        if (validMoves.Count == 0) return false;

        Vector2Int to =
            validMoves[Random.Range(0, validMoves.Count)];

        BoardManager.Instance.TryMovePiece(piece, from, to);

        return true;
    }

}