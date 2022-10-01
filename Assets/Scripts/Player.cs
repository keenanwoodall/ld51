using System;
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
    public PlayerInput input;
    [Space]
    public Transform swordContainer;
    public LayerMask swordLayer;
    [Space] 
    public Transform rig;

    public float cycleSpeed = 3f;
    public float accelerationTilt = 3f;
    public float accelerationTiltResponse = 3f;
    public float sway = 5f;
    public float bob = 0.1f;
    public float squash = 0.1f;

    public float CurrentSpeed => rb.velocity.magnitude;

    private Vector3 _previousVelocity;

    private float _cycle;
    
    private void Awake()
    {
        Instance = this;
        input.PickupSword += OnPickupSwordInput;
        input.DropSword += OnDropSwordInput;
    }

    private void FixedUpdate()
    {
        var targetRotation = rb.rotation;
        if (input.AimDirection.sqrMagnitude > 0f)
            targetRotation = Quaternion.LookRotation(input.AimDirection);
        //rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, turnResponse * Time.fixedDeltaTime);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, turnResponse * Time.fixedDeltaTime));

        var targetVelocity = input.Movement * walkSpeed;
        var response = input.Walking ? walkResponse : stopResponse;
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, response * Time.fixedDeltaTime);
        
        _cycle += Time.deltaTime * cycleSpeed * CurrentSpeed;

        UpdateRig();
        
        _previousVelocity = rb.velocity;
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
        rig.localRotation *= Quaternion.Euler(0f, 0f, Mathf.Sin(_cycle) * sway * CurrentSpeed / walkSpeed);
        
        // bob
        rig.localPosition = Vector3.up * (Mathf.Max(Mathf.Sin(_cycle * 2f), 0f) * bob * CurrentSpeed) / walkSpeed;
        
        // squash
        var s = Mathf.Sin(_cycle * 2f) * CurrentSpeed / walkSpeed * squash;
            rig.localScale = new Vector3(1f / (1f + s), 1f + s, 1f / (1f + s));
    }

    private bool _canPickUpSword;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Sword"))
        {
            _canPickUpSword = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Sword"))
        {
            _canPickUpSword = false;
        }
    }
    
    private void OnDropSwordInput()
    {
        Sword.Instance.Drop();
    }

    private void OnPickupSwordInput()
    {
        if (_canPickUpSword)
        {
            Sword.Instance.Pickup(swordContainer);
        }
    }
}