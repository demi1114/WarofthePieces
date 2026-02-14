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
    }

    private void Start()
    {
        BuildDeck();
        ShuffleDeck();
    }

    void BuildDeck()
    {
        runtimeDeck.Clear();

        foreach (var entry in deckList)
        {
            for (int i = 0; i < entry.count; i++)
            {
                runtimeDeck.Add(entry.cardData);
            }
        }
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < runtimeDeck.Count; i++)
        {
            CardData temp = runtimeDeck[i];
            int randomIndex = Random.Range(i, runtimeDeck.Count);
            runtimeDeck[i] = runtimeDeck[randomIndex];
            runtimeDeck[randomIndex] = temp;
        }
    }

    public void DrawCard()
    {
        if (runtimeDeck.Count == 0)
        {
            Debug.Log("デッキがありません");
            return;
        }

        if (hand.Count >= maxHandSize)
        {
            Debug.Log("手札上限です");
            return;
        }

        CardData drawn = runtimeDeck[0];
        runtimeDeck.RemoveAt(0);
        hand.Add(drawn);

        Debug.Log($"ドロー: {drawn.cardName}");
        FindObjectOfType<HandUI>().RefreshHand();

    }
    public void UseCard(int handIndex)
    {
        if (handIndex < 0 || handIndex >= hand.Count)
            return;

        CardData card = hand[handIndex];

        CardUseManager.Instance.StartCardUse(card, handIndex);
    }
    public void RemoveCardFromHand(int index)
    {
        if (index < 0 || index >= hand.Count)
            return;

        hand.RemoveAt(index);
        HandUI.Instance.RefreshHand();
    }


    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            UseCard(0);
        }
    }

}
