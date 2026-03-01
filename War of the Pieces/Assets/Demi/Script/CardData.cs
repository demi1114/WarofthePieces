using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
   
    AddMove,//移動系
    LoseOwnBoardPieces,//盤面ロスト系
    LoseOpponentBoardPieces,
    LoseOwnBoardPiecesRandom,
    LoseOpponentBoardPiecesRandom,
    LoseAllBoardPiecesRandom,
    BuffOwnBoardByTypePermanent,//強化系
    BuffOwnBoardByTypeTemporary,
    DebuffEnemySinglePermanent,//弱体化系
    DebuffEnemySingleTemporary,
    DebuffEnemyRandomTemporary,
    LoseEnemyDeck,//デッキロスト系
    LoseOwnDeck,
    LoseBothDeck,
    ReturnEnemyBoardPieces,//バウンス系
    ReturnOwnBoardPieces,
    ReturnRandomBoardPieces,
    LoseAllBoardPieces,//盤面全ロスト
    SpawnSpecificPieceRandomly,//直接召喚
    TransformOwnSingle,//変身系
    TransformEnemySingle,
    TransformOwnRandom,
    TransformAllOwn,
}
public enum CardCategory
{
    Attack,
    Defense,
    Spell,
    Support
}

[CreateAssetMenu(menuName = "Card/Create Card")]
public class CardData : ScriptableObject
{
    public string cardName;

    [Header("Abilities")]
    public List<Ability> abilities = new List<Ability>();

    public CardCategory category;
    public CardType cardType;
    public PieceAttribute targetPieceAttribute;
    [Header("Effect Settings")]
    public int amount = 1;        // 強化量・弱体量・ドロー枚数
    public int targetCount = 1;   // 対象数（複数効果用）
    [Header("Piece Add Settings")]
    public PieceData specificPiece;          // 特定の駒
    public List<PieceData> candidatePieces;  // ランダム候補リスト

