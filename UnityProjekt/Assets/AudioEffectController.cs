﻿using UnityEngine;
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
        GameObject go = GameObjectPool.Instance.Spawn("SoundEffect", transform.position, Quaternion.identity);
        go.GetComponent<SoundEffectObject>().PlayOneShot(effect);
    }

    public void PlayOneShot(SoundEffect effect, Vector3 position)
    {
        for (int i = 0; i < GameManager.Instance.GetCameras().Length; i++)
        {
            if (GameManager.Instance.GetCameras()[i] != null)
            {
                Vector3 camDiffPos = position - GameManager.Instance.GetCameras()[i].transform.position;
                GameObject go = GameObjectPool.Instance.Spawn("SoundEffect", camDiffPos, Quaternion.identity);
                go.GetComponent<SoundEffectObject>().PlayOneShot(effect);
            }
        }
    }
}
