using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour {

    public PlayerController player;

    public float damage = 10.0f;

    public float speed = 2.0f;

    [SerializeField]
    private Vector2 direction = Vector2.zero;

    public string poolName;

    public string hitEffektPoolName;

    public float DespawnTime = 10.0f;
    [SerializeField]
    private float _despawnTimer = 0.0f;

    public float force = 2.0f;

    public void SetForce(float amount)
    {
        force = amount;
    }

    public int pierceAmount = 0;
    [SerializeField]
    private int pierceCount = 0;

    public void SetPierceAmount(int amount)
    {
        pierceAmount = amount;
    }

    private Vector3 savedVelocity = Vector3.zero;

    public LayerMask HitLayer;


    public SoundEffect ShootEffect;
    public SoundEffect HitEffect;

    private List<GameObject> gameObjectHitted = new List<GameObject>();

    void OnEnable()
    {
        GameEventHandler.OnPause += OnPause;
        GameEventHandler.OnResume += OnResume;
    }

    void OnDestroy()
    {
        UnListen();
    }
    void OnDisable()
    {
        UnListen();
    }

    void UnListen()
    {
        GameEventHandler.OnPause -= OnPause;
        GameEventHandler.OnResume -= OnResume;
    }

    void FixedUpdate()
    {
        if (GameManager.GamePaused)
            return;


        _despawnTimer += Time.fixedDeltaTime;
        if (_despawnTimer >= DespawnTime)
        {
            Hit(null, transform.position);
            return;
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rigidbody2D.velocity.normalized, rigidbody2D.velocity.magnitude * Time.fixedDeltaTime, HitLayer);
        for (int i = 0; i < hits.Length; i++)
        {
            Hit(hits[i].collider.gameObject, hits[i].point);
        }
    }

    public void OnPause()
    {
        savedVelocity = rigidbody2D.velocity;
        rigidbody2D.velocity = Vector2.zero;
    }

    public void OnResume()
    {
        rigidbody2D.velocity = savedVelocity;
    }

    //Invoked from GameObjectPool
    public void Reset()
    {
        gameObjectHitted.Clear();
        pierceCount = 0;
        _despawnTimer = 0;
        AudioEffectController.Instance.PlayOneShot(ShootEffect, transform.position);
    }

    public void SetDamage(float p_damage)
    {
        damage = p_damage;
    }

    public void SetPlayer(PlayerController playerControl)
    {
        player = playerControl;
    }

    public void SetDirection(Vector2 p_direction)
    {
        direction = p_direction;
        rigidbody2D.velocity = direction*speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Hit(other.gameObject, transform.position);
    }


    private void Hit(GameObject other, Vector3 position)
    {
        if (other && other.GetComponent<HitAble>())
        {
            if (gameObjectHitted.Contains(other))
            {
                return;
            }

            HitAble target = other.GetComponent<HitAble>();
            target.Damage(new Damage()
            {
                DamageFromAPlayer = true,
                player = player,
                amount = damage,
                type = DamageType.RANGED,
                other = player.transform
            });
            target.Hit(position, rigidbody2D.velocity * Time.fixedDeltaTime, force);

            gameObjectHitted.Add(other);

            AudioEffectController.Instance.PlayOneShot(HitEffect, transform.position);
        }
        else
        {
            EntitySpawnManager.InstantSpawn(hitEffektPoolName, position, Quaternion.identity, countEntity:false);

            EntitySpawnManager.Despawn(poolName, gameObject, false);
            return;
        }


        EntitySpawnManager.InstantSpawn(hitEffektPoolName, position, Quaternion.identity, countEntity: false);

        if (pierceCount >= pierceAmount)
        {
            rigidbody2D.velocity = Vector2.zero;
            GameObjectPool.Instance.Despawn(poolName, gameObject);
        }
        else
        {
            pierceCount++;
        }
    }
}
