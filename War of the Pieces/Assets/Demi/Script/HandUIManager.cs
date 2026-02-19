using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandUIManager : MonoBehaviour
{
    public static HandUIManager Instance;

    public Transform handArea;
    public GameObject handButtonPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void RefreshHand()
    {
        foreach (Transform child in handArea)
        {
            Destroy(child.gameObject);
        }

        foreach (PieceData piece in BoardManager.Instance.playerHand)
        {
            GameObject btn = Instantiate(handButtonPrefab, handArea);

            btn.GetComponentInChildren<TMP_Text>().text = piece.pieceName;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                BoardManager.Instance.SelectPlacePiece(piece);
            });
        }
    }
}
