using UnityEngine;

//©•ª‚Ìè‹î‚Ì“Á’èí‘°ƒ‰ƒ“ƒ_ƒ€íœ
[CreateAssetMenu(menuName = "Ability/LoseOwnReserveByRace")]
public class LoseOwnReserveByRaceAbility : Ability
{
    public PieceRace targetRace;
    public int amount = 1;

    public override void OnCardUse(AbilityContext context)
    {
        for (int i = 0; i < amount; i++)
        {
            ReserveManager.Instance.RemoveRandomPieceByRace(context.owner, targetRace);
        }
    }
}