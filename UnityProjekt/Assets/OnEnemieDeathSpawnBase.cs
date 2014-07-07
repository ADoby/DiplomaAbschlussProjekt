using UnityEngine;
using System.Collections;

public class OnEnemieDeathSpawnBase : MonoBehaviour
{

    public SpawnInfo[] basePoolNames;

    public LayerMask GroundLayer;

	// Use this for initialization
	void Start ()
	{
	    GameEventHandler.EnemieDied += OnEnemieDied;
	}

    public void OnEnemieDied(EnemieController enemie)
    {
        if (Random.value < 0.2f)
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
        RaycastHit2D hit = Physics2D.Raycast(position + Vector3.up, Vector3.down, float.PositiveInfinity,
                        GroundLayer);

        if (hit)
        {
            GameObject go = GameObjectPool.Instance.Spawn(poolName, hit.point, Quaternion.Euler(hit.normal));
            //go.GetComponent<AlienBase>().SetCurrentTime(0);
            //go.GetComponent<EnemieBase>().Health = 0;
        }
    }
}
