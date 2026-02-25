using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Draw,//ドロー系
    DrawBoth,
    AddMove,//移動系
    LoseOwnBoardPieces,//盤面ロスト系
    LoseOpponentBoardPieces,
    LoseOwnBoardPiecesRandom,
    LoseOpponentBoardPiecesRandom,
    LoseAllBoardPiecesRandom,
    LoseEnemyReservePieces,//手駒ロスト系
    LoseOwnReservePieces,
    LoseEnemyBoardByType,
    BuffOwnBoardByTypePermanent,//強化系
    BuffOwnBoardByTypeTemporary,
    DebuffEnemySinglePermanent,//弱体化系
    DebuffEnemySingleTemporary,
    DebuffEnemyRandomTemporary,
    LoseEnemyDeck,//デッキロスト系
    LoseOwnDeck,
    LoseBothDeck,
}

[CreateAssetMenu(menuName = "Card/Create Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardType cardType;
    public PieceAttribute targetPieceAttribute;
    [Header("Effect Settings")]
    public int amount = 1;        // 強化量・弱体量・ドロー枚数
    public int targetCount = 1;   // 対象数（複数効果用）

    public virtual bool Resolve(Vector2Int targetPos)
    {
        switch (cardType)
        {
            case CardType.Draw:
                {
                    for (int i = 0; i < amount; i++)
                    {
                        DeckManager.Instance.DrawCard();
                    }
                    break;
                }

            case CardType.DrawBoth:
                {
                    for (int i = 0; i < amount; i++)
                    {
                        DeckManager.Instance.DrawCard();
                        EnemyDeckManager.Instance.DrawCard();
                    }
                    break;
                }

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
                    int opponentOwner = 1;

                    var pieces = BoardManager.Instance.GetPiecesByOwner(opponentOwner);

                    int removeCount = Mathf.Min(targetCount, pieces.Count);

                    for (int i = 0; i < removeCount; i++)
                    {
                        int randomIndex = Random.Range(0, pieces.Count);
                        Piece randomPiece = pieces[randomIndex];

                        BoardManager.Instance.RemovePiece(randomPiece);

                        pieces.RemoveAt(randomIndex);
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

            case CardType.LoseEnemyReservePieces:
                {
                    var reserve = BoardManager.Instance.enemyHandPiece;

                    int removeCount = Mathf.Min(targetCount, reserve.Count);

                    for (int i = 0; i < removeCount; i++)
                    {
                        int lastIndex = reserve.Count - 1;
                        BoardManager.Instance.RemoveEnemyReservePiece(lastIndex);
                    }

                    break;
                }

            case CardType.LoseOwnReservePieces:
                {
                    var reserve = BoardManager.Instance.playerHandPiece;

                    int removeCount = Mathf.Min(targetCount, reserve.Count);

                    for (int i = 0; i < removeCount; i++)
                    {
                        int lastIndex = reserve.Count - 1;
                        BoardManager.Instance.RemovePlayerReservePiece(lastIndex);
                    }

                    Debug.Log($"自分の手駒を {removeCount} 失いました");
                    break;
                }

            case CardType.LoseEnemyBoardByType:
                {
                    var enemyPieces = BoardManager.Instance.GetPiecesByOwner(1);

                    List<Piece> filtered = new List<Piece>();

                    foreach (var piece in enemyPieces)
                    {
                        if (piece.data.attribute == targetPieceAttribute)
                            filtered.Add(piece);
                    }

                    int removeCount = Mathf.Min(targetCount, filtered.Count);

                    for (int i = 0; i < removeCount; i++)
                    {
                        BoardManager.Instance.RemovePiece(filtered[i]);
                    }

                    Debug.Log($"相手の {targetPieceAttribute} を {removeCount} 体破壊");

                    return true;
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

            case CardType.LoseEnemyDeck:
                {
                    EnemyDeckManager.Instance.RemoveTopCards(amount);
                    break;
                }

            case CardType.LoseOwnDeck:
                {
                    DeckManager.Instance.RemoveTopCards(amount);
                    break;
                }

            case CardType.LoseBothDeck:
                {
                    DeckManager.Instance.RemoveTopCards(amount);
                    EnemyDeckManager.Instance.RemoveTopCards(amount);
                    break;
                }
        }

        return true;
    }

}
