using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public IEnumerator Start()
    {
        canvasGroup.blocksRaycasts = true;
        yield return Tween(1f, t => canvasGroup.alpha = 1f - t);
        canvasGroup.blocksRaycasts = false;
    }

    public void Play()
    {
        StartCoroutine(PlayRoutine());
    }
    
    public void Quit()
    {
        Application.Quit();
    }
    
    private IEnumerator PlayRoutine()
    {
        canvasGroup.blocksRaycasts = true;
        yield return Tween(1f, t => canvasGroup.alpha = t);
        yield return SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }
    
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
