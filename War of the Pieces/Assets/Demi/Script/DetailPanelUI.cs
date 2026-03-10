using TMPro;
using UnityEngine;

public class DetailPanelUI : MonoBehaviour
{
    public static DetailPanelUI Instance;

    [Header("Panel")]
    public GameObject panel;

    [Header("Common")]
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    [Header("Piece Info")]
    public GameObject pieceArea;
    public TMP_Text powerText;
    public TMP_Text raceText;
    public TMP_Text attributeText;

    [Header("Card Info")]
    public GameObject cardArea;
    public TMP_Text categoryText;

    void Awake()
    {
        Instance = this;
        Hide();
    }

    public void ShowPiece(Piece piece)
    {
        if (piece == null) return;

        panel.SetActive(true);

        pieceArea.SetActive(true);
        cardArea.SetActive(false);

        nameText.text = piece.data.pieceName;
        descriptionText.text = piece.data.description;

        powerText.text = piece.CurrentPower.ToString();
        raceText.text = piece.data.race.ToString();
        attributeText.text = piece.data.attribute.ToString();
    }
    public void ShowReservePiece(PieceData pieceData)
    {
        if (pieceData == null) return;

        panel.SetActive(true);

        pieceArea.SetActive(true);
        cardArea.SetActive(false);

        nameText.text = pieceData.pieceName;
        descriptionText.text = pieceData.description;

        powerText.text = pieceData.basePower.ToString();
        raceText.text = pieceData.race.ToString();
        attributeText.text = pieceData.attribute.ToString();
    }
    public void ShowCard(CardData card)
    {
        if (card == null) return;

        panel.SetActive(true);

        pieceArea.SetActive(false);
        cardArea.SetActive(true);

        nameText.text = card.cardName;
        descriptionText.text = card.description;

        categoryText.text = card.category.ToString();
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}