using UnityEngine;

[DefaultExecutionOrder(1)]
public class PlayerCamera : MonoBehaviour
{
   public static PlayerCamera Instance;
   
   public new Camera camera;
   public float followDuration = 0.3f;
   public float lookAhead = 1f;
   [Space]
   public float shakeSpeed = 5f;
   public float shakeDamping = 5f;
   public float shakeStrength = 0.3f;
   
   private Vector3 _initialOffset;
   private Vector3 _positionVelocityRef;
   private float _shakeT;
   public float CurrentShakeStrength;
   
   private Player player => Player.Instance;

   private void Awake()
   {
      Instance = this;
   }

   private void Start()
   {
      _initialOffset = transform.position - Player.Instance.transform.position;
      LightningSpawner.LightningSpawned += OnLightningSpawned;
   }

   private void FixedUpdate()
   {
      if (player)
      {
         var playerTransform = player.transform;
         var playerPosition = playerTransform.position;
         var targetPosition = playerPosition + player.playerInput.LookDirection * lookAhead;
         targetPosition.y = 0f;

         transform.position =
            Vector3.SmoothDamp(transform.position, targetPosition + _initialOffset, ref _positionVelocityRef,
               followDuration);
      }

      UpdateShake();
   }

   private void UpdateShake()
   {
      _shakeT += Time.fixedDeltaTime * shakeSpeed;
      CurrentShakeStrength = Mathf.Lerp(CurrentShakeStrength, 0f, shakeDamping * Time.deltaTime);

      if (CurrentShakeStrength > 0.001f)
      {
         camera.transform.localPosition = Noisy.Noise3D(_shakeT, 1f, CurrentShakeStrength, 0.5f, 2f, 1);
      }
   }
   
   private void OnLightningSpawned()
   {
      CurrentShakeStrength += shakeStrength;
   }
}
