using UnityEngine;
using System.Collections;

public class EnemieBase : MonoBehaviour {

    public HealthBar healthBar;

    public float Health;
    private float maxHealth;

    public Sprite normal;
    public Sprite death;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        maxHealth = Health;
    }

    public string poolName = "EnemieBase1";

    public void Damage(float amount)
    {
        Health -= amount;
        healthBar.UpdateBar(Health, maxHealth);
        if (Health <= 0)
        {
            healthBar.UpdateInstant();
            spriteRenderer.sprite = death;
            Health = 0;
            collider2D.enabled = false;

            if (GetComponent<SpawningBase>())
                GetComponent<SpawningBase>().enabled = false;
        }
        
    }

    public void Hit(Vector3 position)
    {
        GameObjectPool.Instance.Spawn("Blood", position, Quaternion.identity);
    }
}
