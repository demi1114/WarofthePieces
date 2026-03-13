using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckSaveData
{
    public List<string> cards;
}

public class DeckBuilderManager : MonoBehaviour
{
    public static DeckBuilderManager Instance;

    public int maxCards = 20;
    public int maxDecks = 5;
    public int maxSameCard = 3;

    public List<DeckBuildData> decks = new List<DeckBuildData>();

    public int currentDeckIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (decks.Count == 0)
            {
                for (int i = 0; i < maxDecks; i++)
                {
                    DeckBuildData deck = new DeckBuildData();
                    deck.deckName = "Deck " + (i + 1);
                    deck.cards = new List<CardData>();
                    decks.Add(deck);
                }
            }
            // 保存デッキロード
            for (int i = 0; i < maxDecks; i++)
            {
                LoadDeck(i);
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public DeckBuildData GetCurrentDeckData()
    {
        return decks[currentDeckIndex];
    }

    public List<CardData> GetCurrentDeck()
    {
        return decks[currentDeckIndex].cards;
    }

    public bool AddCard(CardData card)
    {
        var deck = GetCurrentDeck();

        if (deck.Count >= maxCards)
            return false;

        int sameCount = 0;

        foreach (var c in deck)
        {
            if (c.cardID == card.cardID)
                sameCount++;
        }

        if (sameCount >= maxSameCard)
            return false;

        deck.Add(card);
        return true;
    }

    public void RemoveCard(CardData card)
    {
        GetCurrentDeck().Remove(card);
    }

    public void SetCurrentDeck(int index)
    {
        if (index < 0 || index >= decks.Count) return;

        currentDeckIndex = index;
    }

    public void SaveDeck()
    {
        DeckSaveData data = new DeckSaveData();
        data.cards = new List<string>();

        foreach (var card in GetCurrentDeck())
        {
            data.cards.Add(card.cardID);
        }

        string json = JsonUtility.ToJson(data);

        PlayerPrefs.SetString("Deck_" + currentDeckIndex, json);
        PlayerPrefs.Save();
    }

    public void SaveCurrentDeck()
    {
        SaveDeck();
    }

    public void ChangeDeck(int index)
    {
        if (index < 0 || index >= decks.Count)
            return;

        currentDeckIndex = index;

        LoadDeck(index);

        if (DeckBuilderUI.Instance != null)
            DeckBuilderUI.Instance.Refresh();
    }

    void LoadDeck(int index)
    {
        string key = "Deck_" + index;

        var deck = decks[index].cards;

        if (!PlayerPrefs.HasKey(key))
        {
            deck.Clear();
            return;
        }

        string json = PlayerPrefs.GetString(key);

        DeckSaveData data = JsonUtility.FromJson<DeckSaveData>(json);

        deck.Clear();

        foreach (string id in data.cards)
        {
            CardData card = CardDatabase.Instance.GetCardByID(id);

            if (card != null)
                deck.Add(card);
        }
    }
}