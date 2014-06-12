using UnityEngine;
using System.Collections;

public class ThrowingWeapon : MonoBehaviour {

    public PlayerController player;

    public float damage = 10.0f;

    public float speed = 2.0f;

    private Vector2 direction = Vector2.zero;

    public string poolName;

    public string hitEffektPoolName;

    private float detonateTimer = 0f;
    public float detonateTime = 2f;

    public float powerUp = 2.0f;
    public float powerForward = 2.0f;

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
            //cam.WorldToScreenPoint(mouseOverItem.renderer.bounds.center);
            Vector2 pos = Camera.main.WorldToScreenPoint(renderer.bounds.center);
            pos.y = Screen.height - pos.y;
            GUI.Label(new Rect(pos.x, pos.y - 20, 40, 40), detonateTimer.ToString("#.#"));
        }
    }

    public void SetDamage(float p_damage)
    {
        damage = p_damage;
    }

    public void SetPlayer(PlayerController playerControl)
    {
        player = playerControl;
        detonateTimer = detonateTime * playerControl.playerClass.GetAttributeValue(AttributeType.ATTACKSPEED);
    }

    public void SetDirection(Vector2 p_direction)
    {
        direction = p_direction;
        rigidbody2D.velocity = direction * powerForward + Vector2.up * powerUp;
    }

    public float distance = 5f;
    public float force = 1.0f;

    public void Explode()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, distance);
        foreach (var item in collider)
        {
            float distanceToTarget = Vector2.Distance(item.transform.position, transform.position);

            float damageMult = (distance - distanceToTarget);

            item.SendMessage("Damage", damageMult * damage, SendMessageOptions.DontRequireReceiver);
            item.SendMessage("AddForce", damageMult * force, SendMessageOptions.DontRequireReceiver);

            GameEventHandler.TriggerDamageDone(player, damage);
        }

        GameObject go = GameObjectPool.Instance.Spawn(hitEffektPoolName, transform.position, Quaternion.identity);
        

        GameObjectPool.Instance.Despawn(poolName, gameObject);
    }
}
