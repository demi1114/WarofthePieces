using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    public const int SIZE = 9;

    [Header("Cell Settings")]
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private float cellSize = 1.1f;

    // 見た目管理
    private BoardCell[,] cells = new BoardCell[SIZE, SIZE];

    // 駒管理（ゲームロジック用）
    private Piece[,] board = new Piece[SIZE, SIZE];

    #region Unity

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        GenerateBoard();
    }

    #endregion

    #region Board Generation

    private void GenerateBoard()
    {
        for (int y = 0; y < SIZE; y++)
        {
            for (int x = 0; x < SIZE; x++)
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

    private void SetupCellColor(GameObject cellObj, int y)
    {
        Renderer renderer = cellObj.GetComponent<Renderer>();
        if (renderer == null) return;

        if (y == 0)
            renderer.material.color = Color.blue;      // 自陣
        else if (y == SIZE - 1)
            renderer.material.color = Color.red;       // 敵陣
        else
            renderer.material.color = Color.white;     // 通常
    }

    #endregion

    #region Territory Check

    public bool IsBottomTerritory(Vector2Int pos)
    {
        return pos.y == 0;
    }

    public bool IsTopTerritory(Vector2Int pos)
    {
        return pos.y == SIZE - 1;
    }

    #endregion

    #region Board Data Access

    public bool IsInsideBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < SIZE &&
               pos.y >= 0 && pos.y < SIZE;
    }

    public bool IsEmpty(Vector2Int pos)
    {
        if (!IsInsideBoard(pos)) return false;
        return board[pos.x, pos.y] == null;
    }

    public Piece GetPiece(Vector2Int pos)
    {
        if (!IsInsideBoard(pos)) return null;
        return board[pos.x, pos.y];
    }

    public void SetPiece(Vector2Int pos, Piece piece)
    {
        if (!IsInsideBoard(pos)) return;

        board[pos.x, pos.y] = piece;
    }

    public void RemovePiece(Vector2Int pos)
    {
        if (!IsInsideBoard(pos)) return;

        board[pos.x, pos.y] = null;
    }

    #endregion

    #region Utility

    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellSize, 0, gridPos.y * cellSize);
    }

    #endregion
}
