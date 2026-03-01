using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReserveUIManager : MonoBehaviour
{
    public int owner = 0; // ï\é¶ëŒè€
    public Transform handArea;
    public GameObject handButtonPrefab;

    private void Start()
    {
        if (ReserveManager.Instance != null)
            ReserveManager.Instance.OnReserveChanged += RefreshUI;

        RefreshUI();
    }

    private void OnDestroy()
    {
        if (ReserveManager.Instance != null)
            ReserveManager.Instance.OnReserveChanged -= RefreshUI;
    }

    public void RefreshUI()
    {
        if (handArea == null || handButtonPrefab == null) return;

        foreach (Transform child in handArea)
            Destroy(child.gameObject);

        var reserve = ReserveManager.Instance.GetReserve(owner);

        foreach (PieceData piece in reserve)
        {
            GameObject btn = Instantiate(handButtonPrefab, handArea);

            btn.GetComponentInChildren<TMP_Text>().text = piece.pieceName;

            btn.GetComponent<Button>().onClick.RemoveAllListeners();

            if (owner == 0)
            {
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    BoardManager.Instance.SelectPlacePiece(piece);
                });
            }
        }
    }
}