using System.Globalization;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemieBase : HitAble {

    public HealthBar healthBar;

    public float CurrentHealth;
    public float wantedHealth = 1;
    public float MaxHealth = 100;

    public string poolName = "EnemieBase1";

    public float HealthRegenPerSec = 2f;

    public bool isAlive = false;

    public bool StartFull = false, RestartFull = false;


    public LayerMask GroundLayer;

    void Awake()
    {
        GameEventHandler.OnPause += OnPause;
        GameEventHandler.OnResume += OnResume;
    }

    void Start()
    {
        Reset();
        if (StartFull)
        {
            CurrentHealth = MaxHealth;
            wantedHealth = CurrentHealth;
            UpdateHealthBar(true);
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.up, -transform.up, 2f, GroundLayer);

        if (hit)
        {
            transform.localRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
    }

    void OnPause()
    {
        enabled = false;
    }

    void OnResume()
    {
        enabled = true;
    }

    public void Reset()
    {
        isAlive = true;
        if (RestartFull)
        {
            CurrentHealth = MaxHealth;
        }
        else
        {
            CurrentHealth = 0;
        }
        
        wantedHealth = CurrentHealth;

        UpdateHealthBar(true);
    }

    public float HealthChangeSpeed = 10f;

    void Update()
    {
        if (isAlive)
        {
            Heal(HealthRegenPerSec*Time.deltaTime);
        }
        CurrentHealth = Mathf.Lerp(CurrentHealth, wantedHealth, Time.deltaTime * HealthChangeSpeed);
        UpdateHealthBar();

        if (!isAlive && ProzentHealth() <= 0.01f)
        {
            Die();
        }
    }

    public float ProzentHealth()
    {
        return CurrentHealth/MaxHealth;
    }

    public void UpdateHealthBar(bool instant = false)
    {
        healthBar.UpdateBar(CurrentHealth, MaxHealth, instant);
    }

    public void SetHealth(float value)
    {
        CurrentHealth = Mathf.Clamp(value, 0f, MaxHealth);
        wantedHealth = CurrentHealth;
        UpdateHealthBar(true);
    }

    [ContextMenu("Kill")]
    public void Kill()
    {
        Damage(MaxHealth);
    }

    public override void Damage(float amount)
    {
        base.Damage(amount);

        wantedHealth = Mathf.Clamp(wantedHealth - amount, 0f, MaxHealth);

        if (wantedHealth <= 0)
        {
            isAlive = false;
        }
    }

    public void Heal(float amount)
    {
        wantedHealth = Mathf.Clamp(wantedHealth + amount, 0f, MaxHealth);
    }

    public void Die()
    {
        GameObjectPool.Instance.Despawn(poolName, gameObject);
    }

    public void HealFull()
    {
        isAlive = true;
        SetHealth(MaxHealth);
    }
}
