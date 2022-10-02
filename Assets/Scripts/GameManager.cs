using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public CanvasGroup fade;
    public GameObject dead;
    
    protected float _timer;

    private void Awake()
    {
        _instance = this;
        dead.SetActive(false);

        Player.Killed += () =>
        {
            if (dead)
                dead.SetActive(true);
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

    public static float CurrentTime => _instance?._timer ?? 0f;
    public static Action TenSecondsPassed;
    
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