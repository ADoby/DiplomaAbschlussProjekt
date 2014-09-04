using System.Globalization;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemieBase : HitAble {

    public HealthBar healthBar;

    public float CurrentHealth;
    public float wantedHealth = 1;
    public float MaxHealth = 100;

    public float MaxHealthPerDifficulty = 30f;

    public string poolName = "EnemieBase1";

    public float HealthRegenPerSec = 2f;
    public float HealthRegenPerSecPerDifficulty = 1f;

    public bool isAlive = false;

    public bool StartFull = false, RestartFull = false;

    public LayerMask GroundLayer;

    public float HealthChangeSpeed = 10f;

    public SpawnInfos[] Drops;

    void Start()
    {
        Reset();
        if (StartFull)
        {
            CurrentHealth = MaxHealth + GameManager.Instance.CurrentDifficulty * MaxHealthPerDifficulty;
            wantedHealth = CurrentHealth;
            UpdateHealthBar(true);
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.up, -transform.up, 2f, GroundLayer);

        if (hit)
        {
            transform.localRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
    }

    public void Reset()
    {
        isAlive = true;
        if (RestartFull)
        {
            CurrentHealth = MaxHealth + GameManager.Instance.CurrentDifficulty * MaxHealthPerDifficulty;
        }
        else
        {
            CurrentHealth = 0;
        }
        
        wantedHealth = CurrentHealth;

        UpdateHealthBar(true);
    }

    #region performance

    public int HealthRegenEveryFrames = 5;
    public int HealthRegenFrameCounter = 0;

    [SerializeField]
    public static float GrowingSpeed = 5f;
    [SerializeField]
    public static int UpdateScaleEveryFrames = 5;

    #endregion

    void Update()
    {
        if (GameManager.GamePaused)
            return;

        if (isAlive)
        {
            HealthRegenFrameCounter++;
            if (HealthRegenFrameCounter == HealthRegenEveryFrames)
            {
                HealthRegenFrameCounter = 0;
                Heal((HealthRegenPerSec + GameManager.Instance.CurrentDifficulty * HealthRegenPerSecPerDifficulty) * Time.deltaTime * HealthRegenEveryFrames);
            }
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
        
        for (int i = 0; i < Drops.Length; i++)
        {
            if (Drops[i].WantsToSpawn)
            {
                for (int a = 0; a < Drops[i].Amount; a++)
                {
                    string pool = Drops[i].Next().poolName;
                    EntitySpawnManager.Spawn(pool, transform.position, Quaternion.identity, queue: true);
                }
            }
        }

        EntitySpawnManager.Despawn(poolName, gameObject, true);
    }

    public void HealFull()
    {
        isAlive = true;
        SetHealth(MaxHealth);
    }
}
