using UnityEngine;

[System.Serializable]
public class DeckEntry
{
    public CardData cardData;
    [Range(1, 5)]
    public int count;
}
