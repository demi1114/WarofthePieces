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
        Debug.Log("増えたよ");
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

        Debug.Log($"敵デッキを {removeCount} 枚ロストしました");
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