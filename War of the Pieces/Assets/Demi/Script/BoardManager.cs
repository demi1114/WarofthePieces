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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pieceGrid = new Piece[boardSize, boardSize];
        GenerateBoard();
    }

    private void Update()
    {
        HandleClick();
    }

    // 盤面生成
    private void GenerateBoard()
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

    // マス色設定（自陣・敵陣）
    private void SetupCellColor(GameObject obj, int y)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer == null) return;

        // 自陣（下1列）
        if (y == 0)
        {
            renderer.material.color = Color.blue;
        }
        // 敵陣（上1列）
        else if (y == boardSize - 1)
        {
            renderer.material.color = Color.red;
        }
        else
        {
            renderer.material.color = Color.white;
        }
    }

    // クリック処理（Raycast）
    private void HandleClick()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // ① Pieceをクリックした場合
                Piece piece = hit.collider.GetComponent<Piece>();
                if (piece != null)
                {
                    // その駒の座標を探す
                    Vector2Int pos = FindPiecePosition(piece);
                    OnCellClicked(cells[pos.x, pos.y]);
                    return;
                }

                // ② Cellをクリックした場合
                BoardCell cell = hit.collider.GetComponent<BoardCell>();
                if (cell != null)
                {
                    OnCellClicked(cell);
                }
            }
        }
    }


    // マスクリック時
    public void OnCellClicked(BoardCell cell)
    {
        Piece clickedPiece = pieceGrid[cell.x, cell.y];

        // ① 自分の駒をクリック → 選択
        if (clickedPiece != null && clickedPiece.owner == 0)
        {
            if (!movedThisTurn)
            {
                SelectPiece(clickedPiece, cell.x, cell.y);
            }
            return;
        }

        // ② 駒選択中 → 移動処理
        if (selectedPiece != null && !movedThisTurn)
        {
            TryMovePiece(cell.x, cell.y);
            return;
        }

        // ③ 空マスクリック → 選択キャンセル
        if (selectedPiece != null)
        {
            CancelSelection();
            return;
        }

        // ④ 駒の配置
        if (clickedPiece == null)
        {
            TryPlacePiece(cell);
        }
    }


    // 座標取得
    public BoardCell GetCell(int x, int y)
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

    private void SelectPiece(Piece piece, int x, int y)
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

    private void TryMovePiece(int targetX, int targetY)
    {
        // 移動先が空であること
        if (pieceGrid[targetX, targetY] != null)
        {
            Debug.Log("移動先に駒があります");
            return;
        }

        // 前に1マスだけ許可（仮）
        if (targetX == selectedPosition.x &&
            targetY == selectedPosition.y + 1)
        {
            MovePiece(targetX, targetY);
        }
        else
        {
            Debug.Log("そこには移動できません");
        }
    }

    private void MovePiece(int targetX, int targetY)
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
    }


}
