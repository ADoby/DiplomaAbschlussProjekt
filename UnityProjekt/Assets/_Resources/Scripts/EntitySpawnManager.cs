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
public struct HitAbleInfo
{
    public Transform transform;
    public HitAble hitAble;
}

[System.Serializable]
public class EntitySpawnManager : MonoBehaviour
{

    public delegate void EntityEvent(GameObject go);

    public static EntitySpawnManager Instance;

    [SerializeThis]
    private Queue<SpawnQueueInfo> EntitySpawnQueue = new Queue<SpawnQueueInfo>();

    [SerializeField]
    List<HitAbleInfo> SpawnedHitAbles = new List<HitAbleInfo>();

    public float timing = 0.1f;
    [SerializeThis]
    private float timer = 0;

    public int SpawnsPerTick = 10;

    void Awake()
    {
        Instance = this;
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

    public HitAbleInfo GetNearestHitAble(Vector3 globalPosition, float range = -1)
    {
        HitAbleInfo nearest = new HitAbleInfo() { transform = null, hitAble = null };
        if (SpawnedHitAbles.Count == 0)
            return nearest;

        float nearestDistance = -1;
        for (int i = 0; i < SpawnedHitAbles.Count; i++)
        {
            if (!SpawnedHitAbles[i].transform)
                continue;

            float distance = Vector3.Distance(SpawnedHitAbles[i].transform.position, globalPosition);
            if ((range > 0 && distance < range) && (nearestDistance == -1 || distance < nearestDistance))
            {
                nearest = SpawnedHitAbles[i];
                nearestDistance = distance;
            }
        }
        return nearest;
    }

    public HitAbleInfo[] GetHitAbleInCircle(Vector3 globalPosition, float radius)
    {
        if (SpawnedHitAbles.Count == 0)
            return new HitAbleInfo[0];

        List<HitAbleInfo> enemies = new List<HitAbleInfo>();

        for (int i = 0; i < SpawnedHitAbles.Count; i++)
        {
            if (!SpawnedHitAbles[i].transform)
                continue;

            if (Vector3.Distance(SpawnedHitAbles[i].transform.position, globalPosition) < radius)
            {
                enemies.Add(SpawnedHitAbles[i]);
            }
        }

        return enemies.ToArray();
    }

    public HitAbleInfo GetNearestPlayer(Vector3 globalPosition, float range = -1)
    {
        HitAbleInfo nearest = new HitAbleInfo() { transform = null, hitAble = null };
        if (SpawnedHitAbles.Count == 0)
            return nearest;

        float nearestDistance = -1;
        for (int i = 0; i < GameManager.Instance.GetPlayers().Length; i++)
        {
            if (GameManager.Instance.GetPlayers()[i] == null)
                continue;
            HitAble hitAble = GameManager.Instance.GetPlayers()[i];

            float distance = Vector3.Distance(hitAble.transform.position, globalPosition);
            if ((range > 0 && distance < range) && (nearestDistance == -1 || distance < nearestDistance))
            {
                nearest.hitAble = hitAble;
                nearest.transform = hitAble.transform;
                nearestDistance = distance;
            }
        }
        return nearest;
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

    public GameObject Spawn(SpawnQueueInfo info)
    {
        GameObject go = GameObjectPool.Instance.Spawn(info.poolName, info.position + Vector3.forward * 0.0001f * GameManager.Instance.CurrentSpawnedEntityCount, info.rotation);

        if (!go)
        {
            return null;
        }

        go.SendMessage("SetPoolName", info.poolName, SendMessageOptions.DontRequireReceiver);

        HitAble script = go.GetComponent<HitAble>();

        if (script)
        {
            SpawnedHitAbles.Add(new HitAbleInfo { transform = go.transform, hitAble = script });
        }

        if (info.callBack != null)
        {
            info.callBack(go);
        }
        
        if(info.countEntity)
            GameManager.Instance.AddEntity();

        return go;
    }

    public static void Spawn(string poolName, Vector3 position, Quaternion rotation, EntityEvent callBack = null, bool queue = false, bool forceDirectSpawn = false, bool countEntity = true)
    {
        if (forceDirectSpawn || (!queue && GameManager.CanSpawnEntity))
        {
            Instance.Spawn(new SpawnQueueInfo() { poolName = poolName, position = position, rotation = rotation, callBack = callBack, countEntity = countEntity});
        }
        else
        {
            Instance.EntitySpawnQueue.Enqueue(new SpawnQueueInfo() { poolName = poolName, position = position, rotation = rotation, callBack = callBack, countEntity = countEntity });
        }
    }

    public static GameObject InstantSpawn(string poolName, Vector3 position, Quaternion rotation, bool countEntity = true)
    {
        return Instance.Spawn(new SpawnQueueInfo() { poolName = poolName, position = position, rotation = rotation, callBack = null, countEntity = countEntity });
    }

    public void RemoveInfo(Transform transform)
    {
        for (int i = 0; i < SpawnedHitAbles.ToArray().Length; i++)
        {
            if (SpawnedHitAbles[i].transform == transform)
            {
                SpawnedHitAbles.Remove(SpawnedHitAbles[i]);
                break;
            }
        }
    }

    public void DespawnI(string poolName, GameObject go, bool countEntity)
    {
        if (go.GetComponent<HitAble>())
        {
            RemoveInfo(go.transform);
        }

        GameObjectPool.Instance.Despawn(poolName, go);

        if (countEntity)
            GameManager.Instance.RemoveEntity();
    }

    public static void Despawn(string poolName, GameObject go, bool countEntity)
    {
        Instance.DespawnI(poolName, go, countEntity);
    }

    public static void AddHitAble(HitAble hitAble)
    {
        Instance.SpawnedHitAbles.Add(new HitAbleInfo { transform = hitAble.transform, hitAble = hitAble });
    }

    public static void RemoveHitAble(HitAble hitAble)
    {
        Instance.RemoveInfo(hitAble.transform);
    }
}
