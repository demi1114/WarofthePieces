using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckSelectSlot : MonoBehaviour
{
    public TextMeshProUGUI deckNameText;
    public TextMeshProUGUI cardCountText;
    public Button selectButton;

    int deckIndex;

    public void Setup(int index)
    {
        deckIndex = index;

        var deck = DeckBuilderManager.Instance.decks[index];

        deckNameText.text = deck.deckName;

        int count = deck.cards.Count;
        int max = DeckBuilderManager.Instance.maxCards;

        cardCountText.text = count + " / " + max;

        bool usable = count == max;

        selectButton.interactable = usable;

        if (!usable)
            cardCountText.color = Color.red;
    }

    public void OnClick()
    {
        DeckSelectManager.Instance.SelectDeck(deckIndex);
    }
}