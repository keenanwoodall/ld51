using UnityEngine;

public class EnemyInput : CharacterInput
{
    public float pursueDistance = 4f;
    public float circleSpeed = 0.5f;
    private void Update()
    {
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
    }
}