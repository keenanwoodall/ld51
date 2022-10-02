using System;
using System.Collections;
using UnityEngine;
using Random =  UnityEngine.Random;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;
    public Action<int> RoundChanged;
    public int CurrentRound;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        while (Sword.Instance.State == Sword.SwordState.Idle)
        {
            yield return null;
        }
        
        while (true)
        {
            RoundChanged?.Invoke(++CurrentRound);
            for (int i = 0; i < Mathf.Max((CurrentRound - 1) * 2, 1); i++)
            {
                yield return new WaitForSeconds(Random.value);
                EnemyManager.Instance.Spawn();
            }

            while (EnemyManager.Instance.EnemyCount > 0)
                yield return null;
        }
    }
}
