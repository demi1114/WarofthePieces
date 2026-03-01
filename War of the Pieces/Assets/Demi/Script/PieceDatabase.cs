using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/PieceDatabase")]
public class PieceDatabase : ScriptableObject
{
    public List<PieceData> allPieces = new List<PieceData>();

    // Raceåüçı
    public List<PieceData> GetByRace(PieceRace race)
    {
        List<PieceData> result = new List<PieceData>();

        foreach (var piece in allPieces)
        {
            if (piece.race == race)
                result.Add(piece);
        }

        return result;
    }

    // Attributeåüçı
    public List<PieceData> GetByAttribute(PieceAttribute attribute)
    {
        List<PieceData> result = new List<PieceData>();

        foreach (var piece in allPieces)
        {
            if (piece.attribute == attribute)
                result.Add(piece);
        }

        return result;
    }
}