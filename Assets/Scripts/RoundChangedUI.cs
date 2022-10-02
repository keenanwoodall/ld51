using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class RoundChangedUI : MonoBehaviour
{
    public TMP_Text text;

    private string format;

    private void Start()
    {
        format = text.text;
        RoundManager.Instance.RoundChanged += RoundChanged;
        transform.localScale = default;
    }

    private void RoundChanged(int round)
    {
        StartCoroutine(RoundRoutine(round));
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.time * 40f) * 5f);
    }

    private IEnumerator RoundRoutine(int round)
    {
        text.text = string.Format(format, round);
        float t = 0f;
        yield return Tween(0.5f, t =>
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Mathf.SmoothStep(0f, 1f, t));
        });
        yield return new WaitForSeconds(1f);
        yield return Tween(0.5f, t =>
        {
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, Mathf.SmoothStep(0f, 1f, t));
        });
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
