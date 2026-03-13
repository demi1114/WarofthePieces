using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckBuildData
{
    public string deckName;
    public List<CardData> cards = new List<CardData>();
}