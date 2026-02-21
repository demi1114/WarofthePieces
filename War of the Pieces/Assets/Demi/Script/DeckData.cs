using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "KO/Deck")]
public class DeckData : ScriptableObject
{
    public List<CardData> cards = new List<CardData>();

    public bool IsValidDeck()
    {
        return cards.Count == 20;
    }
}