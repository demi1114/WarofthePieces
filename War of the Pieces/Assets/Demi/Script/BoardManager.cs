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

    // ===============================
    // 盤面生成
    // ===============================
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

    // ===============================
    // マス色設定（自陣・敵陣）
    // ===============================
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

    // ===============================
    // クリック処理（Raycast）
    // ===============================
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
                BoardCell cell = hit.collider.GetComponent<BoardCell>();

                if (cell != null)
                {
                    OnCellClicked(cell);
                }
            }
        }
    }

    // ===============================
    // マスクリック時
    // ===============================
    public void OnCellClicked(BoardCell cell)
    {
        TryPlacePiece(cell);
    }

    // ===============================
    // 座標取得
    // ===============================
    public BoardCell GetCell(int x, int y)
    {
        if (x < 0 || x >= boardSize || y < 0 || y >= boardSize)
            return null;

        return cells[x, y];
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

}
