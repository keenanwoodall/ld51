using UnityEngine;
using Random = UnityEngine.Random;

public abstract class CharacterMotor : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float walkResponse = 20f;
    public float stopResponse = 10f;
    public float turnResponse = 10f;
    public Rigidbody rb;
    [Space] 
    public Transform rig;

    public float cycleSpeed = 3.5f;
    public float accelerationTilt = 0.5f;
    public float accelerationTiltResponse = 3f;
    public float sway = 2f;
    public float bob = 0.05f;
    public float squash = 0.06f;

    public AudioSource footstepSource;
    public AudioItem[] footsteps;
    private int _lastFootstep = -1;

    public float CurrentSpeed => rb.velocity.magnitude;
    public abstract CharacterControl CharacterInput { get; }

    private Vector3 _previousVelocity;
    private float _cycle;
    
    protected virtual void FixedUpdate()
    {
        var targetRotation = rb.rotation;
        if (CharacterInput.LookDirection.sqrMagnitude > 0f)
            targetRotation = Quaternion.LookRotation(CharacterInput.LookDirection);
        //rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, turnResponse * Time.fixedDeltaTime);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, turnResponse * Time.fixedDeltaTime));

        var targetVelocity = CharacterInput.Movement * walkSpeed;
        var response = targetVelocity.magnitude >= CurrentSpeed ? walkResponse : stopResponse;
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, response * Time.fixedDeltaTime);
        
        UpdateRig();
    }

    private bool _playedFootstep = true;
    private int _playedFootstepIndex = -1;
    private void UpdateRig()
    {
        _cycle += Time.deltaTime * cycleSpeed * CurrentSpeed;
        
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
        var h = Mathf.Sin(_cycle * 2f);
        if (h < 0f)
        {
            if (!_playedFootstep && footsteps.Length > 0)
            {
                _playedFootstep = true;
                var newSoundIndex = _playedFootstepIndex;
                while (newSoundIndex == _playedFootstepIndex && footsteps.Length > 1)
                {
                    newSoundIndex = Random.Range(0, footsteps.Length);
                }
                newSoundIndex = Mathf.Max(0, newSoundIndex);
                var sound = footsteps[newSoundIndex];
                footstepSource.pitch = 1f + Random.Range(-sound.pitchVariance / 2f, sound.pitchVariance / 2f);
                footstepSource.PlayOneShot(sound.clip, sound.volume);

                _playedFootstepIndex = newSoundIndex;
            }
        }
        else
        {
            _playedFootstep = false;
        }
        rig.localPosition = Vector3.up * (Mathf.Max(h, 0f) * bob * CurrentSpeed) / walkSpeed;
        
        // squash
        var s = Mathf.Sin(_cycle * 2f) * CurrentSpeed / walkSpeed * squash;
        SetSquash(s);
        
        _previousVelocity = rb.velocity;
    }

    public void SetSquash(float s)
    {
        rig.localScale = new Vector3(1f / (1f + s), 1f + s, 1f / (1f + s));
    }
}