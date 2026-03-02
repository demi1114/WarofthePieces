using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public virtual void OnTurnStart(AbilityContext context) { }
    public virtual void OnTurnEnd(AbilityContext context) { }
    public virtual void OnCardUse(AbilityContext context) { }
    public virtual void OnPiecePlaced(AbilityContext context) { }
}