using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static CardDatabase Instance;

    public List<CardData> allCards = new List<CardData>();

    Dictionary<string, CardData> cardDict = new Dictionary<string, CardData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeDatabase()
    {
        allCards.Clear();
        cardDict.Clear();

        // Resources/CardData から全カード読み込み
        CardData[] cards = Resources.LoadAll<CardData>("Card");

        foreach (var card in cards)
        {
            allCards.Add(card);

            if (!cardDict.ContainsKey(card.cardID))
                cardDict.Add(card.cardID, card);
        }

        Debug.Log($"CardDatabase: {allCards.Count} 枚のカードをロード");
    }

    public CardData GetCardByID(string id)
    {
        if (cardDict.TryGetValue(id, out CardData card))
            return card;

        return null;
    }
}