using UnityEngine;

//‹@”\‚µ‚Ä‚¢‚È‚¢
[CreateAssetMenu(menuName = "Ability/AddRandomReservePieceByAttribute")]
public class AddRandomReservePieceByAttributeAbility : Ability
{
    public PieceDatabase database;
    public PieceAttribute targetAttribute;
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        var candidates = database.GetByAttribute(targetAttribute);

        if (candidates.Count == 0) return;

        for (int i = 0; i < amount; i++)
        {
            int rand = Random.Range(0, candidates.Count);
            ReserveManager.Instance.AddPiece(context.owner, candidates[rand]);
        }
    }
}