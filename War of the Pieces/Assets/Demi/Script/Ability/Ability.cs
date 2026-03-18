using UnityEngine;

public enum AbilityTargetType
{
    Auto,       // 従来（ランダム・全体・条件）
    Select,     // プレイヤー選択
}

public abstract class Ability : ScriptableObject
{
    public AbilityTargetType targetType = AbilityTargetType.Auto;

    public virtual void OnTurnStart(AbilityContext context) { }
    public virtual void OnTurnEnd(AbilityContext context) { }
    public virtual bool OnCardUse(AbilityContext context)
    {
        return true;
    }
    public virtual void OnDeath(AbilityContext context) { }
    public virtual bool IsValidTarget(Piece piece, int owner)
    {
        return true;
    }
}