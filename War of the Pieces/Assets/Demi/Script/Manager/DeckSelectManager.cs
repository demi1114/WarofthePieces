using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckSelectManager : MonoBehaviour
{
    public static DeckSelectManager Instance;

    public int selectedDeckIndex = -1;

    void Awake()
    {
        Instance = this;
    }

    public void SelectDeck(int index)
    {
        var deck = DeckBuilderManager.Instance.decks[index];

        if (deck.cards.Count < DeckBuilderManager.Instance.maxCards)
        {
            Debug.Log("このデッキは20枚未満なので使用できません");
            return;
        }

        selectedDeckIndex = index;

        Debug.Log("デッキ選択 : " + deck.deckName);

        StartBattle();
    }

    void StartBattle()
    {
        SceneManager.LoadScene("BattleScene");
    }
}