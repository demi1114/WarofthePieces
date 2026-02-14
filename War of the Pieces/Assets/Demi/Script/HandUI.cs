using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HandUI : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform handPanel;

    private List<GameObject> currentCards = new List<GameObject>();

    private void Start()
    {
        RefreshHand();
    }

    public void RefreshHand()
    {
        // Šù‘¶ƒJ[ƒhíœ
        foreach (var card in currentCards)
        {
            Destroy(card);
        }

        currentCards.Clear();

        // èD•ª¶¬
        for (int i = 0; i < DeckManager.Instance.hand.Count; i++)
        {
            int index = i;

            GameObject cardObj = Instantiate(cardPrefab, handPanel);

            TMP_Text text = cardObj.GetComponentInChildren<TMP_Text>();
            text.text = DeckManager.Instance.hand[i].cardName;

            Button btn = cardObj.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                DeckManager.Instance.UseCard(index);
                RefreshHand();
            });

            currentCards.Add(cardObj);
        }
    }
}
