using UnityEngine;
using System.Collections.Generic;

//ğŒˆê’v‚Ì‹î‚©‚çƒ‰ƒ“ƒ_ƒ€”j‰ó
[CreateAssetMenu(menuName = "Ability/Lose Random By Filter")]
public class LoseRandomByFilterAbility : Ability
{
    public FilterType filterType;

    public PieceRace targetRace;
    public PieceAttribute targetAttribute;

    public bool targetEnemy = true; //true=“G false=©•ª

    [Header("Random Lose Count")]
    public int amount = 1;
    public override void OnCardUse(AbilityContext context)
    {
        int owner = targetEnemy ? 1 - context.owner : context.owner;

        var pieces = BoardManager.Instance.GetPiecesByOwner(owner);

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

        if (targets.Count == 0)
        {
            Debug.Log("‘ÎÛ‚ª‘¶İ‚µ‚Ü‚¹‚ñ");
            return;
        }

        int destroyCount = Mathf.Min(amount, targets.Count);

        for (int i = 0; i < destroyCount; i++)
        {
            int rand = Random.Range(0, targets.Count);

            Piece target = targets[rand];

            target.Die();

            targets.RemoveAt(rand); //“¯‚¶‹î‚ğ‘I‚Î‚È‚¢
        }

        Debug.Log($"ğŒˆê’v‹î‚ğƒ‰ƒ“ƒ_ƒ€{destroyCount}‘Ì”j‰ó");
        VictoryManager.Instance.CheckAfterAction();
    }
}