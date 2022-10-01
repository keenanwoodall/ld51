using System.Collections;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    public int pointCount = 50;
    public float growDuration = 0.3f;
    public float flashDuration = 0.3f;
    public LineRenderer lineRenderer;

    private Coroutine _strikeRoutine;
    private Vector3[] _points;

    public Coroutine Strike(Vector3 target, Vector3 from)
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

        for (int i = 0; i < pointCount; i++)
        {
            var t = i / (pointCount - 1f);
            var point = Vector3.Lerp(from, target, t);
            
            _points[i] = point;
        }
        
        lineRenderer.SetPositions(_points);

        yield break;
    }
}