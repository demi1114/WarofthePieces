using UnityEngine;
using System.Collections.Generic;

//相手の駒ランダムロスト
[CreateAssetMenu(menuName = "Ability/Lose Random Enemy Piece")]
public class LoseRandomEnemyPieceAbility : Ability
{
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        int enemyOwner = 1 - context.owner;

        var pieces = BoardManager.Instance.GetPiecesByOwner(enemyOwner);
        if (pieces.Count == 0) return;

        int count = Mathf.Min(amount, pieces.Count);

        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(0, pieces.Count);
            Piece target = pieces[rand];

            BoardManager.Instance.RemovePiece(target);
            pieces.RemoveAt(rand);
        }

        Debug.Log("ランダム相手駒をロスト");
        VictoryManager.Instance.CheckAfterAction();
    }
}