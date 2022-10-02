using System.Collections;
using MPO;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [ColorUsage(false, true)]
    public Color a, b;
    public float speed = 1f;
    public ColorPropertyOverride colorProperty;

    public void Do() => StartCoroutine(DoRoutine());
    private IEnumerator DoRoutine()
    {
        float t = 0f;
        while (this != null)
        {
            t += Time.deltaTime * speed;
            colorProperty.Value = Color.Lerp(a, b, Mathf.PingPong(t, 1f));
            yield return null;
        }
    }
}