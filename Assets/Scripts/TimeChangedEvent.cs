using UnityEngine;
using UnityEngine.Events;

public class TimeChangedEvent : MonoBehaviour
{
    public float customTime = 9f;
    public UnityEvent<float> timeChanged;
    public UnityEvent<float> timeChangedNormalized;
    public UnityEvent customTimePassed;
    public UnityEvent tenSecondsPassed;

    private bool _customTimeSometin;

    private void Start()
    {
        GameManager.Instance.TenSecondsPassed += tenSecondsPassed.Invoke;
        GameManager.Instance.TenSecondsPassed += () => _customTimeSometin = false;
    }

    private void Update()
    {
        timeChanged.Invoke(GameManager.CurrentTime);
        timeChangedNormalized.Invoke(GameManager.CurrentTime / 10f);

        if (!_customTimeSometin && GameManager.CurrentTime > customTime)
        {
            _customTimeSometin = true;
            customTimePassed.Invoke();
        }
    }
}