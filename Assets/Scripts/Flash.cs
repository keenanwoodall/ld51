using System.Collections;
using MPO;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [ColorUsage(false, true)]
    public Color a, b;
    public float speed = 1f;
    public ColorPropertyOverride colorProperty;

    public void OneShot() => StartCoroutine(DoRoutine(1));
    public void LoopingShot() => StartCoroutine(DoRoutine(-1));
    private IEnumerator DoRoutine(int count)
    {
        int currentCount = 0;
        float t = 0f;
        while (this != null && count <= 0 || currentCount < count)
        {
            t += Time.deltaTime * speed;
            if (t >= 2f)
            {
                currentCount++;
                t %= 2f;
            }

            colorProperty.Value = Color.Lerp(a, b, Mathf.PingPong(t, 1f));
            yield return null;
        }
    }
}