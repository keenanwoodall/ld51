using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public float launchHeight = 1f;
    public float landDuration = 1f;
    public float fallStretch = 1f;
    public float landStretch = -0.3f;
    public float squashDuration = 0.5f;
    [Space]
    public EnemyControl seed;
    public Vector2 spawnPerimeter = new Vector2(25f, 25f);
    public Vector2 landPerimeter = new Vector2(15f, 15f);

    public int EnemyCount;

    private Vector3[] _spawnPoints;
    private Vector3[] _landPoints;

    private void Awake()
    {
        Instance = this;
        seed.gameObject.SetActive(false);
        _spawnPoints = Enumerable.Repeat<Vector3>(default, 4).ToArray();
        _landPoints = Enumerable.Repeat<Vector3>(default, 4).ToArray();
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.E))
                Spawn();
        }
    }

    public Coroutine Spawn()
    {
        EnemyCount++;
        return StartCoroutine(SpawnRoutine());
    }
    
    private IEnumerator SpawnRoutine()
    {
        var randomSpawn = Random.value;
        var spawnPoint = GetSpawnPoint(randomSpawn);
        var enemy = Instantiate(seed, spawnPoint, Quaternion.LookRotation(-spawnPoint));
        enemy.gameObject.SetActive(true);
        var enemyMotor = enemy.GetComponent<EnemyMotor>();
        enemyMotor.rb.isKinematic = true;
        enemy.enabled = false;
        enemyMotor.enabled = false;

        var landPoint = GetLandPoint(randomSpawn);
        Debug.DrawLine(spawnPoint, landPoint, Color.cyan, 3f);
        yield return Tween(landDuration, t =>
        {
            var p = Vector3.Lerp(spawnPoint, landPoint, t);
            var heightT = Mathf.Sin(t * Mathf.PI);
            p.y = Mathf.Lerp(0f, launchHeight, heightT);
            enemyMotor.SetSquash(Mathf.Lerp(0f, fallStretch, 1f - heightT));
            enemyMotor.transform.position = p;
        });
        
        yield return Tween(squashDuration, t =>
        {
            var s = Mathf.Sin(t * Mathf.PI);
            enemyMotor.SetSquash(Mathf.Lerp(0f, landStretch, s));
        });
        
        enemyMotor.rb.isKinematic = false;
        enemy.enabled = true;
        enemyMotor.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(spawnPerimeter.x, 0f, spawnPerimeter.y));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(landPerimeter.x, 0f, landPerimeter.y));
    }

    private Vector3 GetSpawnPoint(float t)
    {
        UpdatePerimeter(_spawnPoints, spawnPerimeter);
        return GetPoint(t, _spawnPoints);
    }
    
    private Vector3 GetLandPoint(float t)
    {
        UpdatePerimeter(_landPoints, landPerimeter);
        return GetPoint(t, _landPoints);
    }

    private void UpdatePerimeter(Vector3[] points, Vector2 perimeter)
    {
        var halfPerimeter = perimeter * 0.5f;
        points[0] = new Vector3(halfPerimeter.x, 0f, halfPerimeter.y);
        points[1] = new Vector3(-halfPerimeter.x, 0f, halfPerimeter.y);
        points[2] = new Vector3(-halfPerimeter.x, 0f, -halfPerimeter.y);
        points[3] = new Vector3(halfPerimeter.x, 0f, -halfPerimeter.y);
    }
    
    private Vector3 GetPoint(float t, Vector3[] points)
    {
        t %= 1f;
        
        if (t <= 0f)
            return points[0];
        if (t >= 1f)
            return points[^1];
        
        var i1 = Mathf.FloorToInt(t * (points.Length - 1));
        var i2 = Mathf.CeilToInt(t * (points.Length - 1));

        var t1 = (float)i1 / (points.Length - 1); 
        var t2 = (float)i2 / (points.Length - 1);

        return Vector3.Lerp(points[i1], points[i2], Mathf.InverseLerp(t1, t2, t));
    }
    
    private IEnumerator Tween(float duration, Action<float> onStep)
    {
        float t = 0f;
        while (!Mathf.Approximately(t, 1f))
        {
            t = Mathf.Clamp01(t + Time.deltaTime / duration);
            onStep(t);
            yield return null;
        }
    }
}