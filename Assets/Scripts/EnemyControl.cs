using System;
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

    public UnityEvent onStuck;
    [FormerlySerializedAs("onRelease")]
    public UnityEvent onKill;
    
    private Vector3 _knockbackVelocity;
    private bool _stuck;

    private void OnDisable()
    {
        Movement = Vector3.zero;
    }

    private void Update()
    {
        if (_stuck)
        {
            Movement = transform.right;
            LookDirection = -transform.right;
        }
        else
        {
            var player = Player.Instance;
            if (Vector3.Distance(player.transform.position, transform.position) > pursueDistance)
            {
                Movement = (player.transform.position - transform.position).normalized;
                LookDirection = Movement;
            }
            else
            {
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
        onKill?.Invoke();
        Kill();
    }

    private Coroutine _killRoutine;
    public void Kill()
    {
        if (_killRoutine != null)
            return;
        _killRoutine = StartCoroutine(KillRoutine());
        if (Sword.Instance.CurrentSwordTarget == this)
        {
            Sword.Instance.Drop();
        }
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
            Destroy(c);
        Destroy(GetComponent<Rigidbody>());
        
        yield return new WaitForSeconds(5f);

        while (transform.localScale.sqrMagnitude > 0.1)
        {
            yield return null;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 2f * Time.deltaTime);
            top.transform.localScale = Vector3.Lerp(top.transform.localScale, Vector3.zero, 2f * Time.deltaTime);
            bottom.transform.localScale = Vector3.Lerp(bottom.transform.localScale, Vector3.zero, 2f * Time.deltaTime);
        }

        Destroy(top.gameObject);
        Destroy(bottom.gameObject);
        Destroy(gameObject);
    }
}