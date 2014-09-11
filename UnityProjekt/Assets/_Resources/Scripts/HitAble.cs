using UnityEngine;
using System.Collections;
using System.Linq;

#if (UNITY_EDITOR)
using UnityEditor;
#endif

[System.Flags]
public enum HitAbleType : int
{
    ALIEN = (1 << 0),
    ALIENBASE = (1 << 1),
    PLAYER = (1 << 2),
    PROPS = (1 << 3)
}

#if (UNITY_EDITOR)
public class EnumFlagsAttribute : PropertyAttribute
{
    public EnumFlagsAttribute() { }
}

[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        _property.intValue = EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
    }
}

[CustomPropertyDrawer(typeof(HitAbleType))]
public class HitAbleCustomDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        _property.intValue = EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
    }
}
#endif

[System.Serializable]
public class HitAble : MonoBehaviour
{
    #region Type
    public HitAbleType hitAbleType;

    public static bool CheckForBitInMask(int bit, int mask)
    {
        return ((mask & bit) != 0);
    }
    public static bool CheckForBitInMask(HitAbleType bit, HitAbleType mask)
    {
        return CheckForBitInMask((int)bit, (int)mask);
    }
    #endregion

    #region Health

    public HealthBar healthBar;

    public float CurrentHealth = 100f;
    public float StartMaxHealth = 100f;
    public float MaxHealthPerDifficulty = 10f;
    public float MaxHealth
    {
        get
        {
            return StartMaxHealth + MaxHealthPerDifficulty * GameManager.Instance.CurrentDifficulty;
        }
    }
    public float ProzentHealth()
    {
        return CurrentHealth / MaxHealth;
    }

    public float StartHealthRegen = 2.0f;
    public float HealthRegenPerDifficulty = 0.2f;
    public float HealthRegen
    {
        get
        {
            return StartHealthRegen + HealthRegenPerDifficulty * GameManager.Instance.CurrentDifficulty;
        }
    }

    #endregion

    public string poolName = "";

    //Effect on death
    public bool HitEffect = true;
    public string HitEffectPoolName = "Blood";

    public bool sendFurther = false;
    public HitAble reciever;

    public bool IsDead = false;
    public bool GotHit = false;

    public SpawnInfos[] Drops;

    #region Collider

    public Collider2D[] usedCollider;

    public Vector3 ColliderBounds
    {
        get
        {
            return MainCollider.bounds.size;
        }
    }

    public Collider2D MainCollider
    {
        get
        {
            if (usedCollider.Length > 0)
            {
                return usedCollider[0];
            }
            return null;
        }
        set
        {
            if (usedCollider.Length == 0)
                usedCollider = new Collider2D[1];
            usedCollider[0] = value;
        }
    }

    public Vector3 ColliderCenter
    {
        get
        {
            return MainCollider.bounds.center;
        }
    } 

    public bool ColliderIsOneOfYours(Collider2D targetCollider)
    {
        if (!targetCollider)
            return false;
        for (int i = 0; i < usedCollider.Length; i++)
        {
            if (targetCollider == usedCollider[i])
                return true;
        }
        return false;
    }

    #endregion

    private Transform targetTransform;
    public Transform TargetTransform
    {
        get
        {
            if (!MainCollider)
                return null;
            if (!targetTransform)
                targetTransform = MainCollider.transform;

            return targetTransform;
        }
    }

    public virtual void Start()
    {
        if (!MainCollider)
        {
            if (collider2D)
            {
                MainCollider = collider2D;
            }
        }
    }

    public virtual void Reset()
    {
        CurrentHealth = MaxHealth;
        healthBar.UpdateBar(CurrentHealth, MaxHealth, true);
        IsDead = false;
        GotHit = false;
    }

    public virtual void Update()
    {
        if (GameManager.GamePaused || IsDead || sendFurther)
            return;

        Heal(HealthRegen * Time.deltaTime);
    }

    public virtual void Hit(Vector3 HitPosition, Vector3 HitDirection, float forceAmount = 0f)
    {
        if (IsDead)
            return;

        if (sendFurther && reciever)
        {
            reciever.Hit(HitPosition, HitDirection, forceAmount);
            return;
        }

        if (HitEffect)
        {
            int layermask = 1 << gameObject.layer;
            RaycastHit2D hit = Physics2D.RaycastAll(HitPosition - HitDirection, HitDirection, HitDirection.magnitude * 2f, layermask).FirstOrDefault(o => o.collider == collider2D);
            if (hit)
            {
                EntitySpawnManager.Spawn(HitEffectPoolName, HitPosition, Quaternion.FromToRotation(Vector3.back, hit.normal), countEntity:false, forceDirectSpawn:true);
                if (forceAmount != 0)
                {
                    Force(HitPosition, HitDirection.normalized, forceAmount);
                }
            }
        }

        
    }

    public virtual void Die()
    {
        for (int i = 0; i < Drops.Length; i++)
        {
            if (Drops[i].WantsToSpawn)
            {
                for (int a = 0; a < Drops[i].Amount; a++)
                {
                    EntitySpawnManager.Spawn(Drops[i].Next().poolName, transform.position, Quaternion.identity, queue: true);
                }
            }
        }

        EntitySpawnManager.Despawn(poolName, gameObject, true);
        GameEventHandler.TriggerEntityDied(this);
    }

    public virtual void Heal(float amount)
    {
        if (IsDead)
            return;

        if (sendFurther && reciever)
        {
            reciever.Heal(amount);
            return;
        }

        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        healthBar.UpdateBar(CurrentHealth, MaxHealth);
    }

    public void HealFull()
    {
        CurrentHealth = MaxHealth;
        healthBar.UpdateBar(CurrentHealth, MaxHealth, true);
    }

    public virtual void Damage(Damage damage)
    {
        if (IsDead)
            return;

        GotHit = true;

        if (sendFurther && reciever)
        {
            reciever.Damage(damage);
            return;
        }

        damage.amount = Mathf.Min(damage.amount, CurrentHealth);
        CurrentHealth -= damage.amount;
        GameEventHandler.TriggerDamageDone(damage.other.GetComponent<PlayerController>(), damage);
        healthBar.UpdateBar(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
        {
            IsDead = true;
            Die();
        }
    }

    public void RecieveForce(Vector3 position, Vector3 direction, float amount)
    {
        if (IsDead)
            return;

        Force(position, direction, amount);
    }

    protected virtual void Force(Vector3 position, Vector3 direction, float amount)
    {
        if (IsDead)
            return;

        if (sendFurther && reciever)
            reciever.RecieveForce(position, direction, amount);

        if(rigidbody2D)
            rigidbody2D.AddForceAtPosition(direction * amount, position, ForceMode2D.Impulse);
    }
}
