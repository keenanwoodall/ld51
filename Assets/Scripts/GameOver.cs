using System;
using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public TMP_Text highscoreText, currentScoreText, causeOfDeathText;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(string reason)
    {
        gameObject.SetActive(true);
        var highscore = PlayerPrefs.GetInt("highscore", 0);
        var currentScore = GameManager.Instance.Score;

        if (currentScore > highscore)
        {
            PlayerPrefs.SetInt("highscore", currentScore);
            highscoreText.text = $"<b>{currentScore}</b> (New)";
        }
        else
            highscoreText.text = $"<b>{highscore}</b>";

        currentScoreText.text = $"Bye bye to <b>{currentScore}</b> piggies :(";
        
        causeOfDeathText.text = $"<b>{reason}</b>";
    }
}