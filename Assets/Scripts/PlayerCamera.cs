using UnityEngine;

[DefaultExecutionOrder(1)]
public class PlayerCamera : MonoBehaviour
{
   public static PlayerCamera Instance;

   public float followDuration = 0.3f;
   
   private Vector3 _offset;
   private Vector3 positionVelocityRef;
   
   private void Awake() => Instance = this;

   private void Start()
   {
      _offset = transform.position - Player.Instance.transform.position;
   }

   private void FixedUpdate()
   {
      var playerPosition = Player.Instance.transform.position;
      transform.position =
         Vector3.SmoothDamp(transform.position, playerPosition + _offset, ref positionVelocityRef, followDuration);
   }
}
