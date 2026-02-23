using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Draw,//ƒhƒ[Œn
    DrawBoth,
    AddMove,//ˆÚ“®Œn
    LoseOwnBoardPieces,//”Õ–ÊƒƒXƒgŒn
    LoseOpponentBoardPieces,
    LoseOwnBoardPiecesRandom,
    LoseOpponentBoardPiecesRandom,
    LoseAllBoardPiecesRandom,
    LoseEnemyReservePieces,//è‹îƒƒXƒgŒn
    LoseOwnReservePieces,
    LoseEnemyBoardByType,
}

[CreateAssetMenu(menuName = "Card/Create Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardType cardType;
    public PieceAttribute targetPieceAttribute;
    public int value = 1;   // Œø‰Ê—Êi’Ç‰ÁˆÚ“®”‚È‚Çj

    public virtual bool Resolve(Vector2Int targetPos)
    {
        switch (cardType)
        {
            case CardType.Draw:
                {
                    for (int i = 0; i < value; i++)
                    {
                        DeckManager.Instance.DrawCard();
                    }
                    break;
                }

            case CardType.DrawBoth:
                {
                    for (int i = 0; i < value; i++)
                    {
                        DeckManager.Instance.DrawCard();
                        EnemyDeckManager.Instance.DrawCard();
                    }
                    break;
                }

            case CardType.AddMove:
                TurnManager.Instance.AddExtraMove(value);
                Debug.Log("ˆÚ“®‰ñ” +1");
                break;

            case CardType.LoseOwnBoardPieces:
                {
                    var pieces = BoardManager.Instance.GetPlayerPiecesOnBoard();

                    int removeCount = Mathf.Min(value, pieces.Count);

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

                    int removeCount = Mathf.Min(value, enemyPieces.Count);

                    for (int i = 0; i < removeCount; i++)
                    {
                        BoardManager.Instance.RemovePiece(enemyPieces[i]);
                    }
                    break;
                }

            case CardType.LoseOwnBoardPiecesRandom:
                {
                    var pieces = BoardManager.Instance.GetPiecesByOwner(0);

                    int removeCount = Mathf.Min(value, pieces.Count);

                    for (int i = 0; i < removeCount; i++)
                    {
                        int randomIndex = Random.Range(0, pieces.Count);
                        Piece randomPiece = pieces[randomIndex];

                        BoardManager.Instance.RemovePiece(randomPiece);

                        pieces.RemoveAt(randomIndex); // d•¡–h~
                    }
                    break;
                }

            case CardType.LoseOpponentBoardPiecesRandom:
                {
                    int opponentOwner = 1;

                    var pieces = BoardManager.Instance.GetPiecesByOwner(opponentOwner);

                    int removeCount = Mathf.Min(value, pieces.Count);

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

                    // ‚Ü‚Æ‚ß‚é
                    List<Piece> allPieces = new List<Piece>();
                    allPieces.AddRange(playerPieces);
                    allPieces.AddRange(enemyPieces);

                    int removeCount = Mathf.Min(value, allPieces.Count);

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

                    int removeCount = Mathf.Min(value, reserve.Count);

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

                    int removeCount = Mathf.Min(value, reserve.Count);

                    for (int i = 0; i < removeCount; i++)
                    {
                        int lastIndex = reserve.Count - 1;
                        BoardManager.Instance.RemovePlayerReservePiece(lastIndex);
                    }

                    Debug.Log($"©•ª‚Ìè‹î‚ğ {removeCount} ¸‚¢‚Ü‚µ‚½");
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

                    int removeCount = Mathf.Min(value, filtered.Count);

                    for (int i = 0; i < removeCount; i++)
                    {
                        BoardManager.Instance.RemovePiece(filtered[i]);
                    }

                    Debug.Log($"‘Šè‚Ì {targetPieceAttribute} ‚ğ {removeCount} ‘Ì”j‰ó");

                    return true;
                }
        }

        return true;
    }

}
