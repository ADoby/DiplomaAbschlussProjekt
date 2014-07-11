using UnityEngine;
using System.Collections;

public class HitAble : MonoBehaviour {

    public bool sendFurther = false;
    public HitAble reciever;

    public bool SpawnPool = true;
    public string SpawnPoolName = "Blood";

    public virtual void Hit(Vector3 HitPosition, Vector3 HitterPosition)
    {
        if (sendFurther && reciever)
            reciever.Hit(HitPosition, HitterPosition);

        if (SpawnPool)
        {
            Debug.Log(gameObject.name + " Got Hit");
            Vector3 direction = (HitPosition - HitterPosition);
            RaycastHit2D hit = Physics2D.Raycast(HitterPosition, direction, direction.magnitude * 2f);
            if (hit)
                GameObjectPool.Instance.Spawn(SpawnPoolName, hit.point, Quaternion.FromToRotation(Vector3.back, hit.normal));
        }
    }

    public virtual void Damage(float amount)
    {
        if (sendFurther && reciever)
            reciever.Damage(amount);
    }

    public virtual void Force(Vector3 HitterPosition, float amount)
    {
        if (sendFurther && reciever)
            reciever.Force(HitterPosition, amount);

        if(rigidbody2D)
            rigidbody2D.AddForce((transform.position - HitterPosition) * amount);
    }
}
