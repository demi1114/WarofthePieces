using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardItemUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text countText;
    public Image cardImage;

    private CardData card;
    bool isDeckCard;

    public void Setup(CardData data, bool deckCard)
    {
        card = data;
        isDeckCard = deckCard;

        nameText.text = data.cardName;

        countText.text = "";

        GetComponent<Button>().onClick.RemoveAllListeners();

        if (deckCard)
            GetComponent<Button>().onClick.AddListener(RemoveFromDeck);
        else
            GetComponent<Button>().onClick.AddListener(AddToDeck);
    }

    void AddToDeck()
    {
        if (DeckBuilderManager.Instance.AddCard(card))
        {
            DeckBuilderUI.Instance.Refresh();
        }
    }

    void RemoveFromDeck()
    {
        DeckBuilderManager.Instance.RemoveCard(card);

        DeckBuilderUI.Instance.Refresh();
    }
}