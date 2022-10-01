using UnityEngine;

[DefaultExecutionOrder(-2)]
public class PlayerInput : MonoBehaviour
{
    public Vector3 Movement;
    public Vector3 LookDirection;
    public bool Walking => Movement.sqrMagnitude > 0f;

    private new PlayerCamera camera => PlayerCamera.Instance;
    private Player player => Player.Instance;
    
    private void Update()
    {
        var inputPlane = new Plane(Vector3.up, Vector3.zero);
        
        Movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        Movement = Vector3.ClampMagnitude(Movement, 1f);

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