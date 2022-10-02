using UnityEngine;

public class Enemy : Character, ISwordTarget
{
    public EnemyInput enemyInput;
    public float knockbackForce = 5f;
    public float knockbackRecoverySpeed = 3f;

    private Vector3 _knockbackVelocity;
    
    public override CharacterInput CharacterInput => enemyInput;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (_knockbackVelocity.sqrMagnitude > 0.05f)
            rb.velocity = _knockbackVelocity;
        _knockbackVelocity = Vector3.Lerp(_knockbackVelocity, Vector3.zero, knockbackRecoverySpeed * Time.deltaTime);
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