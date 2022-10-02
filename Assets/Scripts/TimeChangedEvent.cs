using UnityEngine;
using UnityEngine.Events;

public class TimeChangedEvent : MonoBehaviour
{
    public UnityEvent<float> timeChanged;
    public UnityEvent<float> timeChangedNormalized;
    public UnityEvent tenSecondsPassed;

    private void Awake()
    {
        GameManager.Instance.TenSecondsPassed += tenSecondsPassed.Invoke;
    }

    private void Update()
    {
        timeChanged.Invoke(GameManager.CurrentTime);
        timeChangedNormalized.Invoke(GameManager.CurrentTime / 10f);
    }
}