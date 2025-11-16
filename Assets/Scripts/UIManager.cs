using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI populationText;
    public TextMeshProUGUI jokersText;
    public TextMeshProUGUI nextBuildingsText;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalPopulationText;
    public Button restartButton;

    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    public void UpdatePopulation(int population)
    {
        if (populationText != null)
        {
            populationText.text = $"Population: {population}";
        }
    }

    public void UpdateJokers(int jokers)
    {
        if (jokersText != null)
        {
            jokersText.text = $"Jokers: {jokers}";
        }
    }

    public void UpdateNextBuildings(List<string> buildingsInfo)
    {
        if (nextBuildingsText != null)
        {
            nextBuildingsText.text = "Next:\n" + string.Join("\n", buildingsInfo);
        }
    }

    public void ShowGameOver(int finalScore, int finalPopulation)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {finalScore}";
        }

        if (finalPopulationText != null)
        {
            finalPopulationText.text = $"Total Population: {finalPopulation}";
        }
    }

    void OnRestartClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
}
