using UnityEngine;

public class HandAreaSwitcher : MonoBehaviour
{
    public static HandAreaSwitcher Instance;

    public GameObject cardHandArea;
    public GameObject pieceHandArea;

    private bool showingPieceHand = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateDisplay();
    }

    public void ToggleHandArea()
    {
        showingPieceHand = !showingPieceHand;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        pieceHandArea.SetActive(showingPieceHand);
        cardHandArea.SetActive(!showingPieceHand);

        Debug.Log("åªç›ï\é¶: " + (showingPieceHand ? "éËãÓ" : "éËéD"));
    }
}
