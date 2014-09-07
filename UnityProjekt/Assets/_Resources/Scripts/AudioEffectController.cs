using UnityEngine;
using System.Collections;

public class AudioEffectController : MonoBehaviour {

    private static AudioEffectController instance;
    public static AudioEffectController Instance
    {
        get
        {
            return instance;
        }
    }

	// Use this for initialization
	void Awake () {
        instance = this;
	}

    public void PlayOneShot(SoundEffect effect)
    {
        GameObject go = EntitySpawnManager.InstantSpawn("SoundEffect", transform.position, Quaternion.identity, countEntity:false);
        go.GetComponent<SoundEffectObject>().PlayOneShot(effect);
    }

    public void PlayOneShot(SoundEffect effect, Vector3 position)
    {
        for (int i = 0; i < GameManager.Instance.GetCameras().Length; i++)
        {
            if (GameManager.Instance.GetCameras()[i] != null)
            {
                Vector3 camDiffPos = position - GameManager.Instance.GetCameras()[i].transform.position;
                GameObject go = EntitySpawnManager.InstantSpawn("SoundEffect", camDiffPos, Quaternion.identity, countEntity: false);
                if(go && go.GetComponent<SoundEffectObject>())
                    go.GetComponent<SoundEffectObject>().PlayOneShot(effect);
            }
        }
    }
}
