using UnityEngine;

public abstract class CharacterInput : MonoBehaviour
{
    public Vector3 Movement;
    public Vector3 LookDirection;
    public bool Walking => Movement.sqrMagnitude > 0f;
}