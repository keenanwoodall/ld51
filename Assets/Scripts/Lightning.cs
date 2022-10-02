using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Lightning : MonoBehaviour
{
    public float growDuration = 0.3f;
    public float flashDuration = 0.3f;
    public float lightIntensity = 20;
    public AnimationCurve flashCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [Space]
    public int pointCount = 50;
    public float frequency = 1f;
    public float magnitude = 1f;
    public float persistance = 0.5f;
    public float lacunarity = 2f;
    public int octaves = 2;
    public new Light light;
    public AnimationCurve noiseStrength = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    public LineRenderer lineRenderer;

    private Coroutine _strikeRoutine;
    private Vector3[] _points;
    private Gradient _gradient;

    // private void Update()
    // {
    //     if (_points == null || _points.Length != pointCount)
    //     {
    //         lineRenderer.positionCount = pointCount;
    //         _points = new Vector3[pointCount];
    //     }
    //
    //     for (int i = 0; i < pointCount; i++)
    //     {
    //         var t = i / (pointCount - 1f);
    //         var point = Vector3.Lerp(transform.position, Vector3.zero, t);
    //         point += Noisy.Noise3D(t, frequency, magnitude, persistance, lacunarity, octaves) *
    //                  noiseStrength.Evaluate(t);
    //         _points[i] = point;
    //     }
    //     
    //     lineRenderer.SetPositions(_points);
    // }

    public Coroutine Strike(Vector3 target, Vector3 from, int branches)
    {
        if (_strikeRoutine != null)
            StopCoroutine(_strikeRoutine);
        return _strikeRoutine = StartCoroutine(StrikeRoutine(target, from));
    }

    private IEnumerator StrikeRoutine(Vector3 target, Vector3 from)
    {
        if (_points == null || _points.Length != pointCount)
        {
            lineRenderer.positionCount = pointCount;
            _points = new Vector3[pointCount];
        }

        var seed = Random.value * 100f;
        
        for (int i = 0; i < pointCount; i++)
        {
            var t = i / (pointCount - 1f);
            var point = Vector3.Lerp(from, target, t);
            point += Noisy.Noise3D(t + seed, frequency, magnitude, persistance, lacunarity, octaves) * noiseStrength.Evaluate(t);
            _points[i] = point;
        }
        
        lineRenderer.SetPositions(_points);

        var alphaKeys  = new[]
        {
            new GradientAlphaKey(flashCurve.Evaluate(0f), 0f),
            new GradientAlphaKey(0f, 0f)
        };
        
        _gradient = new Gradient();

        light.gameObject.SetActive(true);
        
        // grow
        yield return Tween(growDuration, t =>
        {
            alphaKeys[0].time = t;
            alphaKeys[1].time = t + 0.2f;
            light.transform.position = GetPoint(t) + new Vector3(0f, 1f, 0f);
            _gradient.alphaKeys = alphaKeys;
            lineRenderer.colorGradient = _gradient;
        });

        var hits = Physics.SphereCastAll(target, 3f, Vector3.forward, 0f, ~(1 << LayerMask.NameToLayer("Sword")) & ~0);
        foreach (var enemy in hits.Select(h => h.transform.GetComponent<EnemyControl>()))
        {
            if (enemy)
                enemy.Kill();
        }

        if (Sword.Instance.transform.GetComponentInParent<Player>() != null)
        {
            Sword.Instance.Drop();
            Player.Instance.Kill();
        }

        foreach (var rb in hits.Select(h => h.transform.GetComponent<Rigidbody>()))
        {
            if (rb && !rb.isKinematic)
                rb.AddExplosionForce(500f, target, 30f, 100);
        }

        // flash
        yield return Tween(flashDuration, t =>
        {
            alphaKeys[0].alpha = flashCurve.Evaluate(t);
            light.intensity = lightIntensity * flashCurve.Evaluate(t);
            _gradient.alphaKeys = alphaKeys;
            lineRenderer.colorGradient = _gradient;
        });
        
        Destroy(gameObject);
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

    private Vector3 GetPoint(float t)
    {
        if (t <= 0f)
            return _points[0];
        if (t >= 1f)
            return _points[^1];
        
        var i1 = Mathf.FloorToInt(t * (_points.Length - 1));
        var i2 = Mathf.CeilToInt(t * (_points.Length - 1));

        var t1 = (float)i1 / (_points.Length - 1); 
        var t2 = (float)i2 / (_points.Length - 1);

        return Vector3.Lerp(_points[i1], _points[i2], Mathf.InverseLerp(t1, t2, t));
    }
}