using UnityEngine;

public class BattleResult
{
    public Piece winner;
    public Piece loser;

    public BattleResult(Piece winner, Piece loser)
    {
        this.winner = winner;
        this.loser = loser;
    }
}

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    public BattleResult ResolveBattle(
    Piece attacker,
    Piece defender,
    int attackerBoardCount,
    int defenderBoardCount)
    {
        int attackerScore =
            attackerBoardCount + attacker.CurrentPower;

        int defenderScore =
            defenderBoardCount + defender.CurrentPower;

        int bonus =
            GetAttributeModifier(
                attacker.GetAttribute(),
                defender.GetAttribute());

        attackerScore += bonus;

        if (attackerScore > defenderScore)
            return new BattleResult(attacker, defender);
        else
            return new BattleResult(defender, attacker);
    }

    private int GetAttributeModifier(PieceAttribute attackerAttr, PieceAttribute defenderAttr)
    {
        if (IsStrongAgainst(attackerAttr, defenderAttr)) return 1;
        if (IsStrongAgainst(defenderAttr, attackerAttr)) return -1;
        return 0;
    }

    private bool IsStrongAgainst(PieceAttribute a, PieceAttribute b) =>
        (a == PieceAttribute.Human && b == PieceAttribute.Demon) ||
        (a == PieceAttribute.Demon && b == PieceAttribute.Fairy) ||
        (a == PieceAttribute.Fairy && b == PieceAttribute.Human);

    public bool PredictWinner(Piece attacker, Piece defender, int attackerBoardCount, int defenderBoardCount)
    {
        return ResolveBattle(
            attacker,
            defender,
            attackerBoardCount,
            defenderBoardCount
        ).winner == attacker;
    }
}