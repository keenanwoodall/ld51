using UnityEngine;

[DefaultExecutionOrder(1)]
public class PlayerCamera : MonoBehaviour
{
   public static PlayerCamera Instance;
   
   public new Camera camera;
   public float followDuration = 0.3f;
   public float lookAhead = 1f;
   
   private Vector3 _initialOffset;
   private Vector3 _positionVelocityRef;
   
   private Player player => Player.Instance;

   private void Awake()
   {
      Instance = this;
   }

   private void Start()
   {
      _initialOffset = transform.position - Player.Instance.transform.position;
   }

   private void FixedUpdate()
   {
      var playerTransform = player.transform;
      var playerPosition = playerTransform.position;
      var targetPosition = playerPosition + player.input.LookDirection * lookAhead;
      targetPosition.y = 0f;
      transform.position =
         Vector3.SmoothDamp(transform.position, targetPosition + _initialOffset, ref _positionVelocityRef, followDuration);
   }
}
