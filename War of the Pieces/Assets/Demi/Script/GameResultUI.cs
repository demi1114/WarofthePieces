using TMPro;
using UnityEngine;

public class GameResultUI : MonoBehaviour
{
    public static GameResultUI Instance;

    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    void Awake()
    {
        Instance = this;
        resultPanel.SetActive(false);
    }

    public void ShowVictory()
    {
        resultPanel.SetActive(true);
        resultText.text = "VICTORY";
    }

    public void ShowDefeat()
    {
        resultPanel.SetActive(true);
        resultText.text = "DEFEAT";
    }
}