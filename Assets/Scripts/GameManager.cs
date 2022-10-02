using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CanvasGroup fade;
    public GameOver gameOver;

    public int Score;
    
    protected float _timer;

    private void Awake()
    {
        Instance = this;

        Player.Killed += (s) =>
        {
            if (gameOver)
                gameOver.Show(s);
        };
    }

    private IEnumerator Start()
    {
        yield return null;
        yield return Tween(1f, f => fade.alpha = 1f - f);
        fade.blocksRaycasts = false;
        fade.interactable = false;
    }

    private void Update()
    {
        Time.timeScale = Input.GetKey(KeyCode.LeftShift) ? 0.5f : 1f;

        if (RoundManager.Instance.CurrentRound > 0)
            _timer += Time.deltaTime;

        if (_timer >= 10f)
        {
            _timer = 0f;
            TenSecondsPassed();
        }
    }

    public void Restart()
    {
        fade.blocksRaycasts = true;
        fade.interactable = true;

        StartCoroutine(Tween(1f, f => fade.alpha = f));

        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }

    public static float CurrentTime => Instance?._timer ?? 0f;
    public Action TenSecondsPassed;
    
    private IEnumerator Tween(float duration, Action<float> onStep)
    {
        float t = 0f;
        while (!Mathf.Approximately(t, 1f))
        {
            t = Mathf.Clamp01(t + Time.deltaTime / duration);
            onStep(t);
            yield return null;
        }
    }
}