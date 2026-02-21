using System.Collections.Generic;
using UnityEngine;

public class EnemyDeckManager : MonoBehaviour
{
    public static EnemyDeckManager Instance;

    public DeckData enemyDeck;   // ← 難易度ごとに差し替え可能

    private List<CardData> runtimeDeck = new List<CardData>();
    private List<CardData> hand = new List<CardData>();

    public int maxHandSize = 5;

    private void Awake()
    {
        Instance = this;
        InitializeDeck();
    }

    public void InitializeDeck()
    {
        if (enemyDeck == null || !enemyDeck.IsValidDeck())
        {
            Debug.LogError("敵デッキが20枚ではありません");
            return;
        }

        runtimeDeck = new List<CardData>(enemyDeck.cards);
        Shuffle();
    }

    private void Shuffle()
    {
        for (int i = 0; i < runtimeDeck.Count; i++)
        {
            int rand = Random.Range(i, runtimeDeck.Count);
            (runtimeDeck[i], runtimeDeck[rand]) =
            (runtimeDeck[rand], runtimeDeck[i]);
        }
    }

    public void DrawCard()
    {
        if (runtimeDeck.Count == 0 || hand.Count >= maxHandSize)
            return;

        CardData card = runtimeDeck[0];
        runtimeDeck.RemoveAt(0);
        hand.Add(card);
    }

    public CardData GetRandomCardFromHand()
    {
        if (hand.Count == 0) return null;

        int index = Random.Range(0, hand.Count);
        CardData card = hand[index];
        hand.RemoveAt(index);

        return card;
    }
}