using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    public DeckData selectedDeck;

    public List<DeckEntry> deckList = new List<DeckEntry>();
    private List<CardData> runtimeDeck = new List<CardData>();
    public List<CardData> hand = new List<CardData>();

    public int maxHandSize = 8;

    private void Awake()
    {
        Instance = this;
        InitializeDeck(); 
    }

    public void InitializeDeck()
    {
        if (selectedDeck == null || !selectedDeck.IsValidDeck())
        {
            Debug.LogError("デッキが20枚ではありません");
            return;
        }

        runtimeDeck = new List<CardData>(selectedDeck.cards);
        ShuffleDeck();
    }
    private void ShuffleDeck()
    {
        for (int i = 0; i < runtimeDeck.Count; i++)
        {
            int rand = Random.Range(i, runtimeDeck.Count);
            (runtimeDeck[i], runtimeDeck[rand]) = (runtimeDeck[rand], runtimeDeck[i]);
        }
    }

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) UseCard(0);
    }

    public void DrawCard()
    {
        if (runtimeDeck.Count == 0 || hand.Count >= maxHandSize) return;

        CardData drawn = runtimeDeck[0];
        runtimeDeck.RemoveAt(0);
        hand.Add(drawn);

        CardUIManager.Instance?.RefreshHand(hand);
        Debug.Log($"ドロー: {drawn.cardName}");
    }

    public void DrawCardByCategory(CardCategory category)
    {
        for (int i = 0; i < runtimeDeck.Count; i++)
        {
            if (runtimeDeck[i].category == category)
            {
                CardData card = runtimeDeck[i];
                runtimeDeck.RemoveAt(i);
                hand.Add(card);
                CardUIManager.Instance?.RefreshHand(hand);
                Debug.Log($"特定タイプドロー: {card.cardName}");
                return;
            }
            Debug.Log("該当するカードがデッキにありません");
        }
    }

    public void RemoveTopCards(int count)
    {
        if (runtimeDeck.Count == 0) return;

        int removeCount = Mathf.Min(count, runtimeDeck.Count);

        for (int i = 0; i < removeCount; i++)
        {
            runtimeDeck.RemoveAt(0);
        }

        Debug.Log($"デッキを {removeCount} 枚ロストしました");
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

}