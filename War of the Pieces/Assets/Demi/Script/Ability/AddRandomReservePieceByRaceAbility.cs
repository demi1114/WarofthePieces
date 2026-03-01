using UnityEngine;

//‹@”\‚µ‚Ä‚¢‚È‚¢
[CreateAssetMenu(menuName = "Ability/AddRandomReservePieceByRace")]
public class AddRandomReservePieceByRaceAbility : Ability
{
    public PieceData database;
    public PieceRace targetRace;
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        var candidates = database.GetByRace(targetRace);

        if (candidates.Count == 0) return;

        for (int i = 0; i < amount; i++)
        {
            int rand = Random.Range(0, candidates.Count);
            ReserveManager.Instance.AddPiece(context.owner, candidates[rand]);
        }
    }
}