using System;
using UnityEngine;

public class EnemyInput : CharacterInput, ISwordTarget
{
    public float pursueDistance = 4f;
    public float circleSpeed = 0.5f;
    public float knockbackRecoverySpeed = 3f;
    public float knockbackForce = 5f;
    
    private Vector3 _knockbackVelocity;
    private void Update()
    {
        _knockbackVelocity = Vector3.Lerp(_knockbackVelocity, Vector3.zero, knockbackRecoverySpeed * Time.deltaTime);

        var player = Player.Instance;
        if (Vector3.Distance(player.transform.position, transform.position) > pursueDistance)
        {
            Movement = (player.transform.position - transform.position).normalized;
            LookDirection = Movement;
        }
        else
        {
            Movement = Vector3.Cross((player.transform.position - transform.position).normalized, Vector3.up) * circleSpeed;
            LookDirection = (player.transform.position - transform.position).normalized;
        }

        Movement += _knockbackVelocity;
    }

    public void OnStuck(Sword sword)
    {
        _knockbackVelocity += Vector3.ProjectOnPlane
        (
            vector: Vector3.Normalize(transform.position - sword.transform.position), 
            planeNormal: Vector3.up
        ) * knockbackForce;
    }

    public void OnRelease(Sword sword)
    {
        Destroy(gameObject);
    }
}