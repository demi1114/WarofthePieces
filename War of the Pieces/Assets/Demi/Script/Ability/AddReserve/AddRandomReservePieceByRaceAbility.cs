using UnityEngine;

//機能していない
[CreateAssetMenu(menuName = "Ability/AddRandomReservePieceByRace")]
public class AddRandomReservePieceByRaceAbility : Ability
{
    public PieceDatabase database;
    public PieceRace targetRace;
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        Debug.Log("Ability 発動確認");
        Debug.Log("owner = " + context.owner);
        var candidates = database.GetByRace(targetRace);

        if (candidates.Count == 0) return;
        Debug.Log("候補数 = " + candidates.Count);
        for (int i = 0; i < amount; i++)
        {
            int rand = Random.Range(0, candidates.Count);
            ReserveManager.Instance.AddPiece(context.owner, candidates[rand]);
        }
    }
}