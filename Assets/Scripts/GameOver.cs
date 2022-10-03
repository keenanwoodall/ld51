using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameOver : MonoBehaviour
{
    public TMP_Text highscoreText, currentScoreText, causeOfDeathText;
    public UnityEvent onhighscore;

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
            onhighscore.Invoke();
            PlayerPrefs.SetInt("highscore", currentScore);
            highscoreText.text = $"<b>{currentScore}</b> (New)";
        }
        else
            highscoreText.text = $"<b>{highscore}</b>";

        currentScoreText.text = $"Bye bye to <b>{currentScore}</b> piggies :(";
        
        causeOfDeathText.text = $"<b>{reason}</b>";
    }
}