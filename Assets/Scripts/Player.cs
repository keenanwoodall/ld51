using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Player : MonoBehaviour
{
    public static Player Instance;
    
    public float walkSpeed = 5f;
    public float walkResponse = 20f;
    public float stopResponse = 10f;
    public float turnResponse = 50f;
    public Rigidbody rb;
    [Space] 
    public Transform rig;

    public float cycleSpeed = 3f;
    public float accelerationTilt = 3f;
    public float accelerationTiltResponse = 3f;
    public float sway = 5f;
    public float bob = 0.1f;
    public float squash = 0.1f;


    private bool _walking;
    private Vector3 _targetDirection;
    private Quaternion _targetRotation = Quaternion.identity;
    private float _currentSpeed;
    private Vector3 _previousVelocity;

    private float _cycle;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        _targetDirection = Vector3.ClampMagnitude(input, 1f);
        _walking = _targetDirection.sqrMagnitude > 0f;
    }

    private void FixedUpdate()
    {
        if (_walking)
            _targetRotation = Quaternion.LookRotation(_targetDirection);
        rb.rotation = Quaternion.Slerp(rb.rotation, _targetRotation, turnResponse * Time.fixedDeltaTime);

        var targetSpeed = _walking ? walkSpeed : 0f;
        var response = _walking ? walkResponse : stopResponse;
        
        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, response * Time.fixedDeltaTime);

        var velocity = transform.forward * _currentSpeed;

        rb.velocity = velocity;
        
        _cycle += Time.deltaTime * cycleSpeed * _currentSpeed;

        UpdateRig();
        
        _previousVelocity = velocity;
    }

    private void UpdateRig()
    {
        var acceleration = (rb.velocity - _previousVelocity) / Time.deltaTime;
        var axis = Vector3.Cross(acceleration, Vector3.up);

        // tilt
        rig.rotation = Quaternion.Slerp
        (
            rig.rotation,
            Quaternion.AngleAxis(accelerationTilt * acceleration.magnitude, axis) * transform.rotation,
            accelerationTiltResponse * Time.deltaTime
        );
        
        // sway
        rig.localRotation *= Quaternion.Euler(0f, 0f, Mathf.Sin(_cycle) * sway * _currentSpeed / walkSpeed);
        
        // bob
        rig.localPosition = Vector3.up * (Mathf.Max(Mathf.Sin(_cycle * 2f), 0f) * bob * _currentSpeed) / walkSpeed;
        
        // squash
        var s = Mathf.Sin(_cycle * 2f) * _currentSpeed / walkSpeed * squash;
            rig.localScale = new Vector3(1f / (1f + s), 1f + s, 1f / (1f + s));
    }
}