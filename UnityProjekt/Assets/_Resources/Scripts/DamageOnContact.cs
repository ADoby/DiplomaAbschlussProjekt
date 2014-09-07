using UnityEngine;
using System.Collections;

public class DamageOnContact : MonoBehaviour {

    public float amount = 30f;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other && other.gameObject && other.gameObject.tag == "Player" && GetComponent<EnemieBase>())
        {
            GetComponent<EnemieBase>().Damage(new Damage()
            {
                amount = amount,
                type = DamageType.MEELE,
                other = transform
            });

            EntitySpawnManager.InstantSpawn("Blood", transform.position + Vector3.up * 0.2f, Quaternion.identity, countEntity:false);
        }
    }
    
}
