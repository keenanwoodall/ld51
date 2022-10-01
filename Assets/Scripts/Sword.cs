using System.Collections;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

[DefaultExecutionOrder(1)]
public class Sword : MonoBehaviour
{
    public static Sword Instance;

    public float maxSpinSpeed = 2160f;
    public float minThrowSpeed = 10f;
    public float maxThrowSpeed = 20f;
    public float maxDropForce = 15f;
    public float minThrowAngle = 15f;
    public AnimationCurve throwSensitivityCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [Space]
    public Transform wobbleRoot;
    public float wobbleDuration = 1f;
    public float wobbleSpeed = 20f;
    public float wobbleStrength = 10f;
    public AnimationCurve wobbleCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    [Space]
    public Rigidbody rb;
    public Transform rotateAround;

    public enum SwordState
    {
        Held,
        Stuck, 
        Dropped,
    }

    private bool _thrown;
    
    private SwordState _state;
    private bool _heldBefore;
    
    private float _currentThrowSpeed;
    private Vector3 _throwDirection;
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void FixedUpdate()
    {
        if (_heldBefore && _state == SwordState.Dropped && _thrown)
        {
            transform.RotateAround(rotateAround.position, Vector3.up, maxSpinSpeed * Time.fixedDeltaTime);
            transform.position += _throwDirection * (_currentThrowSpeed * Time.fixedDeltaTime);
            rb.position = transform.position;
        }
    }

    public void Pickup(Transform holder)
    {
        if (_state == SwordState.Stuck && _stuckRoutine != null)
        {
            StopCoroutine(_stuckRoutine);
        }
        
        _heldBefore = true;
        if (_state == SwordState.Held)
            return;
        
        _state = SwordState.Held;
        
        rb.isKinematic = true;
        rb.constraints = ~RigidbodyConstraints.FreezeAll;
        transform.SetParent(holder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
    
    public void Drop()
    {
        if (_state != SwordState.Held)
            return;
        
        _state = SwordState.Dropped;
        rb.isKinematic = false;
        
        transform.SetParent(null);

        _throwDirection = Player.Instance.input.AimDirection;
        var throwAngle = Vector3.Angle(Player.Instance.transform.forward, _throwDirection);
        _thrown = throwAngle > minThrowAngle; 
        if (_thrown)
        {
            var throwSpeed = Mathf.Lerp(minThrowSpeed, maxThrowSpeed, throwAngle / 180f);
            _currentThrowSpeed = throwSpeed;   
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForceAtPosition(_throwDirection * (maxDropForce * throwSensitivityCurve.Evaluate(throwAngle / minThrowAngle)), wobbleRoot.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_state == SwordState.Dropped && _thrown && collision.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            rb.isKinematic = true;
            _state = SwordState.Stuck;

            foreach (var c in collision.contacts)
            {
                Debug.DrawRay(c.point, c.normal * 0.1f, Color.magenta, 20f);
            }
            
            var contact = collision.contacts[^1];
            var penetrationDirection = contact.normal;
            transform.rotation = Quaternion.LookRotation(penetrationDirection, Vector3.up);
            transform.position = contact.point + contact.normal * (wobbleRoot.position - transform.position).magnitude;

            if (_stuckRoutine != null)
                StopCoroutine(_stuckRoutine);
            _stuckRoutine = StartCoroutine(StuckRoutine());
        }
    }

    private Coroutine _stuckRoutine;
    private IEnumerator StuckRoutine()
    {
        var t = 0f;
        while (!Mathf.Approximately(t, 1f))
        {
            t = Mathf.Clamp01(t + Time.deltaTime / wobbleDuration);
            yield return null;
            var angle = Mathf.Sin(t * wobbleSpeed) * wobbleCurve.Evaluate(t) * wobbleStrength;
            wobbleRoot.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }
    }
}