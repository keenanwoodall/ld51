using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private void Start(){
        DontDestroyOnLoad(gameObject);
    }
}