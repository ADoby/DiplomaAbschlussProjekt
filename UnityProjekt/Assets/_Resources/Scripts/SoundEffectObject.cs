using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoundEffectObject : MonoBehaviour {

    public float extraTime = 1.0f;

    public void PlayOneShot(SoundEffect effect)
    {
        audio.pitch = effect.Pitch;
        audio.volume = effect.volumeScale;
        audio.PlayOneShot(effect.clip);
        timer = effect.clip.length + extraTime;
    }

    public void Reset()
    {
        audio.pitch = 1.0f;
    }

    public float timer = 0;

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else if(timer < 0)
        {
            timer = 0;
            GameObjectPool.Instance.Despawn("SoundEffect", gameObject);
        }
    }
}
