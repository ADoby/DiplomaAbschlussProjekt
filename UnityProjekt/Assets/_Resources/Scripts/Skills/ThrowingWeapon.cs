using UnityEngine;
using System.Collections;

public class ThrowingWeapon : MonoBehaviour {

    public PlayerController player;

    public float damage = 10.0f;

    public float speed = 2.0f;

    [SerializeField]
    private Vector2 direction = Vector2.zero;

    public string poolName;

    public string hitEffektPoolName;

    [SerializeField]
    private float detonateTimer = 0f;
    public float detonateTime = 2f;

    public float powerUp = 2.0f;
    public float powerForward = 2.0f;

    public SoundEffect explosion;

    public void Reset()
    {
        detonateTimer = detonateTime;
    }

    void Update()
    {
        detonateTimer -= Time.deltaTime;
        if (detonateTimer <= 0)
        {
            Explode();
        }
    }

    void OnGUI()
    {
        if (detonateTimer > 0)
        {
            for (int i = 0; i < GameManager.Instance.GetCameras().Length; i++)
            {
                if (GameManager.Instance.GetCameras()[i] != null)
                {
                    Vector2 pos = GameManager.Instance.GetCameras()[i].camera.WorldToScreenPoint(transform.position);
                    pos.y = Screen.height - pos.y;
                    GUI.Label(new Rect(pos.x, pos.y - 20, 40, 40), detonateTimer.ToString("#0.#"));
                }
            }
        }
    }

    public void SetDamage(float p_damage)
    {
        damage = p_damage;
    }

    public void SetPlayer(PlayerController playerControl)
    {
        player = playerControl;
        detonateTimer = detonateTime;
    }

    public void SetDirection(Vector2 p_direction)
    {
        direction = p_direction;
        rigidbody2D.velocity = direction * (powerForward + Random.value * powerForward/2f)  + Vector2.up * powerUp;
        rigidbody2D.angularVelocity = rigidbody2D.velocity.magnitude * 90f;
    }

    public float ExplosionRange = 5f;
    public float force = 1.0f;

    public HitAbleType HitAbleHitMask;
    public void Explode()
    {
        HitAbleInfo[] HitAblesInRange = EntitySpawnManager.Instance.GetHitAbleInCircles(transform.position, HitAbleHitMask, ExplosionRange, true);
        //HitAbleInfo[] collider = EntitySpawnManager.Instance.GetHitAbleInCircles(transform.position, HitAbleHitMask, distance, true, false);
        for (int i = 0; i < HitAblesInRange.Length; i++)
        {
            HitAbleInfo item = HitAblesInRange[i];

            if (item == null || item.hitAble == null)
                continue;

            float distance = 0f;
            distance = item.distance;

            float damageMult = (ExplosionRange - distance);

            item.hitAble.Damage(new Damage()
            {
                type = DamageType.EXPLOSION,
                amount = damageMult * damage,
                DamageFromAPlayer = true,
                player = player,
                other = player.transform
            });
            item.hitAble.Hit(item.ColliderCenter, (item.ColliderCenter - transform.position), force);

            /*
            Debug.Log("Distance: " + HitAblesInRange[i].distance);
            HitAblesInRange[i].hitAble.Damage(new Damage()
            {
                DamageFromAPlayer = true,
                player = player,
                amount = Mathf.Min(distance - HitAblesInRange[i].distance, 0.0f),
                type = DamageType.EXPLOSION,
                other = player.transform
            });
            HitAblesInRange[i].hitAble.Hit(HitAblesInRange[i].ColliderCenter, (HitAblesInRange[i].ColliderCenter - transform.position), force);
             * */
        }

        AudioEffectController.Instance.PlayOneShot(explosion, transform.position);
        EntitySpawnManager.Spawn(hitEffektPoolName, transform.position, Quaternion.identity, forceDirectSpawn:true, countEntity:false);

        EntitySpawnManager.Despawn(poolName, gameObject, false);
    }
}
