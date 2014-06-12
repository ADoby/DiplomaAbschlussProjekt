using UnityEngine;
using System.Collections;

public class DamageOnContact : MonoBehaviour {

    public float amount = 30f;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other && other.gameObject && other.gameObject.tag == "Player" && GetComponent<EnemieBase>())
        {
            GetComponent<EnemieBase>().Damage(amount);

            GameObjectPool.Instance.Spawn("Blood", transform.position + Vector3.up * 0.2f, Quaternion.identity);
        }
    }
    
}
