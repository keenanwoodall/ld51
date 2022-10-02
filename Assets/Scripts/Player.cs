using System;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Player : CharacterMotor
{
    public static Player Instance;
    public static Action Killed;
    public Transform swordContainer;
    public PlayerInput playerInput;
    public float minThrowAngle = 20f;

    public override CharacterControl CharacterInput => playerInput;
    
    private void Awake()
    {
        Instance = this;
        playerInput.PickupSword += OnPickupSwordInput;
        playerInput.DropSword += OnDropSwordInput;
    }
    
    private void OnDropSwordInput()
    {
        var throwDirection = playerInput.LookDirection;
        var throwAngle = Vector3.Angle(transform.forward, throwDirection);
        if (throwAngle < minThrowAngle)
            Sword.Instance.Drop();
        else
            Sword.Instance.Throw(playerInput.LookDirection, Mathf.InverseLerp(minThrowAngle, 180f, throwAngle));
    }

    private void OnPickupSwordInput()
    {
        Sword.Instance.Pickup(swordContainer);
    }

    public void Kill()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        Killed?.Invoke();
        Destroy(this);
    }
}