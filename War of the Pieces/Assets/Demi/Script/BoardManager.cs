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

    public GameObject piecePrefab;
    private Piece[,] pieceGrid;
    private int playerHandPieces = 8;

    private Piece selectedPiece;
    private Vector2Int selectedPosition;
    private bool movedThisTurn = false;

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
    public void ResetMoveFlag() //ターン終了フラグ
    {
        movedThisTurn = false;
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

        if (clickedPiece != null && clickedPiece.owner == 0)// ① 自分の駒をクリック → 選択
        {
            if (!movedThisTurn)
            {
                SelectPiece(clickedPiece, cell.x, cell.y);
            }
            return;
        }

        if (selectedPiece != null && !movedThisTurn) // ② 駒選択中 → 移動処理
        {
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
        piece.type = PieceType.Soldier;

        pieceGrid[cell.x, cell.y] = piece;

        playerHandPieces--;

        Debug.Log($"駒配置！残り: {playerHandPieces}");
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

        Debug.Log($"駒選択: {x},{y}");
    }
    private void CancelSelection()//駒選択の解除
    {
        Debug.Log("選択キャンセル");

        selectedPiece = null;
    }
    private void TryMovePiece(int targetX, int targetY) //駒の移動先選択
    {
        // 前に1マスだけ許可（仮）
        if (targetX == selectedPosition.x &&
            targetY == selectedPosition.y + 1)
        {
            Piece targetPiece = pieceGrid[targetX, targetY];

            if (targetPiece != null && targetPiece.owner != 0)
            {
                // 敵がいる → 戦闘
                Battle(targetX, targetY, targetPiece);
            }
            else if (targetPiece == null)
            {
                // 空マス → 通常移動
                MovePiece(targetX, targetY);
            }
        }
        else
        {
            Debug.Log("そこには移動できません");
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

        movedThisTurn = true;

        Debug.Log("移動完了（このターンはもう移動できません）");

        selectedPiece = null;
        CheckVictory();
    }
    private void Battle(int targetX, int targetY, Piece enemyPiece) //戦闘
    {
        int playerCount = CountPieces(0);
        int enemyCount = CountPieces(1);

        Debug.Log($"戦闘開始 Player:{playerCount} Enemy:{enemyCount}");

        bool playerWins = false;

        if (playerCount > enemyCount)
        {
            playerWins = true;
        }
        else if (playerCount < enemyCount)
        {
            playerWins = false;
        }
        else
        {
            // 同数 → 攻撃側勝利
            playerWins = true;
        }

        if (playerWins)
        {
            Destroy(enemyPiece.gameObject);
            pieceGrid[targetX, targetY] = null;

            MovePiece(targetX, targetY);

            Debug.Log("プレイヤー勝利");
        }
        else
        {
            Destroy(selectedPiece.gameObject);
            pieceGrid[selectedPosition.x, selectedPosition.y] = null;

            Debug.Log("プレイヤー敗北");

            selectedPiece = null;
            movedThisTurn = true;
        }

        CheckVictory();
    }
    private void SpawnEnemyTestPiece()
    {
        int x = 4;
        int y = 1;

        Vector3 pos = cells[x, y].transform.position + Vector3.up * 0.5f;
        GameObject obj = Instantiate(piecePrefab, pos, Quaternion.identity);

        Piece piece = obj.GetComponent<Piece>();
        piece.owner = 1;

        pieceGrid[x, y] = piece;
    }

}
