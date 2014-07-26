using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoundEffect {

    public AudioClip clip;
    public float volumeScale = 1.0f;
    public bool randomPitch = false;
    public float minPitch = 0.0f, maxPitch = 1.0f;
    public float pitch = 1.0f;

    public float Pitch
    {
        get
        {
            if (randomPitch)
            {
                return Random.Range(minPitch, maxPitch);
            }
            else
            {
                return pitch;
            }
        }
    }
}
