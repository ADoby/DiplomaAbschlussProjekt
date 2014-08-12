using UnityEngine;
using System.Collections;

public class RandomSounds : MonoBehaviour {

    public SoundEffect[] sounds;

    public float timer = 0;
    public float minTime = 1f, maxTime = 1f;

	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = Random.Range(minTime, maxTime);
            PlaySound();
        }
	}

    public void PlaySound()
    {
        SoundEffect effect = sounds[Random.Range(0, sounds.Length)];
        AudioEffectController.Instance.PlayOneShot(effect, transform.position);
    }
}
