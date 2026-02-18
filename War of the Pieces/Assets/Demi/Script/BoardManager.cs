using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [Header("Board Settings")]
    public int boardSize = 9;
    public float cellSize = 1.0f;
    public GameObject cellPrefab;
    private BoardCell[,] cells;
    private List<BoardCell> highlightedCells = new List<BoardCell>();

    public GameObject piecePrefab;
    private Piece[,] pieceGrid;
    private int playerHandPieces = 8;

    private Piece selectedPiece;
    private Vector2Int selectedPosition;

    private int CountPieces(int owner) //盤上の駒数カウント関数
    {
        int count = 0;

        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                if (pieceGrid[x, y] != null &&
                    pieceGrid[x, y].owner == owner)
                {
                    count++;
                }
            }
        }

        return count;
    }
    private void CheckVictory() //勝利チェック関数
    {
        // ① 敵駒が盤上にいない
        if (CountPieces(1) == 0)
        {
            Debug.Log("プレイヤー勝利！（敵全滅）");
            return;
        }

        // ② 自分の駒が敵陣に到達
        for (int x = 0; x < boardSize; x++)
        {
            if (pieceGrid[x, boardSize - 1] != null &&
                pieceGrid[x, boardSize - 1].owner == 0)
            {
                Debug.Log("プレイヤー勝利！（敵陣到達）");
                return;
            }
        }
    }
    private void UpdatePieceCountUI() //駒数UI関数
    {
        if (GameUIManager.Instance == null)
            return;

        int playerCount = CountPieces(0);
        int enemyCount = CountPieces(1);

        GameUIManager.Instance.UpdatePieceCounts(playerCount, enemyCount);
    }
    private int GetTypeBonus(Piece attacker, Piece defender) //相性関数
    {
        // 赤 > 緑
        if (attacker.type == PieceType.Red && defender.type == PieceType.Green)
            return 1;

        // 緑 > 青
        if (attacker.type == PieceType.Green && defender.type == PieceType.Blue)
            return 1;

        // 青 > 赤
        if (attacker.type == PieceType.Blue && defender.type == PieceType.Red)
            return 1;

        return 0;
    }
    private void HighlightMovableCells() //ハイライト関数
    {
        List<Vector2Int> movable =
    selectedPiece.GetMovablePositions(selectedPosition, boardSize);

        foreach (var pos in movable)
        {
            BoardCell targetCell = cells[pos.x, pos.y];
            Piece targetPiece = pieceGrid[pos.x, pos.y];

            Renderer renderer = targetCell.GetComponent<Renderer>();

            if (targetPiece == null)
                renderer.material.color = Color.cyan;
            else if (targetPiece.owner != selectedPiece.owner)
            {
                bool willWin = PredictBattle(selectedPiece, targetPiece);
                renderer.material.color = willWin ? Color.green : Color.red;
            }

            highlightedCells.Add(targetCell);
        }

    }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        pieceGrid = new Piece[boardSize, boardSize];
        GenerateBoard();
        SpawnEnemyTestPiece();//仮置きの敵配置
        UpdatePieceCountUI();
    }
    private void Update()
    {
        HandleClick();
    }
    private void GenerateBoard() // 盤面生成
    {
        cells = new BoardCell[boardSize, boardSize];

        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                Vector3 spawnPos = new Vector3(x * cellSize, 0, y * cellSize);

                Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);

                GameObject obj = Instantiate(cellPrefab, spawnPos, rotation, transform);

                BoardCell cell = obj.GetComponent<BoardCell>();
                cell.Init(x, y);

                cells[x, y] = cell;

                SetupCellColor(obj, y);
            }
        }
    }
    private void SetupCellColor(GameObject obj, int y) // マス色設定（自陣・敵陣）
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer == null) return;

        if (y == 0)// 自陣（下1列）
        {
            renderer.material.color = Color.blue;
        }
        
        else if (y == boardSize - 1)// 敵陣（上1列）
        {
            renderer.material.color = Color.red;
        }
        else
        {
            renderer.material.color = Color.white;
        }
    }
    private void HandleClick() // クリック処理（Raycast）
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Piece piece = hit.collider.GetComponent<Piece>();// ① Pieceをクリックした場合
                if (piece != null)
                {
                    Vector2Int pos = FindPiecePosition(piece);// その駒の座標を探す
                    OnCellClicked(cells[pos.x, pos.y]);
                    return;
                }

                BoardCell cell = hit.collider.GetComponent<BoardCell>();// ② Cellをクリックした場合
                if (cell != null)
                {
                    OnCellClicked(cell);
                }

                if (CardUseManager.Instance != null && CardUseManager.Instance.IsWaitingForTarget())
                {
                    CardUseManager.Instance.CancelCardUse();
                }
            }
            else
            {
                // ★ 何にも当たらなかった場合もキャンセル
                if (CardUseManager.Instance != null &&
                    CardUseManager.Instance.IsWaitingForTarget())
                {
                    CardUseManager.Instance.CancelCardUse();
                }
            }
        }
    }
    public void OnCellClicked(BoardCell cell) // マスクリック時
    {
        if (!TurnManager.Instance.isPlayerTurn)
        {
            return;
        }

        if (CardUseManager.Instance != null &&
            CardUseManager.Instance.IsWaitingForTarget()) // カード使用待機中なら優先処理
        {
            Vector2Int gridPosition = new Vector2Int(cell.x, cell.y);
            CardUseManager.Instance.ResolveCard(gridPosition);
            return;
        }
        Piece clickedPiece = pieceGrid[cell.x, cell.y];

        if (clickedPiece != null && clickedPiece.owner == 0) //➀駒の選択
        {
            SelectPiece(clickedPiece, cell.x, cell.y);
            return;
        }

        if (selectedPiece != null) //➁駒の移動
        {
            Piece targetPiece = pieceGrid[cell.x, cell.y];

            if (targetPiece != null && targetPiece.owner != 0)
            {
                ShowBattlePrediction(selectedPiece, targetPiece);
            }

            TryMovePiece(cell.x, cell.y);
            return;
        }

        if (selectedPiece != null) // ③ 空マスクリック → 選択キャンセル
        {
            CancelSelection();
            return;
        }

        if (clickedPiece == null) // ④ 駒の配置
        {
            TryPlacePiece(cell);
        }
    }
    public BoardCell GetCell(int x, int y) // 座標取得
    {
        if (x < 0 || x >= boardSize || y < 0 || y >= boardSize)
            return null;

        return cells[x, y];
    }

    private Vector2Int FindPiecePosition(Piece piece)
    {
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                if (pieceGrid[x, y] == piece)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    private void TryPlacePiece(BoardCell cell)
    {
        // 自陣判定（下1列）
        if (cell.y != 0)
        {
            Debug.Log("自陣ではありません");
            return;
        }

        // 空きマス判定
        if (pieceGrid[cell.x, cell.y] != null)
        {
            Debug.Log("すでに駒があります");
            return;
        }

        // 手持ち確認
        if (playerHandPieces <= 0)
        {
            Debug.Log("手持ち駒がありません");
            return;
        }

        // 駒生成
        Vector3 spawnPos = cell.transform.position + Vector3.up * 0.5f;

        GameObject obj = Instantiate(piecePrefab, spawnPos, Quaternion.identity);

        Piece piece = obj.GetComponent<Piece>();
        piece.owner = 0;
        piece.type = (PieceType)Random.Range(0, 3);
        piece.ApplyColor();
        pieceGrid[cell.x, cell.y] = piece;

        playerHandPieces--;
        UpdatePieceCountUI();
    }
    private void SelectPiece(Piece piece, int x, int y) //駒の選択
    {
        if (selectedPiece == piece)
        {
            CancelSelection();
            return;
        }

        selectedPiece = piece;
        selectedPosition = new Vector2Int(x, y);
        HighlightMovableCells();

        Debug.Log($"駒選択: {x},{y}");
    }
    private void CancelSelection()//駒選択の解除
    {
        Debug.Log("選択キャンセル");

        ClearHighlights();
        selectedPiece = null;
    }
    private void TryMovePiece(int targetX, int targetY)
    {
        if (!TurnManager.Instance.CanMove())
            return;

        List<Vector2Int> movable =
            selectedPiece.GetMovablePositions(selectedPosition, boardSize);

        Vector2Int target = new Vector2Int(targetX, targetY);

        if (!movable.Contains(target))
        {
            Debug.Log("そこには移動できません");
            return;
        }

        Piece targetPiece = pieceGrid[targetX, targetY];

        if (targetPiece != null && targetPiece.owner != selectedPiece.owner)
        {
            Battle(targetX, targetY, targetPiece);
        }
        else if (targetPiece == null)
        {
            MovePiece(targetX, targetY);
            TurnManager.Instance.ConsumeMove();
        }
    }

    private void MovePiece(int targetX, int targetY) //駒の移動
    {
        // データ更新
        pieceGrid[targetX, targetY] = selectedPiece;
        pieceGrid[selectedPosition.x, selectedPosition.y] = null;

        // 見た目更新
        Vector3 newPos = cells[targetX, targetY].transform.position + Vector3.up * 0.5f;
        selectedPiece.transform.position = newPos;

        Debug.Log("移動完了（このターンはもう移動できません）");

        selectedPiece = null;
        CheckVictory();
        UpdatePieceCountUI();
    }
    private void Battle(int targetX, int targetY, Piece enemyPiece) //戦闘
    {
        int playerCount = CountPieces(0);
        int enemyCount = CountPieces(1);

        int typeBonus = GetTypeBonus(selectedPiece, enemyPiece); // ★ タイプ補正取得

        int finalPlayerPower = playerCount + typeBonus;
        int finalEnemyPower = enemyCount;

        bool playerWins;

        if (finalPlayerPower > finalEnemyPower)
        {
            playerWins = true;
        }
        else if (finalPlayerPower < finalEnemyPower)
        {
            playerWins = false;
        }
        else
        {
            playerWins = true; // 同数なら攻撃側勝利
        }

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowBattleResult(
                playerCount,
                typeBonus,
                enemyCount,
                playerWins
            );
        }

        if (playerWins)
        {
            Destroy(enemyPiece.gameObject);
            pieceGrid[targetX, targetY] = null;

            MovePiece(targetX, targetY);
            TurnManager.Instance.ConsumeMove();

            Debug.Log("プレイヤー勝利");
        }
        else
        {
            Destroy(selectedPiece.gameObject);
            pieceGrid[selectedPosition.x, selectedPosition.y] = null;

            Debug.Log("プレイヤー敗北");

            selectedPiece = null;
        }

        CheckVictory();
        UpdatePieceCountUI();
    }
    private void SpawnEnemyTestPiece()
    {
        int x = 4;
        int y = 1;

        Vector3 pos = cells[x, y].transform.position + Vector3.up * 0.5f;
        GameObject obj = Instantiate(piecePrefab, pos, Quaternion.identity);

        Piece piece = obj.GetComponent<Piece>();
        piece.owner = 1;
        piece.type = PieceType.Green;
        piece.ApplyColor();
        pieceGrid[x, y] = piece;
    }
    private void ShowBattlePrediction(Piece attacker, Piece defender)
    {
        int playerCount = CountPieces(0);
        int enemyCount = CountPieces(1);

        int typeBonus = GetTypeBonus(attacker, defender);

        int finalPlayerPower = playerCount + typeBonus;
        int finalEnemyPower = enemyCount;

        bool willWin = finalPlayerPower > finalEnemyPower;
        bool isDraw = finalPlayerPower == finalEnemyPower;

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowPrediction(willWin, isDraw);
        }
    }
    private bool PredictBattle(Piece attacker, Piece defender)
    {
        int playerCount = CountPieces(0);
        int enemyCount = CountPieces(1);

        int typeBonus = GetTypeBonus(attacker, defender);

        return (playerCount + typeBonus) >= enemyCount;
    }
    private void ClearHighlights()
    {
        foreach (BoardCell cell in highlightedCells)
        {
            SetupCellColor(cell.gameObject, cell.y);
        }

        highlightedCells.Clear();
    }
}
