using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EnemyControl : CharacterControl, ISwordTarget
{
    public float pursueDistance = 4f;
    public float circleSpeed = 0.5f;
    public float knockbackRecoverySpeed = 3f;
    public float knockbackForce = 5f;
    public GameObject top, bottom;
    public Slingshot slingshot;
    public float shootDelay = 1f;
    public GameObject gibletSeed;
    public Transform gibletVolume;

    public UnityEvent onStuck;
    [FormerlySerializedAs("onRelease")]
    public UnityEvent onKill;

    private bool _killed;
    private Vector3 _knockbackVelocity;
    private bool _stuck;
    private float _timeSinceShoot;

    private void Start()
    {
        gibletSeed.SetActive(false);
        StartCoroutine(ShootRoutine());
    }

    private void OnDisable()
    {
        Movement = Vector3.zero;
    }

    private bool _canShoot;
    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            if (_canShoot && !_killed)
                yield return slingshot.Shoot();
            yield return new WaitForSeconds(shootDelay); 
        }
    }

    private void Update()
    {
        _canShoot = false;
        if (_stuck)
        {
            Movement = transform.right;
            LookDirection = -transform.right;
        }
        else
        {
            var player = Player.Instance;
            if (player == null)
                return;
            if (Vector3.Distance(player.transform.position, transform.position) > pursueDistance)
            {
                Movement = (player.transform.position - transform.position).normalized;
                LookDirection = Movement;
            }
            else
            {
                _canShoot = true;
                
                Movement = Vector3.Cross((player.transform.position - transform.position).normalized, Vector3.up) *
                           circleSpeed;
                LookDirection = (player.transform.position - transform.position).normalized;
            }
        }

        _knockbackVelocity = Vector3.Lerp(_knockbackVelocity, Vector3.zero, knockbackRecoverySpeed * Time.deltaTime);
        Movement += _knockbackVelocity;
    }

    public void OnStuck(Sword sword)
    {
        _stuck = true;
        _knockbackVelocity += Vector3.ProjectOnPlane
        (
            vector: Vector3.Normalize(transform.position - sword.transform.position), 
            planeNormal: Vector3.up
        ) * knockbackForce;
        onStuck.Invoke();
    }

    public void OnRelease(Sword sword)
    {
        Kill();
    }

    private Coroutine _killRoutine;
    public void Kill(bool explode = false)
    {
        if (_killRoutine != null)
            return;
        if (_killed)
            return;

        _killed = true;
        EnemyManager.Instance.EnemyCount--;
        onKill?.Invoke();


        if (explode)
        {
            var bodySize = gibletVolume.localScale;
            
            var gibletSize = gibletSeed.transform.GetChild(0).localScale;
            var numGiblets = new Vector3Int((int)(bodySize.x/gibletSize.x),(int)(bodySize.y/gibletSize.y), (int)(bodySize.z/gibletSize.z));
            
            for (int i = 0; i < numGiblets.x; ++i)
            {
                for (int j = 0; j < numGiblets.y; ++j)
                {
                    for (int k = 0; k < numGiblets.z; ++k)
                    {
                        var p = new Vector3((float)i / numGiblets.x, (float)j / numGiblets.y, (float)k / numGiblets.z);
                        p += Vector3.one * -0.5f;
                        p = gibletVolume.localToWorldMatrix.MultiplyPoint3x4(p);
                        
                        var newGiblet = Instantiate(gibletSeed, p + Vector3.up, gibletVolume.rotation);
                        newGiblet.SetActive(true);
                    }
                }
            }
        }
        _killRoutine = StartCoroutine(KillRoutine());
        Destroy(slingshot.gameObject);
        if (Sword.Instance.transform.IsChildOf(transform))
        {
            Sword.Instance.Drop();
        }

        GameManager.Instance.Score += 1;
    }

    private IEnumerator KillRoutine()
    {
        top.transform.SetParent(null);
        top.GetComponent<Collider>().enabled = true;
        var trb = top.GetComponent<Rigidbody>();
        trb.isKinematic = false;

        bottom.transform.SetParent(null);
        bottom.GetComponent<Collider>().enabled = true;
        var brb = bottom.GetComponent<Rigidbody>();
        brb.isKinematic = false;
        yield return null;
        trb.AddExplosionForce(300f, transform.position + Vector3.back, 10f, 50f);
        brb.AddExplosionForce(300f, transform.position + Vector3.back, 10f, 50f);

        Destroy(GetComponent<EnemyMotor>());

        foreach (var c in GetComponentsInChildren<Collider>())
        {
            if (!c.transform.IsChildOf(Sword.Instance.transform))
                Destroy(c);
        }

        Destroy(GetComponent<Rigidbody>());
        
        yield return new WaitForSeconds(5f);

        while (transform.localScale.sqrMagnitude > 0.1)
        {
            yield return null;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 2f * Time.deltaTime);
            top.transform.localScale = Vector3.Lerp(top.transform.localScale, Vector3.zero, 2f * Time.deltaTime);
            bottom.transform.localScale = Vector3.Lerp(bottom.transform.localScale, Vector3.zero, 2f * Time.deltaTime);
        }

        if (Sword.Instance.transform.IsChildOf(transform))
        {
            Sword.Instance.Drop();
        }
        
        Destroy(top.gameObject);
        Destroy(bottom.gameObject);
        Destroy(gameObject);
    }
}