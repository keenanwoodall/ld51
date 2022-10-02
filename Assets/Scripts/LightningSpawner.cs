using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LightningSpawner : MonoBehaviour
{
    public static LightningSpawner Instance;
    public static Action LightningSpawned;
    
    public Lightning seed;

    private void Awake()
    {
        Instance = this;
        GameManager.TenSecondsPassed += OnTenSecondsPassed;
    }

    private void OnTenSecondsPassed()
    {
        var sword = Sword.Instance;
        var target = sword.wobbleRoot.position;
        Spawn().Strike(target, target + new Vector3(Random.Range(-5f, 5f), 10f, Random.Range(-5f, 5f)), 0);
    }

    public void Update()
    {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.Space))
        {
            var ray = PlayerCamera.Instance.camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Spawn().Strike(hit.point, hit.point + transform.position, 0);
            }
        }
    }

    public static Lightning Spawn()
    {
        LightningSpawned.Invoke();
        return Instantiate(Instance.seed);
    }
}