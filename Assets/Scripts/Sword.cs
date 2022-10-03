using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class Sword : MonoBehaviour
{
    public static Sword Instance;

    public float maxSpinSpeed = 2160f;
    public float minThrowSpeed = 3f;
    public float maxThrowSpeed = 30f;
    [Space]
    public Transform wobbleRoot;
    public float wobbleDuration = 1f;
    public float wobbleSpeed = 20f;
    public float wobbleStrength = 10f;
    public AnimationCurve wobbleCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    [Space]
    public Rigidbody rb;
    public Transform rotateAround;
    public GameObject blood;
    public float bloodDuration;

	public enum SwordState
	{
		Idle,
		Holding,
        Retrieving,
		Throwing,
		Stuck,
	}

    public SwordState State = SwordState.Idle;

    public ISwordTarget CurrentSwordTarget;
    private float _currentThrowSpeed;
    private Vector3 _throwDirection;

    private float lastBloodTime = -100f;

    private void OnEnable()
    {
        Instance = this;
        SetBloodActive(false);
    }

    public void SetBloodActive(bool active)
    {
        foreach (var p in blood.GetComponentsInChildren<ParticleSystem>())
        {
            if (active && !p.isPlaying)
                p.Play();
            else if (!active && p.isPlaying)
                p.Stop();
        }
    }
    private void FixedUpdate()
    {
        if (Time.time - lastBloodTime > bloodDuration)
        {
            SetBloodActive(false);
        }

        if (State == SwordState.Throwing)
        {
            transform.RotateAround(rotateAround.position, Vector3.up, maxSpinSpeed * Time.fixedDeltaTime);
            transform.position += _throwDirection * (_currentThrowSpeed * Time.fixedDeltaTime);
            rb.position = transform.position;
        }
    }

    public void Pickup(Transform holder)
    {
        if (State == SwordState.Stuck && _wiggleRoutine != null)
        {
            StopCoroutine(_wiggleRoutine);
        }

        if (State is SwordState.Holding or SwordState.Retrieving)
            return;

        StartCoroutine(PickupRoutine(holder));
    }

    private IEnumerator PickupRoutine(Transform holder)
    {
        State = SwordState.Retrieving;
        rb.detectCollisions = true;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        transform.SetParent(null);
        transform.localScale = Vector3.one;
        
        transform.LookAt(Player.Instance.transform.position);

        if (CurrentSwordTarget != null)
        {
            var st = CurrentSwordTarget;
            CurrentSwordTarget = null;
            st.OnRelease(this);
        }

        float returnTime = 0f;
        while (Vector3.Distance(transform.position, holder.position) > 1f)
        {
            if (State is not SwordState.Retrieving)
                yield break;
            returnTime += Time.fixedDeltaTime;
            transform.RotateAround(rotateAround.position, Vector3.up, maxSpinSpeed * Time.fixedDeltaTime);
            transform.position = Vector3.MoveTowards(transform.position, holder.position, Mathf.Clamp01(returnTime) * maxThrowSpeed * Time.fixedDeltaTime);
            rb.position = transform.position;
            yield return new WaitForFixedUpdate();
        }

        State = SwordState.Holding;
        
        transform.SetParent(holder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
    
    public void Drop()
    {
        if (State is SwordState.Throwing or SwordState.Retrieving)
            return;

        rb.isKinematic = false;
        if (CurrentSwordTarget != null)
        {
            var st = CurrentSwordTarget;
            CurrentSwordTarget = null;
            st.OnRelease(this);
        }

        rb.detectCollisions = true;
        
        transform.SetParent(null);

        rb.constraints = RigidbodyConstraints.None;
        State = SwordState.Idle;
    }

	public void Throw(Vector3 direction, float speed)
    {
        if (CurrentSwordTarget != null)
        {
            var st = CurrentSwordTarget;
            CurrentSwordTarget = null;
            st.OnRelease(this);
        }
        
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.detectCollisions = true;
        _throwDirection = direction;
        var throwSpeed = Mathf.Lerp(minThrowSpeed, maxThrowSpeed, speed);
        _currentThrowSpeed = throwSpeed;   
        rb.constraints = RigidbodyConstraints.FreezeAll;
		State = SwordState.Throwing;
	}
    
    private void OnCollisionEnter(Collision collision)
    {
        var enemyLayer = LayerMask.NameToLayer("Enemy");
        var playerLayer = LayerMask.NameToLayer("Player");
        if (collision.gameObject.layer != playerLayer && State is not SwordState.Stuck)
        {
            rb.isKinematic = true;

            var contact = collision.GetContact(0);

            if (collision.gameObject.layer == enemyLayer)
            {
                SetBloodActive(true);
                lastBloodTime = Time.time;
                if (State is SwordState.Throwing)
                {
                    rb.detectCollisions = false;
                    State = SwordState.Stuck;
                    var enemyRB = collision.transform.GetComponentInParent<Rigidbody>();
                    transform.SetParent(enemyRB.transform);
                    var enemyPosition = collision.transform.position;
                    enemyPosition.y = transform.position.y;
                    var normal = (transform.position - enemyPosition).normalized;
                    transform.position =
                        enemyPosition + normal * (wobbleRoot.position - transform.position).magnitude;
                    transform.rotation = Quaternion.LookRotation(normal, Vector3.up);
                }
                else if (State is SwordState.Retrieving or SwordState.Holding)
                {
                    var enemy = collision.transform.GetComponentInParent<EnemyControl>();
                    enemy.Kill();

                    return;
                }
            }
            else
            {
                rb.detectCollisions = false;
                State = SwordState.Stuck;
                transform.rotation =
                    Quaternion.LookRotation(contact.normal, Vector3.up);
                transform.position =
                    contact.point + contact.normal * (wobbleRoot.position - transform.position).magnitude;
            }

            CurrentSwordTarget = collision.gameObject.GetComponentInParent<ISwordTarget>();
            CurrentSwordTarget?.OnStuck(this);
            
            if (_wiggleRoutine != null)
                StopCoroutine(_wiggleRoutine);
            _wiggleRoutine = StartCoroutine(WiggleRoutine());
        }
    }

    private Coroutine _wiggleRoutine;
    private IEnumerator WiggleRoutine(bool inverse = false, float speed = 1f)
    {
        var t = 0f;
        while (!Mathf.Approximately(t, 1f))
        {
            t = Mathf.Clamp01(t + Time.deltaTime / wobbleDuration * speed);
            var tt = t;
            if (inverse)
                tt = 1f - t;
            yield return null;
            var angle = Mathf.Sin(tt * wobbleSpeed) * wobbleCurve.Evaluate(tt) * wobbleStrength;
            wobbleRoot.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }
    }
}
