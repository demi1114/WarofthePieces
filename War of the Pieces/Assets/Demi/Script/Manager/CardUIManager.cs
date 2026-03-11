using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CardUIManager : MonoBehaviour
{
    public static CardUIManager Instance;

    public Transform cardHandArea;
    public GameObject cardButtonPrefab;

    private void Awake() => Instance = this;

    public void RefreshHand(List<CardData> hand)
    {
        foreach (Transform child in cardHandArea) Destroy(child.gameObject);

        for (int i = 0; i < hand.Count; i++)
        {
            CardData card = hand[i];
            int index = i;  // ローカルコピー

            GameObject btn = Instantiate(cardButtonPrefab, cardHandArea);
            btn.GetComponentInChildren<TMP_Text>().text = card.cardName;
            Button button = btn.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                DetailPanelUI.Instance.ShowCard(card);

                if (TurnManager.Instance.isPlayerTurn)
                {
                    CardUseManager.Instance.StartCardUse(card, index, 0);
                }
            });
        }
    }
}