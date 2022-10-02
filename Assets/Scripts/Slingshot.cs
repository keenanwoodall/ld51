using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Slingshot : MonoBehaviour
{
    public LineRenderer tetherA;
    public LineRenderer tetherB;
    public Transform pouch;
    public Transform pouchPull;
    public Transform pouchRest;
    public Ball seed;
    [Space] 
    public float pullDuration = 0.5f;
    public AnimationCurve pullCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public float releaseDuration = 0.5f;
    public float releaseTime = 0.3f;
    public AnimationCurve releaseCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private static Vector3 _seedScale;

    private void Awake()
    {
        if (_seedScale.sqrMagnitude < 0.001f)
            _seedScale = seed.transform.localScale;
        seed.transform.localScale = Vector3.zero;
    }

    private void OnValidate() => UpdateTethers();
    private void Update() => UpdateTethers();

    public void UpdateTethers()
    {
        if (tetherA == null || tetherB == null || pouch == null)
            return;
        
        tetherA.SetPosition(0, tetherA.transform.position);
        tetherA.SetPosition(1, pouch.position);
        tetherB.SetPosition(0, tetherB.transform.position);
        tetherB.SetPosition(1, pouch.position);
    }

    public Coroutine Shoot()
    {
        if (_shootRoutine != null)
            return null;
        return _shootRoutine = StartCoroutine(ShootRoutine());
    }
    
    private Coroutine _shootRoutine;
    private IEnumerator ShootRoutine()
    {
        yield return Tween(pullDuration, t =>
        {
            pouch.transform.position =
                Vector3.LerpUnclamped(pouchRest.position, pouchPull.position, pullCurve.Evaluate(t));
            seed.transform.localScale = Vector3.Lerp(Vector3.zero, _seedScale, t);
        });
        yield return new WaitForSeconds(Random.value);
        bool released = false;
        yield return Tween(releaseDuration, t =>
        {
            pouch.transform.position =
                Vector3.LerpUnclamped(pouchPull.position, pouchRest.position, releaseCurve.Evaluate(t));
            if (!released && t > releaseTime)
            {
                released = true;
                var newSeed = Instantiate(seed, seed.transform.position, seed.transform.rotation);
                newSeed.outline.gameObject.SetActive(true);
                newSeed.rb.isKinematic = false;
                newSeed.rb.velocity = Vector3.Scale(transform.forward, new Vector3(1f, 0f, 1f)) * 5f;
                newSeed.collider.enabled = true;
                seed.transform.localScale = Vector3.zero;
                
                Destroy(newSeed.gameObject, 5f);
            }
        });
        _shootRoutine = null;
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