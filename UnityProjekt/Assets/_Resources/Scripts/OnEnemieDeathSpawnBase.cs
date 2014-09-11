using UnityEngine;
using System.Collections;

public class OnEnemieDeathSpawnBase : MonoBehaviour
{

    [SerializeThis]
    public SpawnInfo[] basePoolNames;

    public float spawnChance = 0.5f;
    public float maxDistanceDown = 5f;
    public float minSpace = 3f;

    public LayerMask GroundLayer;
    public LayerMask BaseLayer;

    void OnEnable()
    {
        GameEventHandler.EntityDied -= OnEntityDied;
        GameEventHandler.EntityDied += OnEntityDied;
    }

    void OnDisable()
    {
        GameEventHandler.EntityDied -= OnEntityDied;
    }

    void OnDestroy()
    {
        GameEventHandler.EntityDied -= OnEntityDied;
    }

    public void OnEntityDied(HitAble entity)
    {
        if (entity.hitAbleType != HitAbleType.ALIEN)
            return;

        if (Random.value < spawnChance)
        {
            var rnd = Random.value;
            for (int i = 0; i < basePoolNames.Length; i++)
            {
                if (rnd < basePoolNames[i].weight)
                {
                    TrySpawning(basePoolNames[i].poolName, entity.transform.position);
                    return;
                }
                rnd -= basePoolNames[i].weight;
            }
        }
    }

    public void TrySpawning(string poolName, Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position + Vector3.up, Vector3.down, maxDistanceDown, GroundLayer);

        if (hit)
        {
            if (!Physics2D.OverlapCircle(hit.point, minSpace, BaseLayer))
            {
                EntitySpawnManager.Spawn(poolName, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            }
                
        }
    }
}
