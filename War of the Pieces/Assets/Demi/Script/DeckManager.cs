using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;

    public List<DeckEntry> deckList = new List<DeckEntry>();
    private List<CardData> runtimeDeck = new List<CardData>();
    public List<CardData> hand = new List<CardData>();

    public int maxHandSize = 8;

    private void Awake()
    {
        Instance = this;
        BuildDeck();
        ShuffleDeck();
    }

    private void BuildDeck()
    {
        runtimeDeck.Clear();
        foreach (var entry in deckList)
        {
            for (int i = 0; i < entry.count; i++)
                runtimeDeck.Add(entry.cardData);
        }
    }

    private void ShuffleDeck()
    {
        for (int i = 0; i < runtimeDeck.Count; i++)
        {
            int r = Random.Range(i, runtimeDeck.Count);
            CardData temp = runtimeDeck[i];
            runtimeDeck[i] = runtimeDeck[r];
            runtimeDeck[r] = temp;
        }
    }

    public void DrawCard()
    {
        if (runtimeDeck.Count == 0 || hand.Count >= maxHandSize) return;

        CardData drawn = runtimeDeck[0];
        runtimeDeck.RemoveAt(0);
        hand.Add(drawn);

        CardUIManager.Instance?.RefreshHand(hand);
        Debug.Log($"ÉhÉçÅ[: {drawn.cardName}");
    }

    public void UseCard(int handIndex)
    {
        if (handIndex < 0 || handIndex >= hand.Count) return;
        CardUseManager.Instance.StartCardUse(hand[handIndex], handIndex);
    }

    public void RemoveCardFromHand(int index)
    {
        if (index < 0 || index >= hand.Count) return;
        hand.RemoveAt(index);
        CardUIManager.Instance?.RefreshHand(hand);
    }

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) UseCard(0);
    }
}