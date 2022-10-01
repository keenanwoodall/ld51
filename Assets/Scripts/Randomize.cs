using UnityEngine;

public class Randomize : MonoBehaviour
{
    public Vector3 positionRange;
    public Vector3 rotationRange;

    private void Start()
    {
        transform.position += new Vector3
        (
            Random.Range(-positionRange.x, positionRange.x),
            Random.Range(-positionRange.y, positionRange.y),
            Random.Range(-positionRange.z, positionRange.z)
        );
        
        transform.rotation *= Quaternion.Euler(new Vector3
        (
            Random.Range(-rotationRange.x, rotationRange.x),
            Random.Range(-rotationRange.y, rotationRange.y),
            Random.Range(-rotationRange.z, rotationRange.z)
        ));
    }
}