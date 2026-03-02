using UnityEngine;
using System.Collections.Generic;

//永続のランダム相手デバフ  
[CreateAssetMenu(menuName = "Ability/Debuff Random Enemy Permanent")]
public class DebuffRandomEnemyPermanentAbility : Ability
{
    public int amount = 1;
    public int count = 1;

    public override void OnCardUse(AbilityContext context)
    {
        int enemyOwner = 1 - context.owner;

        var pieces = BoardManager.Instance.GetPiecesByOwner(enemyOwner);
        if (pieces.Count == 0) return;

        int applyCount = Mathf.Min(count, pieces.Count);

        for (int i = 0; i < applyCount; i++)
        {
            Piece randomPiece = pieces[Random.Range(0, pieces.Count)];
            randomPiece.AddPermanentPower(-amount);
            pieces.Remove(randomPiece);
        }
    }
}