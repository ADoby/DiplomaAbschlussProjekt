using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public PlayerController player;

    public float damage = 10.0f;

    public float speed = 2.0f;

    private Vector2 direction = Vector2.zero;

    public string poolName;

    public string hitEffektPoolName;

    public float DespawnTime = 10.0f;
    private float _despawnTimer = 0.0f;

    private Vector3 savedVelocity = Vector3.zero;

    void Awake()
    {
        GameEventHandler.OnPause += OnPause;
        GameEventHandler.OnResume += OnResume;
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.GamePaused)
            return;

        //rigidbody2D.velocity = direction * speed * Time.fixedDeltaTime;

        _despawnTimer += Time.fixedDeltaTime;
        if (_despawnTimer >= DespawnTime)
        {
            Explode(null, Vector3.zero);
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
        _despawnTimer = 0;
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

    void OnCollisionEnter2D(Collision2D info)
    {
        Explode(info.gameObject, info.contacts[0].point);
    }

    private void Explode(GameObject other, Vector3 position)
    {
        if (other && other.GetComponent<EnemieController>())
        {
            other.GetComponent<EnemieController>().Damage(damage);
            other.GetComponent<EnemieController>().Hit(position);
            GameEventHandler.TriggerDamageDone(player, damage);
        }
        else if (other && other.GetComponent<EnemieBase>())
        {
            other.GetComponent<EnemieBase>().Damage(damage);
            other.GetComponent<EnemieBase>().Hit(position);
            GameEventHandler.TriggerDamageDone(player, damage);
        }


        //Effekt
        GameObjectPool.Instance.Spawn(hitEffektPoolName, transform.position, Quaternion.identity);

        rigidbody2D.velocity = Vector2.zero;
        GameObjectPool.Instance.Despawn(poolName, gameObject);
    }
}
