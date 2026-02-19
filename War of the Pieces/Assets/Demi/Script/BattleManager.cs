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

    public BattleResult ResolveBattle(Piece attacker, Piece defender)
    {
        int attackerCount = BoardManager.Instance.GetBoardCount(attacker.owner);
        int defenderCount = BoardManager.Instance.GetBoardCount(defender.owner);

        Debug.Log($"攻撃側駒数: {attackerCount}");
        Debug.Log($"防御側駒数: {defenderCount}");

        // 🔥 相性補正（戦闘中の2体のみ）
        int bonus = GetAttributeModifier(attacker.GetAttribute(), defender.GetAttribute());

        attackerCount += bonus;

        Debug.Log($"相性補正: {bonus}");
        Debug.Log($"補正後攻撃側: {attackerCount}");

        if (attackerCount > defenderCount)
        {
            return new BattleResult(attacker, defender);
        }
        else
        {
            return new BattleResult(defender, attacker);
        }
    }

    private int GetAttributeModifier(PieceAttribute attackerAttr, PieceAttribute defenderAttr)
    {
        if (IsStrongAgainst(attackerAttr, defenderAttr))
            return 1;

        if (IsWeakAgainst(attackerAttr, defenderAttr))
            return -1;

        return 0;
    }

    private bool IsStrongAgainst(PieceAttribute a, PieceAttribute b)
    {
        return
            (a == PieceAttribute.Human && b == PieceAttribute.Demon) ||
            (a == PieceAttribute.Demon && b == PieceAttribute.Fairy) ||
            (a == PieceAttribute.Fairy && b == PieceAttribute.Human);
    }

    private bool IsWeakAgainst(PieceAttribute a, PieceAttribute b)
    {
        return
            (a == PieceAttribute.Human && b == PieceAttribute.Demon) ||
            (a == PieceAttribute.Demon && b == PieceAttribute.Fairy) ||
            (a == PieceAttribute.Fairy && b == PieceAttribute.Human);
    }

    public bool PredictWinner(Piece attacker, Piece defender)
    {
        BattleResult result = ResolveBattle(attacker, defender);
        return result.winner == attacker;
    }
}

