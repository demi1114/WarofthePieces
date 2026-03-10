using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("Player UI")]
    public TextMeshProUGUI deckCountText;
    public TextMeshProUGUI handCountText;
    public TextMeshProUGUI reserveCountText;

    [Header("Enemy UI")]
    public TextMeshProUGUI enemyDeckCountText;
    public TextMeshProUGUI enemyHandCountText;
    public TextMeshProUGUI enemyReserveCountText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RefreshUI();

        // ReserveïœçXéûçXêV
        ReserveManager.Instance.OnReserveChanged += RefreshUI;
    }

    public void RefreshUI()
    {
        UpdatePlayerUI();
        UpdateEnemyUI();
    }

    void UpdatePlayerUI()
    {
        int owner = 0;

        deckCountText.text =
            DeckManager.Instance.GetDeckCount(owner).ToString();

        handCountText.text =
            DeckManager.Instance.GetHandCount(owner).ToString();

        reserveCountText.text =
            ReserveManager.Instance.GetReserveCount(owner).ToString();
    }

    void UpdateEnemyUI()
    {
        int owner = 1;

        enemyDeckCountText.text =
            DeckManager.Instance.GetDeckCount(owner).ToString();

        enemyHandCountText.text =
            DeckManager.Instance.GetHandCount(owner).ToString();

        enemyReserveCountText.text =
            ReserveManager.Instance.GetReserveCount(owner).ToString();
    }
}