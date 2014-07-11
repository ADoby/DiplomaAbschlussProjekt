using System.Globalization;
using UnityEngine;
using System.Collections;

public class EnemieBase : HitAble {

    public HealthBar healthBar;

    private float CurrentHealth;
    private float wantedHealth = 1;
    public float maxHealth = 100;

    public string poolName = "EnemieBase1";

    public float HealthRegenPerSec = 2f;

    private bool isAlive = false;

    public bool StartFull = false, RestartFull = false;

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
            CurrentHealth = maxHealth;
            wantedHealth = CurrentHealth;
            UpdateHealthBar(true);
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
            CurrentHealth = maxHealth;
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

        if (!isAlive && CurrentHealth <= 1)
        {
            Die();
        }
    }

    public float ProzentHealth()
    {
        return CurrentHealth/maxHealth;
    }

    public void UpdateHealthBar(bool instant = false)
    {
        healthBar.UpdateBar(CurrentHealth, maxHealth, instant);
    }

    public void SetHealth(float value)
    {
        CurrentHealth = Mathf.Clamp(value, 0f, maxHealth);
        wantedHealth = CurrentHealth;
        UpdateHealthBar(true);
    }

    public override void Damage(float amount)
    {
        base.Damage(amount);

        wantedHealth = Mathf.Clamp(wantedHealth - amount, 0f, maxHealth);

        if (wantedHealth <= 0)
        {
            isAlive = false;
        }
    }

    public void Heal(float amount)
    {
        wantedHealth = Mathf.Clamp(wantedHealth + amount, 0f, maxHealth);
    }

    public void Die()
    {
        GameObjectPool.Instance.Despawn(poolName, gameObject);
    }
}
