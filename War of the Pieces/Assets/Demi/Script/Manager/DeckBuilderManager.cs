using System.Collections.Generic;
using UnityEngine;

public class DeckBuilderManager : MonoBehaviour
{
    public static DeckBuilderManager Instance;

    public DeckBuildData currentDeck = new DeckBuildData();

    public int maxCards = 20;

    private void Awake()
    {
        Instance = this;
    }

    public bool AddCard(CardData card)
    {
        if (currentDeck.cards.Count >= maxCards)
            return false;

        currentDeck.cards.Add(card);
        return true;
    }

    public void RemoveCard(CardData card)
    {
        currentDeck.cards.Remove(card);
    }

    public List<CardData> GetDeck()
    {
        return currentDeck.cards;
    }
}