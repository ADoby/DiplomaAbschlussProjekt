using UnityEngine;
using System.Collections;

public class EnemieBase : MonoBehaviour {

    public HealthBar healthBar;

    public float Health;
    private float maxHealth;

    public Sprite normal;
    public Sprite death;

    private SpriteRenderer spriteRenderer;
    public string poolName = "EnemieBase1";

    public float HealthRegenPerSec = 2f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        maxHealth = Health;
    }

    public void Reset()
    {
        Health = maxHealth;
        UpdateHealthBar();

        healthBar.UpdateInstant();
        if (spriteRenderer)
            spriteRenderer.sprite = normal;

        if (GetComponent<SpawningBase>())
            GetComponent<SpawningBase>().enabled = true;
    }

    void Update()
    {
        Health = Mathf.Clamp(Health + HealthRegenPerSec*Time.deltaTime, 0f, maxHealth);
        UpdateHealthBar();
    }

    public float ProzentHealth()
    {
        return Health/maxHealth;
    }

    public void UpdateHealthBar()
    {
        healthBar.UpdateBar(Health, maxHealth);
    }

    public void SetHealth(float value)
    {
        Health = Mathf.Clamp(value, 0f, maxHealth);
        UpdateHealthBar();
    }

    public void Damage(float amount)
    {
        Health = Mathf.Clamp(Health - amount, 0f, maxHealth);
        UpdateHealthBar();
        if (Health == 0)
        {
            Die();
        }
    }

    public void Hit(Vector3 position)
    {
        GameObjectPool.Instance.Spawn("Blood", position, Quaternion.identity);
    }

    public void Die()
    {
        healthBar.UpdateInstant();
        if (spriteRenderer)
            spriteRenderer.sprite = death;
        //collider2D.enabled = false;

        if (GetComponent<SpawningBase>())
            GetComponent<SpawningBase>().enabled = false;

        GameObjectPool.Instance.Despawn(poolName, gameObject);
    }

    
}
