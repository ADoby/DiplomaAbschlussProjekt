using UnityEngine;
using System.Collections;

public class ParticleEffekt : MonoBehaviour {

    public string poolName = "ParticleEffekt";

    public void Reset()
    {
        if (particleSystem)
        {
            particleSystem.Play();
        }
    }

	// Update is called once per frame
	void Update () {
        if (particleSystem && !particleSystem.IsAlive())
        {
            GameObjectPool.Instance.Despawn(poolName, gameObject);
        }
	}
}
