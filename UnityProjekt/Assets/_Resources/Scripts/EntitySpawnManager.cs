using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct SpawnQueueInfo
{
    public string poolName;
    public Vector3 position;
    public Quaternion rotation;
    public EntitySpawnManager.EntityEvent callBack;
    public bool countEntity;
}

[System.Serializable]
public class EntitySpawnManager : MonoBehaviour
{

    public delegate void EntityEvent(GameObject go);

    public static EntitySpawnManager instance;

    [SerializeThis]
    private Queue<SpawnQueueInfo> EntitySpawnQueue = new Queue<SpawnQueueInfo>();

    public float timing = 0.1f;
    [SerializeThis]
    private float timer = 0;

    public int SpawnsPerTick = 10;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = timing;
            TrySpawning();
        }
    }

    private void TrySpawning()
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
    }

    public void Spawn(SpawnQueueInfo info)
    {
        GameObject go = GameObjectPool.Instance.Spawns(info.poolName, info.position + Vector3.forward * 0.0001f * GameManager.Instance.CurrentSpawnedEntityCount, info.rotation);

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
