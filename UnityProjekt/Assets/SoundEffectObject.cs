using UnityEngine;
using System.Collections;

public class SoundEffectObject : MonoBehaviour {

    public void PlayOneShot(SoundEffect effect)
    {
        audio.pitch = effect.Pitch;
        audio.PlayOneShot(effect.clip, effect.volumeScale);
        timer = effect.clip.length;
    }

    public void Reset()
    {
        audio.pitch = 1.0f;
    }

    public float timer = 0;

    void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else if(timer < 0)
        {
            timer = 0;
            GameObjectPool.Instance.Despawn("SoundEffect", gameObject);
        }
    }
}
