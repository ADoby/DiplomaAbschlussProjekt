using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public PlayerController player;

    public float damage = 10.0f;

    public float speed = 2.0f;

    private Vector2 direction = Vector2.zero;

    public string poolName;

    public string hitEffektPoolName;

    void LateUpdate()
    {
        if (Game.Paused)
        {
            rigidbody2D.velocity = Vector2.zero;
        }
        else
        {
            rigidbody2D.velocity = direction * speed;
        }
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
    }

    void OnTriggerEnter2D(Collider2D info)
    {
        Explode(info.gameObject);
    }

    void OnCollisionEnter2D(Collision2D info)
    {
        Explode(info.gameObject);
    }

    private void Explode(GameObject other)
    {
        if (other && other.GetComponent<EnemieController>())
        {
            other.GetComponent<EnemieController>().Damage(damage);
            GameEventHandler.TriggerDamageDone(player, damage);
        }
        else if (other && other.GetComponent<EnemieBase>())
        {
            other.GetComponent<EnemieBase>().Damage(damage);
            GameEventHandler.TriggerDamageDone(player, damage);
        }


        //Effekt
        GameObjectPool.Instance.Spawn(hitEffektPoolName, transform.position, Quaternion.identity);

        GameObjectPool.Instance.Spawn("Blood", transform.position, Quaternion.identity);


        GameObjectPool.Instance.Despawn(poolName, gameObject);
    }
}
