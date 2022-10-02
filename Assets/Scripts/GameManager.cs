using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    protected float _timer;

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= 10f)
        {
            _timer = 0f;
            TenSecondsPassed();
        }
    }

    public static float CurrentTime => _instance?._timer ?? 0f;
    public static Action TenSecondsPassed;
}