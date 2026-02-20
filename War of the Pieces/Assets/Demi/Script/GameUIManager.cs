using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    public TMP_Text turnText;
    public TMP_Text moveText;
    public TMP_Text playerCountText;
    public TMP_Text enemyCountText;
    public TMP_Text battleText;
    public TMP_Text predictionText;

    private void Awake() => Instance = this;

    public void UpdateTurn(bool isPlayerTurn) => turnText.text = isPlayerTurn ? "Player Turn" : "Enemy Turn";
    public void UpdateMoves(int remainingMoves) => moveText.text = $"Moves: {remainingMoves}";
    public void UpdatePieceCounts(int playerCount, int enemyCount)
        => (playerCountText.text, enemyCountText.text) = ($"Player Pieces: {playerCount}", $"Enemy Pieces: {enemyCount}");

    public void ShowBattleResult(int playerCount, int typeBonus, int enemyCount, bool playerWins)
    {
        string result = playerWins ? "Player Wins!" : "Player Loses!";
        battleText.text = $"Battle!\nPlayer: {playerCount} (+{typeBonus})\nEnemy: {enemyCount}\n{result}";
    }

    public void ShowPrediction(bool willWin, bool isDraw)
    {
        if (isDraw) predictionText.text = "Even (Attacker Wins)";
        else predictionText.text = willWin ? "Win" : "Lose";
    }

    public void ClearPrediction() => predictionText.text = "";
}