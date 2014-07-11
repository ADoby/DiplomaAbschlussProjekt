using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
    public float startSize = 0.05f;
    public float endSize = 1.0f;

    public float time = 1f;
    private float timer = 0f;

    public string poolName = "Explosion";

    public Animator anim;

    public void Reset()
    {
        timer = time;
        if (anim)
        {
            anim.SetTrigger("Explode");
        }
    }
	
	// Update is called once per frame
	void Update () {

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            EntitySpawnManager.Despawn(poolName, gameObject, false);
            //GameObjectPool.Instance.Despawn(poolName, gameObject);
        }
        /*timer = Mathf.Clamp(timer + Time.deltaTime, 0f, time);

        float size = startSize + endSize * (timer / time);
        transform.localScale = new Vector3(size, size, size);

        if (timer > time * 0.8f)
        {
            GetComponent<SpriteRenderer>().color = new Color(1,1,1,time - timer);
        }

        if (timer == time)
        {
            GameObjectPool.Instance.Despawn(poolName, gameObject);
        }*/
	}
}