    public virtual bool Resolve(int owner, Vector2Int targetPos)
    {
        switch (cardType)
        {
            case CardType.AddMove:
                TurnManager.Instance.AddExtraMove(amount);
                Debug.Log("移動回数 +1");
                break;

            case CardType.LoseOwnBoardPieces:
                {
                    var pieces = BoardManager.Instance.GetPlayerPiecesOnBoard();

                    int removeCount = Mathf.Min(targetCount, pieces.Count);

                    for (int i = 0; i < removeCount; i++)
                    {
                        BoardManager.Instance.RemovePiece(pieces[i]);
                    }
                    break;
                }

            case CardType.LoseOpponentBoardPieces:
                {
                    int opponentOwner = 1;

                    var enemyPieces = BoardManager.Instance.GetPiecesByOwner(opponentOwner);

                    int removeCount = Mathf.Min(targetCount, enemyPieces.Count);

                    for (int i = 0; i < removeCount; i++)
                    {
                        BoardManager.Instance.RemovePiece(enemyPieces[i]);
                    }
                    break;
                }

            case CardType.LoseOwnBoardPiecesRandom:
                {
                    var pieces = BoardManager.Instance.GetPiecesByOwner(0);

                    int removeCount = Mathf.Min(targetCount, pieces.Count);

                    for (int i = 0; i < removeCount; i++)
                    {
                        int randomIndex = Random.Range(0, pieces.Count);
                        Piece randomPiece = pieces[randomIndex];

                        BoardManager.Instance.RemovePiece(randomPiece);

                        pieces.RemoveAt(randomIndex); // 重複防止
                    }
                    break;
                }

            case CardType.LoseOpponentBoardPiecesRandom:
                {
                    var pieces = BoardManager.Instance.GetPiecesByOwner(1);

                    int removeCount = Mathf.Min(targetCount, pieces.Count);

                    for (int i = 0; i < removeCount; i++)
                    {
                        int rand = Random.Range(0, pieces.Count);
                        BoardManager.Instance.RemovePiece(pieces[rand]);
                        pieces.RemoveAt(rand);
                    }
                    break;
                }

            case CardType.LoseAllBoardPiecesRandom:
                {
                    var playerPieces = BoardManager.Instance.GetPiecesByOwner(0);
                    var enemyPieces = BoardManager.Instance.GetPiecesByOwner(1);

                    // まとめる
                    List<Piece> allPieces = new List<Piece>();
                    allPieces.AddRange(playerPieces);
                    allPieces.AddRange(enemyPieces);

                    int removeCount = Mathf.Min(targetCount, allPieces.Count);

                    for (int i = 0; i < removeCount; i++)
                    {
                        int randomIndex = Random.Range(0, allPieces.Count);
                        Piece randomPiece = allPieces[randomIndex];

                        BoardManager.Instance.RemovePiece(randomPiece);

                        allPieces.RemoveAt(randomIndex);
                    }
                    break;
                }

            case CardType.BuffOwnBoardByTypePermanent:
                {
                    var pieces = BoardManager.Instance.GetPiecesByOwner(0);

                    List<Piece> filtered = new List<Piece>();

                    foreach (var piece in pieces)
                    {
                        if (piece.data.attribute == targetPieceAttribute)
                            filtered.Add(piece);
                    }

                    int applyCount = Mathf.Min(amount, filtered.Count);

                    for (int i = 0; i < applyCount; i++)
                    {
                        filtered[i].AddPermanentPower(1); // ← 強化量を固定1にするなら
                                                          // 強化量も変数化したいなら別intを追加
                    }

                    break;
                }

            case CardType.BuffOwnBoardByTypeTemporary:
                {
                    var pieces = BoardManager.Instance.GetPiecesByOwner(0);

                    List<Piece> filtered = new List<Piece>();

                    foreach (var piece in pieces)
                    {
                        if (piece.data.attribute == targetPieceAttribute)
                            filtered.Add(piece);
                    }

                    int applyCount = Mathf.Min(amount, filtered.Count);

                    for (int i = 0; i < applyCount; i++)
                    {
                        filtered[i].AddTemporaryPower(1);
                    }

                    break;
                }

            case CardType.DebuffEnemySinglePermanent:
                {
                    Piece target = BoardManager.Instance.GetPieceAt(targetPos);

                    if (target != null && target.owner == 1)
                    {
                        target.AddPermanentPower(-amount);
                    }

                    break;
                }

            case CardType.DebuffEnemySingleTemporary:
                {
                    Piece target = BoardManager.Instance.GetPieceAt(targetPos);

                    if (target != null && target.owner == 1)
                    {
                        target.AddTemporaryPower(-amount);
                    }

                    break;
                }

            case CardType.DebuffEnemyRandomTemporary:
                {
                    var pieces = BoardManager.Instance.GetPiecesByOwner(1);

                    int applyCount = Mathf.Min(amount, pieces.Count);

                    for (int i = 0; i < applyCount; i++)
                    {
                        int randomIndex = Random.Range(0, pieces.Count);
                        pieces[randomIndex].AddTemporaryPower(-1);
                        pieces.RemoveAt(randomIndex);
                    }

                    break;
                }

            case CardType.ReturnOwnBoardPieces:
                {
                    var pieces = BoardManager.Instance.GetPiecesByOwner(0);

                    int returnCount = Mathf.Min(targetCount, pieces.Count);

                    for (int i = 0; i < returnCount; i++)
                    {
                        BoardManager.Instance.ReturnPieceToReserve(pieces[i]);
                    }

                    break;
                }

            case CardType.ReturnEnemyBoardPieces:
                {
                    var pieces = BoardManager.Instance.GetPiecesByOwner(1);

                    int returnCount = Mathf.Min(targetCount, pieces.Count);

                    for (int i = 0; i < returnCount; i++)
                    {
                        BoardManager.Instance.ReturnPieceToReserve(pieces[i]);
                    }

                    break;
                }

            case CardType.ReturnRandomBoardPieces:
                {
                    var playerPieces = BoardManager.Instance.GetPiecesByOwner(0);
                    var enemyPieces = BoardManager.Instance.GetPiecesByOwner(1);

                    List<Piece> all = new List<Piece>();
                    all.AddRange(playerPieces);
                    all.AddRange(enemyPieces);

                    int returnCount = Mathf.Min(targetCount, all.Count);

                    for (int i = 0; i < returnCount; i++)
                    {
                        int rand = Random.Range(0, all.Count);
                        BoardManager.Instance.ReturnPieceToReserve(all[rand]);
                        all.RemoveAt(rand);
                    }

                    break;
                }

            case CardType.LoseAllBoardPieces:
                {
                    var playerPieces = BoardManager.Instance.GetPiecesByOwner(0);
                    var enemyPieces = BoardManager.Instance.GetPiecesByOwner(1);

                    List<Piece> all = new List<Piece>();
                    all.AddRange(playerPieces);
                    all.AddRange(enemyPieces);

                    foreach (var piece in all)
                    {
                        BoardManager.Instance.RemovePiece(piece);
                    }

                    Debug.Log("盤面のすべての駒をロスト");

                    break;
                }

            case CardType.SpawnSpecificPieceRandomly:
                {
                    if (specificPiece == null) break;

                    var emptyCells = BoardManager.Instance.GetEmptyPlayerCells();

                    int spawnCount = Mathf.Min(amount, emptyCells.Count);

                    for (int i = 0; i < spawnCount; i++)
                    {
                        int rand = Random.Range(0, emptyCells.Count);
                        Vector2Int pos = emptyCells[rand];

                        BoardManager.Instance.SpawnSpecificPieceInPlayerArea(specificPiece, pos);

                        emptyCells.RemoveAt(rand);
                    }

                    break;
                }

            case CardType.TransformOwnSingle:
                {
                    if (specificPiece == null) break;

                    Piece target = BoardManager.Instance.GetPieceAt(targetPos);

                    if (target != null && target.owner == 0)
                    {
                        target.Transform(specificPiece);
                    }

                    break;
                }

            case CardType.TransformEnemySingle:
                {
                    if (specificPiece == null) break;

                    Piece target = BoardManager.Instance.GetPieceAt(targetPos);

                    if (target != null && target.owner == 1)
                    {
                        target.Transform(specificPiece);
                    }

                    break;
                }

            case CardType.TransformAllOwn:
                {
                    if (specificPiece == null) break;

                    var pieces = BoardManager.Instance.GetPiecesByOwner(0);

                    foreach (var piece in pieces)
                    {
                        piece.Transform(specificPiece);
                    }

                    break;
                }
        }


        AbilityContext context = new AbilityContext
        {
            owner = owner,
            targetPosition = targetPos,
            sourceCard = this
        };

        foreach (var ability in abilities)
        {
            ability.OnCardUse(context);
        }


        return true;
    }

}
