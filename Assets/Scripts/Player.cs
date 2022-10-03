using System;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Player : CharacterMotor
{
    public static Player Instance;
    public static Action<string>Killed;
    public Transform swordContainer;
    public PlayerInput playerInput;
    public float minThrowAngle = 20f;
    public float minThrowSpeed = 1f;
    
    private Vector3 _lastMouseWorldPosition;

    public override CharacterControl CharacterInput => playerInput;
    
    private void Awake()
    {
        Instance = this;
        playerInput.PickupSword += OnPickupSwordInput;
        playerInput.DropSword += OnDropSwordInput;
    }

    private void LateUpdate()
    {
        _lastMouseWorldPosition = GetMouseWorldPosition();
    }

    private Vector3 GetMouseWorldPosition()
    {
        var ray = PlayerCamera.Instance.camera.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.up, Vector3.zero);
        if (plane.Raycast(ray, out float hitDistance))
        {
            return ray.GetPoint(hitDistance);
        }

        return default;
    }

    private void OnDropSwordInput()
    {
        if (!Sword.Instance.transform.IsChildOf(transform))
            return;
        
        var throwDirection = playerInput.LookDirection;
        var throwAngle = Vector3.Angle(transform.forward, throwDirection);
        if (throwAngle < minThrowAngle && Vector3.Distance(GetMouseWorldPosition(), _lastMouseWorldPosition) / Time.deltaTime < minThrowSpeed)
            Sword.Instance.Drop();
        else
            Sword.Instance.Throw(playerInput.LookDirection, 1f);
    }

    private void OnPickupSwordInput()
    {
        Sword.Instance.Pickup(swordContainer);
    }

    private bool _killed; 
    public void Kill(string reason)
    {
        if (_killed || this == null)
            return;
        _killed = true;
        Killed?.Invoke(reason);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        Destroy(this);
    }
}