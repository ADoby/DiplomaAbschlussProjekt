using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct SpawnQueueInfo
{
    public string poolName;
    public Vector3 position;
    public Quaternion rotation;
    public EntitySpawnManager.EntityEvent callBack;
    public bool countEntity;
}

public class EntitySpawnManager : MonoBehaviour
{

    public delegate void EntityEvent(GameObject go);

    public static EntitySpawnManager instance;

    private Queue<SpawnQueueInfo> EntitySpawnQueue = new Queue<SpawnQueueInfo>();

    public float timing = 0.1f;

    public int SpawnsPerTick = 10;

    void Awake()
    {
        instance = this;
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnTimer());
	}

    private IEnumerator SpawnTimer()
    {
        if (GameManager.CanSpawnEntity && EntitySpawnQueue.Count > 0)
        {
            for (int i = 0; i < SpawnsPerTick; i++)
            {
                if (!GameManager.CanSpawnEntity || EntitySpawnQueue.Count == 0)
                    break;

                Spawn(EntitySpawnQueue.Dequeue());
            }
        }
        
        yield return new WaitForSeconds(timing);
        StartCoroutine(SpawnTimer());
    }

    public void Spawn(SpawnQueueInfo info)
    {
        GameObject go = GameObjectPool.Instance.Spawn(info.poolName, info.position, info.rotation);

        if (info.callBack != null)
        {
            info.callBack(go);
        }
        
        if(info.countEntity)
            GameManager.Instance.AddEntity();
    }

    public static void Spawn(string poolName, Vector3 position, Quaternion rotation, EntityEvent callBack = null, bool queue = false, bool forceDirectSpawn = false, bool countEntity = true)
    {
        if (forceDirectSpawn || (!queue && GameManager.CanSpawnEntity))
        {
            instance.Spawn(new SpawnQueueInfo() { poolName = poolName, position = position, rotation = rotation, callBack = callBack, countEntity = countEntity});
        }
        else
        {
            instance.EntitySpawnQueue.Enqueue(new SpawnQueueInfo() { poolName = poolName, position = position, rotation = rotation, callBack = callBack, countEntity = countEntity });
        }
    }

    public static void Despawn(string poolName, GameObject go, bool countEntity)
    {
        GameObjectPool.Instance.Despawn(poolName, go);

        if(countEntity)
            GameManager.Instance.RemoveEntity();
    }
}
