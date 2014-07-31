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

    public float distance = 5f;
    public float force = 1.0f;

    public void Explode()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, distance);
        foreach (var item in collider)
        {
            if (item.GetComponent<HitAble>())
            {
                HitAble target = item.GetComponent<HitAble>();

                float distanceToTarget = Vector2.Distance(item.transform.position, transform.position);

                float damageMult = (distance - distanceToTarget);

                target.Damage(damageMult * damage);
                target.Hit(item.transform.position, (item.transform.position - transform.position), force);

                GameEventHandler.TriggerDamageDone(player, damage);                
            }
        }

        AudioEffectController.Instance.PlayOneShot(explosion, transform.position);
        GameObjectPool.Instance.Spawns(hitEffektPoolName, transform.position, Quaternion.identity);

        GameObjectPool.Instance.Despawn(poolName, gameObject);
    }
}
