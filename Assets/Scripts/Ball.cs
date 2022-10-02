using System;
using MPO;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody rb;
    public ColorPropertyOverride outline;
    public Collider collider;
    public Color friendlyColor = Color.blue;
    public float speed = 10f;

    private bool _friendly;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & LayerMask.GetMask("Ball hitter")) > 0)
        {
            if (!_friendly)
            {
                Physics.IgnoreCollision(collider, collision.collider);
                _friendly = true;
                outline.Value = friendlyColor;
                rb.velocity = -rb.velocity.normalized * speed;
            }

            return;
        }
        
        if (_friendly && ((1 << collision.gameObject.layer) & LayerMask.GetMask("Enemy")) > 0)
        {
            var enemy = collision.gameObject.GetComponentInParent<EnemyControl>();
            if (enemy)
                enemy.Kill();
        }
        
        if (!_friendly && ((1 << collision.gameObject.layer) & LayerMask.GetMask("Player")) > 0)
        {
            Player.Instance.Kill();
        }
        
        if (((1 << collision.gameObject.layer) & LayerMask.GetMask("Ball")) > 0)
        {
            return;
        }
        
        Destroy(gameObject);
    }
}
