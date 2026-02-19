using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUIManager : MonoBehaviour
{
    public static CardUIManager Instance;

    [Header("Card UI")]
    public Transform cardHandArea;          // カード専用エリア
    public GameObject cardButtonPrefab;     // カードボタンPrefab

    private void Awake()
    {
        Instance = this;
    }

    public void RefreshHand(List<CardData> hand)
    {
        // 既存カードUI削除
        foreach (Transform child in cardHandArea)
        {
            Destroy(child.gameObject);
        }

        // 手札分だけ生成
        for (int i = 0; i < hand.Count; i++)
        {
            CardData card = hand[i];
            int index = i; // ★ ローカルコピー（重要）

            GameObject btn = Instantiate(cardButtonPrefab, cardHandArea);

            btn.GetComponentInChildren<TMP_Text>().text = card.cardName;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                CardUseManager.Instance.StartCardUse(card, index);
            });
        }
    }
}
