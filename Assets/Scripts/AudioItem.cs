using UnityEngine;

[System.Serializable]
public class AudioItem
{
    public AudioClip clip;
    public float volume = 1f;
    public float pitchVariance;

    public void PlayOn(AudioSource source)
    {
        source.pitch = 1f + Random.Range(-pitchVariance / 2f, pitchVariance / 2f);
        source.PlayOneShot(clip, volume);
    }
}