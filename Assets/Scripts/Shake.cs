using UnityEngine;

public class Shake : MonoBehaviour
{
    public float factor;
    public float speed = 20f;
    public Vector3 angle = new(0, 0, 10);
    private float t = 0f;
    private void Update()
    {
        t += Time.deltaTime * speed;
        transform.localRotation = Quaternion.Euler(Mathf.Sin(t) * angle * factor);
    }

    public void SetFactor(float vaal)
    {
        factor = vaal;
    }
}