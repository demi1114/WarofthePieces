using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class DeckBuilderUI : MonoBehaviour
{
    public static DeckBuilderUI Instance;

    public Transform deckContainer;
    public Transform collectionContainer;

    public GameObject cardPrefab;
    public TMP_Text deckCountText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        Clear(deckContainer);
        Clear(collectionContainer);

        List<CardData> deck =
            DeckBuilderManager.Instance.GetCurrentDeck();

        List<CardData> collection =
            CollectionManager.Instance.GetOwnedCards();

        // ----------------
        // Deck
        // ----------------

        var deckGroup = deck.GroupBy(c => c.cardID);

        foreach (var group in deckGroup)
        {
            CardData card = group.First();
            int count = group.Count();

            CreateDeckCard(card, count);
        }

        // ----------------
        // Collection
        // ----------------

        var collectionGroup = collection.GroupBy(c => c.cardID);

        foreach (var group in collectionGroup)
        {
            CardData card = group.First();
            int count = group.Count();

            CreateCollectionCard(card, count);
        }

        deckCountText.text = $"Deck {deck.Count}/20";
    }

    void CreateDeckCard(CardData card, int count)
    {
        GameObject obj = Instantiate(cardPrefab, deckContainer);

        CardItemUI ui = obj.GetComponent<CardItemUI>();

        ui.Setup(card, true);

        ui.countText.text = "Å~" + count;
    }

    void CreateCollectionCard(CardData card, int count)
    {
        GameObject obj = Instantiate(cardPrefab, collectionContainer);

        CardItemUI ui = obj.GetComponent<CardItemUI>();

        ui.Setup(card, false);

        ui.countText.text = "Å~" + count;
    }

    void Clear(Transform t)
    {
        foreach (Transform child in t)
            Destroy(child.gameObject);
    }
}