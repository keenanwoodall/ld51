using System;
using UnityEngine;

public class LightningSpawner : MonoBehaviour
{
    public static LightningSpawner Instance;
    public static Action LightningSpawned;
    
    public Lightning seed;

    private void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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