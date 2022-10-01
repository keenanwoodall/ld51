using UnityEngine;

public class LightningSpawner : MonoBehaviour
{
    public Lightning seed;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var ray = PlayerCamera.Instance.camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var lightning = Instantiate(seed);
                lightning.Strike(hit.point, transform.position);
            }
        }
    }
}