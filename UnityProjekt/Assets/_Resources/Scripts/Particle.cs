using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour {

    public Vector2 startXForce = new Vector2(-2, 2);
    public Vector2 startYForce = new Vector2(2, 6);

    public float lifeTime = 2f;
    private float lifeTimer = 0f;

    public float startAlphaLossAt = 0.8f;

    private SpriteRenderer spriteRenderer;

    public string poolName;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Reset()
    {
        lifeTimer = lifeTime;

        rigidbody2D.velocity = new Vector3(Random.Range(startXForce.x, startXForce.y), Random.Range(startYForce.x, startYForce.y), 0);
    }
	
	// Update is called once per frame
	void Update () {
        
        if (lifeTimer > 0)
        {
            UpdateParticle();

            lifeTimer -= Time.deltaTime;
        }
        else if(lifeTimer < 0)
        {
            lifeTimer = 0;

            UpdateParticle();

            GameObjectPool.Instance.Despawn(poolName, gameObject);
        }
	}

    public void UpdateParticle()
    {
        float alphaLossStartValue = lifeTime - startAlphaLossAt * lifeTime;
        if (lifeTimer < alphaLossStartValue)
        {
            Color color = spriteRenderer.color;
            color.a = lifeTimer / alphaLossStartValue;
            spriteRenderer.color = color;
        }
        else
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
    }
}
