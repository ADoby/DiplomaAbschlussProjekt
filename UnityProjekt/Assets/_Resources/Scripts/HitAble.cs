using UnityEngine;
using System.Collections;
using System.Linq;

[System.Serializable]
public class HitAble : MonoBehaviour {

    public bool sendFurther = false;
    public HitAble reciever;

    
    public Collider2D usedCollider;

    public bool SpawnPool = true;
    public string SpawnPoolName = "Blood";

    void Start()
    {
        if (!usedCollider)
        {
            if (collider2D)
            {
                usedCollider = collider2D;
            }
        }
    }

    public virtual void Hit(Vector3 HitPosition, Vector3 HitDirection, float forceAmount = 0f)
    {
        if (sendFurther && reciever)
            reciever.Hit(HitPosition, HitDirection, forceAmount);

        if (SpawnPool)
        {
            int layermask = 1 << gameObject.layer;
            RaycastHit2D hit = Physics2D.RaycastAll(HitPosition - HitDirection, HitDirection, HitDirection.magnitude * 2f, layermask).FirstOrDefault(o => o.collider == collider2D);
            if (hit)
            {
                EntitySpawnManager.Spawn(SpawnPoolName, HitPosition, Quaternion.FromToRotation(Vector3.back, hit.normal), countEntity:false, forceDirectSpawn:true);
                if (forceAmount != 0)
                {
                    Force(HitPosition, HitDirection.normalized, forceAmount);
                }
            }
        }

        
    }

    public virtual void Damage(float amount)
    {
        if (sendFurther && reciever)
            reciever.Damage(amount);
    }

    public void RecieveForce(Vector3 position, Vector3 direction, float amount)
    {
        Force(position, direction, amount);
    }

    protected virtual void Force(Vector3 position, Vector3 direction, float amount)
    {
        if (sendFurther && reciever)
            reciever.RecieveForce(position, direction, amount);

        if(rigidbody2D)
            rigidbody2D.AddForceAtPosition(direction * amount, position, ForceMode2D.Impulse);
    }
}
