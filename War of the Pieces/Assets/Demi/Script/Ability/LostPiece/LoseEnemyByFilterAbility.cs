using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

//指定種族または属性の相手ロスト
public enum FilterType
{
    Race,
    Attribute
}

[CreateAssetMenu(menuName = "Ability/Lose Enemy By Filter")]
public class LoseEnemyByFilterAbility : Ability
{
    public FilterType filterType;

    public PieceRace targetRace;
    public PieceAttribute targetAttribute;

    public override void OnCardUse(AbilityContext context)
    {
        int enemyOwner = 1 - context.owner;

        var pieces = BoardManager.Instance.GetPiecesByOwner(enemyOwner);

        List<Piece> targets = new List<Piece>();

        foreach (var piece in pieces)
        {
            if (filterType == FilterType.Race &&
                piece.data.race == targetRace)
                targets.Add(piece);

            if (filterType == FilterType.Attribute &&
                piece.data.attribute == targetAttribute)
                targets.Add(piece);
        }
        foreach (var piece in targets)
        {
            piece.Die();
        }


        Debug.Log("条件一致の敵駒を全ロスト");
        VictoryManager.Instance.CheckAfterAction();
    }
}