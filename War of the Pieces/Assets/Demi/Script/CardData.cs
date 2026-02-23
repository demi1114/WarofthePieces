using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Draw,
    DrawBoth,
    AddMove,
    LoseOwnBoardPieces,
    LoseOpponentBoardPieces,
    LoseOwnBoardPiecesRandom,
    LoseOpponentBoardPiecesRandom,
    LoseAllBoardPiecesRandom,
}

[CreateAssetMenu(menuName = "Card/Create Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardType cardType;
    public int value = 1;   // å¯â ó Åií«â¡à⁄ìÆêîÇ»Ç«Åj

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
                Debug.Log("à⁄ìÆâÒêî +1");
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

                        pieces.RemoveAt(randomIndex); // èdï°ñhé~
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

                    // Ç‹Ç∆ÇﬂÇÈ
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
        }

        return true;
    }

}
