using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicController : MonoBehaviour
{

    public AudioClip[] AudioClips;

    private int currentTrack = 0;

	// Use this for initialization
	void Start ()
	{
	    Reset();
	}

    public void Reset()
    {
        StartTrack(0);
    }

    public void PlayTrack()
    {
        audio.clip = AudioClips[currentTrack];
        audio.Play();
    }

    public void RestartTrack()
    {
        
    }

    public void StartTrack(int index)
    {
        if (index >= 0 && index < AudioClips.Length)
        {
            currentTrack = index;
            PlayTrack();
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
