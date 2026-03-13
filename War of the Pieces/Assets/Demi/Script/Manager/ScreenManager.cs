using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance;

    [Header("Screens")]
    public ScreenBase homeScreen;
    public ScreenBase SoroBattleScreen;
    public ScreenBase MultiBattleScreen;
    public ScreenBase CardScreen;
    public ScreenBase deckBuilderScreen;
    public ScreenBase cardListScreen;
    public ScreenBase pieceListScreen;
    public ScreenBase gachaScreen;
    public ScreenBase SettingScreen;
    public ScreenBase UserProfScreen;
    public ScreenBase UserSettingScreen;
    public ScreenBase CreditScreen;


    private ScreenBase currentScreen;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ShowScreen(homeScreen);
    }

    public void ShowHome()
    {
        ShowScreen(homeScreen);
    }

    public void ShowSoroBattle()
    {
        ShowScreen(SoroBattleScreen);
    }

    public void ShowMultiBattle()
    {
        ShowScreen(MultiBattleScreen);
    }

    public void ShowCardScreen()
    {
        ShowScreen(CardScreen);
    }

    public void ShowDeckBuilder()
    {
        ShowScreen(deckBuilderScreen);
    }

    public void ShowCardList()
    {
        ShowScreen(cardListScreen);
    }

    public void ShowPieceList()
    {
        ShowScreen(pieceListScreen);
    }

    public void ShowGacha()
    {
        ShowScreen(gachaScreen);
    }

    public void ShowSetting()
    {
        ShowScreen(SettingScreen);
    }

    public void ShowUserProf()
    {
        ShowScreen(UserProfScreen);
    }

    public void ShowUserSetting()
    {
        ShowScreen(UserSettingScreen);
    }

    public void ShowCredit()
    {
        ShowScreen(CreditScreen);
    }



    public void StartBattle()
    {
        DeckBuilderManager.Instance.SaveCurrentDeck();
        SceneManager.LoadScene("DemiScene");
    }

    void ShowScreen(ScreenBase screen)
    {
        if (screen == currentScreen) return;

        if (currentScreen != null)
        {
            currentScreen.OnClose();
            currentScreen.Hide();
        }

        currentScreen = screen;

        currentScreen.Show();
        currentScreen.OnOpen();
    }
}