using UnityEngine;

public static class Noisy
{
    public static Vector3 Noise3D(float t, float freq, float mag, float per, float lac, int oct)
    {
        return new Vector3
        (
            Noise(new Vector2(t, 0f), freq, mag, per, lac, oct), 
            Noise(new Vector2(0f, t), freq, mag, per, lac, oct),
            Noise(new Vector2(0f, t + 1000f), freq, mag, per, lac, oct)
        );
    }

    public static float Noise(Vector2 p, float freq, float mag, float per, float lac, int oct)
    {
        float f = freq;
        float m = mag;
        float n = 0f;
        for (int i = 0; i < oct; i++)
        {
            n += (Mathf.PerlinNoise(p.x * f, p.y * f) * 2f - 1f) * m;
            f *= lac;
            m *= per;
        }

        return n / oct;
    }
}