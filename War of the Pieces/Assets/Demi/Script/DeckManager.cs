using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;

    [Header("Deck Data")]
    public DeckData playerDeckData;
    public DeckData enemyDeckData;

    private List<CardData> playerDeck = new List<CardData>();
    private List<CardData> enemyDeck = new List<CardData>();

    private List<CardData> playerHand = new List<CardData>();
    private List<CardData> enemyHand = new List<CardData>();

    public int playerMaxHandSize = 8;
    public int enemyMaxHandSize = 5;

    private void Awake()
    {
        Instance = this;
        InitializeDeck(0);
        InitializeDeck(1);
    }

    // =============================
    // 初期化
    // =============================

    public void InitializeDeck(int owner)
    {
        DeckData source = owner == 0 ? playerDeckData : enemyDeckData;

        if (source == null || !source.IsValidDeck())
        {
            Debug.LogError($"{owner} のデッキが20枚ではありません");
            return;
        }

        var runtime = new List<CardData>(source.cards);
        Shuffle(runtime);

        if (owner == 0)
            playerDeck = runtime;
        else
            enemyDeck = runtime;
    }

    private void Shuffle(List<CardData> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            (deck[i], deck[rand]) = (deck[rand], deck[i]);
        }
    }

    // =============================
    // 共通取得
    // =============================

    private List<CardData> GetDeck(int owner)
    {
        return owner == 0 ? playerDeck : enemyDeck;
    }

    private List<CardData> GetHand(int owner)
    {
        return owner == 0 ? playerHand : enemyHand;
    }

    private int GetMaxHandSize(int owner)
    {
        return owner == 0 ? playerMaxHandSize : enemyMaxHandSize;
    }

    // =============================
    // ドロー
    // =============================

    public void DrawCard(int owner)
    {
        var deck = GetDeck(owner);
        var hand = GetHand(owner);

        if (deck.Count == 0 || hand.Count >= GetMaxHandSize(owner))
            return;

        CardData drawn = deck[0];
        deck.RemoveAt(0);
        hand.Add(drawn);

        if (owner == 0)
            CardUIManager.Instance?.RefreshHand(hand);

        Debug.Log($"{owner} ドロー: {drawn.cardName}");
    }

    public void DrawCardByCategory(int owner, CardCategory category)
    {
        var deck = GetDeck(owner);
        var hand = GetHand(owner);

        for (int i = 0; i < deck.Count; i++)
        {
            if (deck[i].category == category)
            {
                CardData card = deck[i];
                deck.RemoveAt(i);
                hand.Add(card);

                if (owner == 0)
                    CardUIManager.Instance?.RefreshHand(hand);

                Debug.Log($"{owner} 特定タイプドロー: {card.cardName}");
                return;
            }
        }

        Debug.Log("該当カードなし");
    }

    // =============================
    // デッキロスト
    // =============================

    public void RemoveTopCards(int owner, int count)
    {
        var deck = GetDeck(owner);

        if (deck.Count == 0) return;

        int removeCount = Mathf.Min(count, deck.Count);

        for (int i = 0; i < removeCount; i++)
        {
            deck.RemoveAt(0);
        }

        Debug.Log($"{owner} のデッキを {removeCount} 枚ロスト");
    }

    // =============================
    // 手札操作
    // =============================

    public void UseCard(int handIndex)
    {
        if (handIndex < 0 || handIndex >= playerHand.Count) return;

        CardUseManager.Instance.StartCardUse(playerHand[handIndex], handIndex, 0);
    }

    public void RemoveCardFromHand(int owner, int index)
    {
        var hand = GetHand(owner);

        if (index < 0 || index >= hand.Count) return;

        hand.RemoveAt(index);

        if (owner == 0)
            CardUIManager.Instance?.RefreshHand(hand);
    }

    public CardData GetRandomCardFromHand(int owner)
    {
        var hand = GetHand(owner);

        if (hand.Count == 0) return null;

        int index = Random.Range(0, hand.Count);
        CardData card = hand[index];
        hand.RemoveAt(index);

        return card;
    }
}