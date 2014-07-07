using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct SpawnQueueInfo
{
    public string poolName;
    public Vector3 position;
    public Quaternion rotation;
    public EntitySpawnManager.EntityEvent callBack;
}

public class EntitySpawnManager : MonoBehaviour
{

    public delegate void EntityEvent(GameObject go);

    public static EntitySpawnManager instance;

    private Queue<SpawnQueueInfo> EntitySpawnQueue = new Queue<SpawnQueueInfo>();

    public float timing = 0.3f;

    void Awake()
    {
        instance = this;
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnTimer());
	}

    IEnumerator SpawnTimer()
    {
        if (GameManager.CanSpawnEntity && EntitySpawnQueue.Count > 0)
        {
            while (true)
            {
                if (!GameManager.CanSpawnEntity && EntitySpawnQueue.Count == 0)
                {
                    break;
                }
                Spawn(EntitySpawnQueue.Dequeue());
            }
        }
        
        yield return new WaitForSeconds(timing);
        SpawnTimer();
       //StartCoroutine(SpawnTimer());
    }

    public void Spawn(SpawnQueueInfo info)
    {
        GameObject go = GameObjectPool.Instance.Spawn(info.poolName, info.position, info.rotation);

        if (info.callBack != null)
        {
            info.callBack(go);
        }
        
        GameManager.Instance.AddEntity();
    }

    public void SpawnEntity(string poolName, Vector3 position, Quaternion rotation, EntityEvent callBack)
    {
        GameObject go = GameObjectPool.Instance.Spawn(poolName, position, rotation);

        if (callBack != null)
        {
            callBack(go);
        }

        GameManager.Instance.AddEntity();
    }

    public static void Spawn(string poolName, Vector3 position, bool queue = false)
    {
        Spawn(poolName, position, Quaternion.identity, null, queue);
    }
    public static void Spawn(string poolName, Vector3 position, EntityEvent callBack, bool queue = false)
    {
        Spawn(poolName, position, Quaternion.identity, callBack, queue);
    }
    public static void Spawn(string poolName, Vector3 position, Quaternion rotation, EntityEvent callBack, bool queue = false)
    {
        if (!queue && !GameManager.CanSpawnEntity)
            return;

        if (GameManager.CanSpawnEntity)
        {
            instance.Spawn(new SpawnQueueInfo() { poolName = poolName, position = position, rotation = rotation, callBack = callBack });
        }
        else
        {
            instance.EntitySpawnQueue.Enqueue(new SpawnQueueInfo() { poolName = poolName, position = position, rotation = rotation, callBack = callBack});
        }
    }

    public static void Despawn(string poolName, GameObject go)
    {
        GameObjectPool.Instance.Despawn(poolName, go);
        GameManager.Instance.RemoveEntity();
    }
}
