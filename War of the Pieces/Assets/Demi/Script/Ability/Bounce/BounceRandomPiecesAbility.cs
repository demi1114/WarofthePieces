using UnityEngine;
using System.Collections.Generic;

//ランダムな駒をバウンス
[CreateAssetMenu(menuName = "Ability/Bounce Random Pieces")]
public class BounceRandomPiecesAbility : Ability
{
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        var playerPieces = BoardManager.Instance.GetPiecesByOwner(0);
        var enemyPieces = BoardManager.Instance.GetPiecesByOwner(1);

        List<Piece> allPieces = new List<Piece>();
        allPieces.AddRange(playerPieces);
        allPieces.AddRange(enemyPieces);

        if (allPieces.Count == 0) return;

        int count = Mathf.Min(amount, allPieces.Count);

        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(0, allPieces.Count);
            Piece piece = allPieces[rand];

            BoardManager.Instance.ReturnPieceToReserve(piece);

            allPieces.RemoveAt(rand);
        }

        Debug.Log("ランダム駒をバウンスしました");
    }
}