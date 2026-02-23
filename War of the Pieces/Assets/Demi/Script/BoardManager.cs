using System.Collections;
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
    public GameObject piecePrefab;

    private BoardCell[,] cells;
    private Piece[,] pieceGrid;
    private List<BoardCell> highlightedCells = new List<BoardCell>();

    [Header("Player/Enemy Settings")]
    public List<PieceData> initialPieces;   // プレイヤー初期手駒
    public List<PieceData> availablePieces; // 敵駒やランダム生成用
    public List<PieceData> playerHand = new List<PieceData>();
    public List<PieceData> enemyHand = new List<PieceData>(); // AI用手駒
    private int playerHandPieces = 8;
    private int enemyHandPieces = 8;        // 仮

    private Piece selectedPiece;
    private PieceData selectedPlacePieceData;
    private Vector2Int selectedPosition;

    private void Awake() => Instance = this;

    private void Start()
    {
        pieceGrid = new Piece[boardSize, boardSize];
        GenerateBoard();

        playerHand = new List<PieceData>(initialPieces);
        HandUIManager.Instance.RefreshHand();
        UpdatePieceCountUI();
    }

    private void Update() => HandleClick();

    // ------------------------
    // 盤面生成・セル設定
    // ------------------------
    private void GenerateBoard()
    {
        cells = new BoardCell[boardSize, boardSize];
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                Vector3 spawnPos = new Vector3(x * cellSize, 0, y * cellSize);
                GameObject obj = Instantiate(cellPrefab, spawnPos, Quaternion.Euler(90f, 0, 0), transform);
                BoardCell cell = obj.GetComponent<BoardCell>();
                cell.Init(x, y);
                cells[x, y] = cell;

                SetupCellColor(obj, y);
            }
        }
    }

    private void SetupCellColor(GameObject obj, int y)
    {
        Renderer r = obj.GetComponent<Renderer>();
        if (r == null) return;

        if (y == 0) r.material.color = Color.blue;          // 自陣
        else if (y == boardSize - 1) r.material.color = Color.red; // 敵陣
        else r.material.color = Color.white;               // 通常
    }

    // ------------------------
    // 駒操作・配置
    // ------------------------
    public void SelectPlacePiece(PieceData data)
    {
        selectedPlacePieceData = data;
        Debug.Log("配置駒選択: " + data.pieceName);
    }

    public void OnCellClicked(BoardCell cell)
    {
        if (!TurnManager.Instance.isPlayerTurn) return;

        // カード使用待機中なら優先処理
        if (CardUseManager.Instance != null && CardUseManager.Instance.IsWaitingForTarget())
        {
            CardUseManager.Instance.ResolveCard(new Vector2Int(cell.x, cell.y));
            return;
        }

        Piece clickedPiece = pieceGrid[cell.x, cell.y];

        // ① 自陣駒をクリック → 選択
        if (clickedPiece != null && clickedPiece.owner == 0)
        {
            SelectPiece(clickedPiece, cell.x, cell.y);
            return;
        }

        // ② 移動可能駒が選択中
        if (selectedPiece != null)
        {
            Piece targetPiece = pieceGrid[cell.x, cell.y];
            if (targetPiece != null && targetPiece.owner != 0)
                ShowBattlePrediction(selectedPiece, targetPiece);

            TryMovePiece(cell.x, cell.y);
            return;
        }

        // ③ 空マスクリックでキャンセル
        if (selectedPiece != null)
        {
            CancelSelection();
            return;
        }

        // ④ 駒を配置
        if (clickedPiece == null)
            TryPlacePiece(cell);
    }

    private void SelectPiece(Piece piece, int x, int y)
    {
        ClearHighlights();

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

    public Vector2Int FindPiecePosition(Piece piece)
    {
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                if (pieceGrid[x, y] == piece)
                    return new Vector2Int(x, y);
            }
        }

        return new Vector2Int(-1, -1);
    }

    private void CancelSelection()
    {
        Debug.Log("選択キャンセル");
        ClearHighlights();
        selectedPiece = null;
    }

    private void TryPlacePiece(BoardCell cell)
    {
        if (cell.y != 0) { Debug.Log("自陣ではありません"); return; }
        if (pieceGrid[cell.x, cell.y] != null) { Debug.Log("すでに駒があります"); return; }
        if (playerHandPieces <= 0) { Debug.Log("手持ち駒がありません"); return; }
        if (selectedPlacePieceData == null) { Debug.Log("駒を選択してください"); return; }

        Vector3 pos = cell.transform.position + Vector3.up * 0.5f;
        GameObject obj = Instantiate(piecePrefab, pos, Quaternion.identity);
        Piece piece = obj.GetComponent<Piece>();
        piece.Initialize(selectedPlacePieceData, 0);

        pieceGrid[cell.x, cell.y] = piece;
        playerHand.Remove(selectedPlacePieceData);
        selectedPlacePieceData = null;

        HandUIManager.Instance.RefreshHand();
        UpdatePieceCountUI();

        Debug.Log("配置完了");
    }

    // ------------------------
    // 移動処理
    // ------------------------
    private void HighlightMovableCells()
    {
        List<Vector2Int> movable = selectedPiece.GetMovablePositions(selectedPosition, boardSize);

        foreach (var pos in movable)
        {
            BoardCell targetCell = cells[pos.x, pos.y];
            if (pieceGrid[pos.x, pos.y] == null)
                targetCell.GetComponent<Renderer>().material.color = Color.cyan;

            highlightedCells.Add(targetCell);
        }
    }

    private void ClearHighlights()
    {
        foreach (BoardCell cell in highlightedCells)
            SetupCellColor(cell.gameObject, cell.y); // 元の陣地色を復元

        highlightedCells.Clear();
    }

    private void TryMovePiece(int targetX, int targetY)
    {
        if (!TurnManager.Instance.CanMove()) return;

        Vector2Int target = new Vector2Int(targetX, targetY);
        List<Vector2Int> movable = selectedPiece.GetMovablePositions(selectedPosition, boardSize);

        if (!movable.Contains(target)) { Debug.Log("そこには移動できません"); return; }

        Piece targetPiece = pieceGrid[targetX, targetY];

        // 戦闘
        if (targetPiece != null && targetPiece.owner != selectedPiece.owner)
        {
            BattleResult result = BattleManager.Instance.ResolveBattle(selectedPiece, targetPiece);

            Piece winner = result.winner;
            Piece loser = result.loser;
            Vector2Int loserPos = GetPiecePosition(loser);

            pieceGrid[loserPos.x, loserPos.y] = null;
            Destroy(loser.gameObject);
            UpdatePieceCountUI();

            if (winner == selectedPiece)
            {
                pieceGrid[targetX, targetY] = selectedPiece;
                pieceGrid[selectedPosition.x, selectedPosition.y] = null;
                selectedPiece.transform.position = cells[targetX, targetY].transform.position + Vector3.up * 0.5f;
            }
            else pieceGrid[selectedPosition.x, selectedPosition.y] = null;

            ClearHighlights();

            selectedPiece = null;
            TurnManager.Instance.ConsumeMove();

            CheckAnnihilationVictory();
            CheckInvasionVictory();
            return;
        }

        // 通常移動
        MovePiece(targetX, targetY);
        TurnManager.Instance.ConsumeMove();
    }

    private void MovePiece(int x, int y)
    {
        pieceGrid[x, y] = selectedPiece;
        pieceGrid[selectedPosition.x, selectedPosition.y] = null;
        selectedPiece.transform.position = cells[x, y].transform.position + Vector3.up * 0.5f;

        ClearHighlights();
        Debug.Log("移動完了（このターンはもう移動できません）");

        selectedPiece = null;
        CheckInvasionVictory();
        UpdatePieceCountUI();
    }

    private Vector2Int GetPiecePosition(Piece piece)
    {
        for (int y = 0; y < boardSize; y++)
            for (int x = 0; x < boardSize; x++)
                if (pieceGrid[x, y] == piece) return new Vector2Int(x, y);

        return new Vector2Int(-1, -1);
    }

    // ------------------------
    // 戦闘関連
    // ------------------------
    public int GetBoardCount(int owner)
    {
        int count = 0;
        for (int y = 0; y < boardSize; y++)
            for (int x = 0; x < boardSize; x++)
                if (pieceGrid[x, y] != null && pieceGrid[x, y].owner == owner) count++;
        return count;
    }

    private void ShowBattlePrediction(Piece attacker, Piece defender)
    {
        bool willWin = BattleManager.Instance.PredictWinner(attacker, defender);
        GameUIManager.Instance?.ShowPrediction(willWin, false);
    }

    private bool CheckAnnihilationVictory()
    {
        if (CountPieces(1) <= 0 && enemyHandPieces <= 0)
        {
            Debug.Log("プレイヤー勝利！（敵全滅）");
            return true;
        }
        return false;
    }

    private bool CheckInvasionVictory()
    {
        for (int x = 0; x < boardSize; x++)
            if (pieceGrid[x, boardSize - 1]?.owner == 0)
            {
                Debug.Log("プレイヤー勝利！（敵陣到達）");
                return true;
            }

        return false;
    }

    private void UpdatePieceCountUI()
    {
        GameUIManager.Instance?.UpdatePieceCounts(CountPieces(0), CountPieces(1));
    }

    private int CountPieces(int owner)
    {
        int count = 0;
        for (int y = 0; y < boardSize; y++)
            for (int x = 0; x < boardSize; x++)
                if (pieceGrid[x, y] != null && pieceGrid[x, y].owner == owner) count++;
        return count;
    }

    // ------------------------
    // 敵AI（テスト用）
    // ------------------------
    // --- 敵ターン処理呼び出し ---
    public void ExecuteEnemyTurn()
    {
        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        Debug.Log("敵ターン開始");

        yield return new WaitForSeconds(0.5f); // 遅延を少し入れると見やすい


        // 1. 手札から駒をランダム配置
        TryPlaceRandomEnemyPiece();

        yield return new WaitForSeconds(0.5f);

        // ① ドロー
        EnemyDeckManager.Instance.DrawCard();

        yield return new WaitForSeconds(0.5f);

        // ② カード使用
        CardData card = EnemyDeckManager.Instance.GetRandomCardFromHand();

        if (card != null)
        {
            Debug.Log("敵カード使用: " + card.cardName);
            card.Resolve(Vector2Int.zero); // 今はターゲット無し想定
        }

        yield return new WaitForSeconds(0.5f);

        // 2. 盤面上の敵駒をランダム移動
        TryRandomMoveEnemyPiece();

        yield return new WaitForSeconds(0.5f);

        // 3. ターン終了
        TurnManager.Instance.EndTurn();
    }

    // --- 敵駒ランダム配置 ---
    private void TryPlaceRandomEnemyPiece()
    {
        if (enemyHandPieces <= 0) return;

        // ランダム属性の敵駒を生成
        PieceData randomData = availablePieces[Random.Range(0, availablePieces.Count)];

        // 自陣ではなく敵陣（上1列）からランダムに配置
        int y = boardSize - 1;
        int x;
        do
        {
            x = Random.Range(0, boardSize);
        } while (pieceGrid[x, y] != null);

        Vector3 pos = cells[x, y].transform.position + Vector3.up * 0.5f;
        GameObject obj = Instantiate(piecePrefab, pos, Quaternion.identity);

        Piece piece = obj.GetComponent<Piece>();
        piece.Initialize(randomData, 1);

        pieceGrid[x, y] = piece;
        enemyHandPieces--;

        Debug.Log($"敵駒配置: {randomData.pieceName} at ({x},{y})");
    }

    // --- 盤面上の敵駒ランダム移動 ---
    private void TryRandomMoveEnemyPiece()
    {
        // 盤面上の敵駒を取得
        var enemyPieces = GetEnemyPiecesOnBoard();

        if (enemyPieces.Count == 0) return;

        Piece piece = enemyPieces[Random.Range(0, enemyPieces.Count)];
        Vector2Int pos = FindPiecePosition(piece);

        if (pos.x == -1) return;

        // 移動可能マスを取得
        var movable = piece.GetMovablePositions(pos, boardSize);

        if (movable.Count == 0) return;

        // ランダムに移動
        Vector2Int target = movable[Random.Range(0, movable.Count)];
        MoveEnemyPiece(piece, pos, target);

        Debug.Log($"敵駒移動: {piece.data.pieceName} from ({pos.x},{pos.y}) to ({target.x},{target.y})");
    }

    // --- ヘルパー関数 ---
    private List<Piece> GetEnemyPiecesOnBoard()
    {
        List<Piece> result = new List<Piece>();

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                Piece piece = pieceGrid[x, y];
                if (piece != null && piece.owner == 1)
                    result.Add(piece);
            }
        }

        return result;
    }
    public List<Piece> GetPlayerPiecesOnBoard()
    {
        List<Piece> result = new List<Piece>();

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                Piece piece = pieceGrid[x, y];
                if (piece != null && piece.owner == 0)
                    result.Add(piece);
            }
        }

        return result;
    }
    public List<Piece> GetPiecesByOwner(int owner)
    {
        List<Piece> result = new List<Piece>();

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                Piece piece = pieceGrid[x, y];
                if (piece != null && piece.owner == owner)
                    result.Add(piece);
            }
        }

        return result;
    }
    public void RemovePiece(Piece piece)
    {
        Vector2Int pos = FindPiecePosition(piece);

        if (pos.x == -1) return;

        pieceGrid[pos.x, pos.y] = null;
        Destroy(piece.gameObject);

        UpdatePieceCountUI();
    }
    private void MoveEnemyPiece(Piece piece, Vector2Int from, Vector2Int to)
    {
        Piece targetPiece = pieceGrid[to.x, to.y];

        if (targetPiece != null)
        {
            // 戦闘処理
            BattleResult result = BattleManager.Instance.ResolveBattle(piece, targetPiece);
            Piece winner = result.winner;
            Piece loser = result.loser;

            Vector2Int loserPos = FindPiecePosition(loser);
            pieceGrid[loserPos.x, loserPos.y] = null;
            Destroy(loser.gameObject);

            if (winner != piece)
            {
                // 敵駒が負けた場合、移動せず終了
                return;
            }
        }

        // 移動
        pieceGrid[to.x, to.y] = piece;
        pieceGrid[from.x, from.y] = null;
        piece.transform.position = cells[to.x, to.y].transform.position + Vector3.up * 0.5f;

        UpdatePieceCountUI();
    }

    // ------------------------
    // クリック処理
    // ------------------------
    private void HandleClick()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Piece piece = hit.collider.GetComponent<Piece>();
            if (piece != null)
            {
                Vector2Int pos = GetPiecePosition(piece);
                OnCellClicked(cells[pos.x, pos.y]);
                return;
            }

            BoardCell cell = hit.collider.GetComponent<BoardCell>();
            if (cell != null) { OnCellClicked(cell); return; }

            CardUseManager.Instance?.CancelCardUse();
        }
        else CardUseManager.Instance?.CancelCardUse();
    }
}