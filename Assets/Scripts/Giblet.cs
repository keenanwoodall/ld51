using System;
using System.Collections;
using UnityEngine;

public class Giblet : MonoBehaviour
{
    public Rigidbody rb;

    private float spawnTime;
    private void Awake()
    {
        spawnTime = Time.time;
        Destroy(gameObject, 10f);
    }

    private IEnumerator OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 16)
            yield break;
        rb.isKinematic = true;
        rb.detectCollisions = false;

        yield return Tween(1f, t =>
        {
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
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
}
