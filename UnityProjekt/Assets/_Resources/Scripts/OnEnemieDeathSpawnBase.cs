using UnityEngine;
using System.Collections;

public class OnEnemieDeathSpawnBase : MonoBehaviour
{

    public SpawnInfo[] basePoolNames;

    public float spawnChance = 0.5f;
    public float maxDistanceDown = 5f;
    public float minSpace = 3f;

    public LayerMask GroundLayer;
    public LayerMask BaseLayer;

	// Use this for initialization
	void Start ()
	{
	    GameEventHandler.EnemieDied += OnEnemieDied;
	}

    public void OnEnemieDied(EnemieController enemie)
    {
        if (Random.value < spawnChance)
        {
            var rnd = Random.value;
            for (int i = 0; i < basePoolNames.Length; i++)
            {
                if (rnd < basePoolNames[i].weight)
                {
                    TrySpawning(basePoolNames[i].poolName, enemie.transform.position);
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
            else
            {
                Debug.Log("Something in the way to spawn");
            }
                
        }
    }
}
