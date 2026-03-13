using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance;

    [Header("所持カードID")]
    public List<string> ownedCardIDs = new List<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeCollection();
    }

    void InitializeCollection()
    {
        ownedCardIDs.Clear();

        // テスト用：全カード所持
        foreach (var card in CardDatabase.Instance.allCards)
        {
            if (!ownedCardIDs.Contains(card.cardID))
                ownedCardIDs.Add(card.cardID);
        }

        Debug.Log("テスト用：全カードを所持状態にしました");
    }

    public List<CardData> GetOwnedCards()
    {
        List<CardData> cards = new List<CardData>();

        foreach (string id in ownedCardIDs)
        {
            CardData card = CardDatabase.Instance.GetCardByID(id);

            if (card != null)
                cards.Add(card);
        }

        return cards;
    }

    public void AddCard(CardData card)
    {
        if (!ownedCardIDs.Contains(card.cardID))
            ownedCardIDs.Add(card.cardID);
    }

    public bool HasCard(CardData card)
    {
        return ownedCardIDs.Contains(card.cardID);
    }
}