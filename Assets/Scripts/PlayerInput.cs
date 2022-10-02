using System;
using UnityEngine;

[DefaultExecutionOrder(-2)]
public class PlayerInput : CharacterControl
{
    public event Action PickupSword;
    public event Action DropSword;

    private new PlayerCamera camera => PlayerCamera.Instance;
    private Player player => Player.Instance;
    
    private void Update()
    {
        if (!player)
            return;
        
        var inputPlane = new Plane(Vector3.up, Vector3.zero);
        
        Movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        Movement = Vector3.ClampMagnitude(Movement, 1f);

        if (Input.GetMouseButtonDown(0))
            PickupSword?.Invoke();
        if (Input.GetMouseButtonUp(0))
            DropSword?.Invoke();

        var inputRay = camera.camera.ScreenPointToRay(Input.mousePosition);
        if (inputPlane.Raycast(inputRay, out var distance))
        {
            var hitPoint = inputRay.GetPoint(distance);
            var playerPosition = player.transform.position;
            playerPosition.y = 0f;
            LookDirection = (hitPoint - playerPosition).normalized;
        }
    }
}