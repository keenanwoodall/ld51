using UnityEngine;

public interface ISwordTarget
{
    void OnStuck(Sword sword);
    void OnRelease(Sword sword);
}