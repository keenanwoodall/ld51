using System;
using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public TMP_Text highscoreText, currentScoreText;

    private void OnEnable()
    {
        var highscore = PlayerPrefs.GetInt("highscore", 0);
        var currentScore = GameManager.Instance.Score;

        if (currentScore > highscore)
        {
            PlayerPrefs.SetInt("highscore", currentScore);
            highscoreText.text = $"{currentScore} (New)";
        }
        else
            highscoreText.text = $"{highscore}";

        currentScoreText.text = $"Bye bye to {currentScore} piggies :(";
    }
}