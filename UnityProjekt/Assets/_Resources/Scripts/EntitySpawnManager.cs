using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public class SpawnQueueInfo
{
    public string poolName;
    public Vector3 position;
    public Quaternion rotation;
    public EntitySpawnManager.EntityEvent callBack;
    public bool countEntity;
}

public static class GeometryUtils
{
    public static float Distance(Vector3 point, Collider2D collider)
    {
        /*Vector3 center = collider.bounds.center;
        Vector3 size = collider.bounds.size;
        Rect rect =
                new Rect(
                    center.x - size.x / 2,
                    center.y - size.y / 2,
                    center.x + size.x / 2,
                    center.y + size.y / 2
                    );
        
         return Distance(point, rect);*/

        return Mathf.Max(Vector3.Distance(point, collider.bounds.center) - collider.bounds.size.x/2 - collider.bounds.size.y/2, 0.0f);
    }

    public static float Distance(Vector3 point, Rect rect)
    {
        float xDist = MinXDistance(point, rect);
        float yDist = MinYDistance(point, rect);
        if (xDist == 0)
        {
            return yDist;
        }
        else if (yDist == 0)
        {
            return xDist;
        }

        return new Vector3(xDist, yDist, 0f).magnitude;
        //return (float)Math.Sqrt(Math.Pow(xDist, 2) + Math.Pow(yDist, 2)); //Just normal magnitude of a Vector2
    }

    private static float MinXDistance(Vector3 point, Rect rect)
    {
        if (rect.x > point.x)
        {
            return rect.x - point.x;
        }
        else if (rect.width < point.x)
        {
            return point.x - rect.width;
        }
        else
        {
            //Im in it (x wise)
            return 0;
        }
    }

    private static float MinYDistance(Vector3 point, Rect rect)
    {
        if (rect.y < point.y)
        {
            return point.y - rect.y;
        }
        else if (rect.height > point.y)
        {
            return rect.height - point.y;
        }
        else
        {
            //Im in it (y wise)
            return 0;
        }
    }
}
#region HitAbleInfo
[System.Serializable]
public class HitAbleInfo : IComparable<HitAbleInfo>
{
    private Transform transform = null;
    public Transform Transform
    {
        get
        {
            if (hitAble && !transform)
                return hitAble.TargetTransform;
            return transform;
        }
        set
        {
            transform = value;
        }
    }

    public Collider2D Collider
    {
        get
        {
            return hitAble.MainCollider;
        }
    }

    public Vector3 ColliderCenter
    {
        get
        {
            return hitAble.ColliderCenter;
        }
    }

    public HitAbleType HitAbleType
    {
        get
        {
            return hitAble.hitAbleType;
        }
    }

    public HitAble hitAble;

    public float distance = 0f;

    public int CompareTo(HitAbleInfo other)
    {
        if (other == null)
        {
            return 1;
        }

        if (other.distance == this.distance)
        {
            return 0;
        }

        return this.distance < other.distance ? -1 : 1;
    }
}
#endregion


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

    public float DistanceBetween(Vector3 position, HitAble target, bool useColliderBounds)
    {
        float distance = 0f;
        if (useColliderBounds)
        {
            distance = GeometryUtils.Distance(position, target.MainCollider);
        }
        else
        {
            distance = Vector3.Distance(target.ColliderCenter, position);
        }
        return distance;
    }

    public HitAbleInfo GetNearestHitAbles(Vector3 globalPosition, HitAbleType mask, bool useColliderBounds, float maxDistance = -1)
    {
        if (SpawnedHitAbles.Count == 0)
            return null;

        HitAbleInfo nearest = new HitAbleInfo() { hitAble = null };

        float nearestDistance = -1;
        for (int i = 0; i < SpawnedHitAbles.Count; i++)
        {
            if (!SpawnedHitAbles[i].Transform)
                continue;
            if (!HitAble.CheckForBitInMask(SpawnedHitAbles[i].hitAble.hitAbleType, mask))
                continue;

            float distance = DistanceBetween(globalPosition, SpawnedHitAbles[i].hitAble, useColliderBounds);
            if ((maxDistance > 0 && distance < maxDistance) && (nearestDistance == -1 || distance < nearestDistance))
            {
                nearest = SpawnedHitAbles[i];
                nearest.distance = distance;
                nearestDistance = distance;
            }
        }
        return nearest;
    }

    public HitAbleInfo[] GetHitAbleInCircles(Vector3 globalPosition, HitAbleType mask, float radius, bool useColliderBounds, bool sorted = false)
    {
        if (SpawnedHitAbles.Count == 0)
            return new HitAbleInfo[0];

        List<HitAbleInfo> enemies = new List<HitAbleInfo>();

        for (int i = 0; i < SpawnedHitAbles.Count; i++)
        {
            if (!SpawnedHitAbles[i].Transform)
                continue;
            if (!HitAble.CheckForBitInMask(SpawnedHitAbles[i].HitAbleType, mask))
                continue;

            float distance = DistanceBetween(globalPosition, SpawnedHitAbles[i].hitAble, useColliderBounds);
            if (distance < radius)
            {
                SpawnedHitAbles[i].distance = distance;
                enemies.Add(SpawnedHitAbles[i]);
            }
        }

        if (sorted)
        {
            enemies.Sort();
        }
        return enemies.ToArray();
    }

    public HitAbleInfo GetNearestPlayers(Vector3 globalPosition, bool useColliderBounds, float range = -1)
    {
        if (GameManager.Instance.PlayerCount == 0)
            return null;
        
        HitAbleInfo nearest = null;

        float nearestDistance = -1;
        for (int i = 0; i < GameManager.Instance.GetPlayers().Length; i++)
        {
            if (GameManager.Instance.GetPlayers()[i] == null)
                continue;

            HitAble hitAble = GameManager.Instance.GetPlayers()[i];

            float distance = DistanceBetween(globalPosition, hitAble, useColliderBounds);
            if ((range > 0 && distance < range) && (nearestDistance == -1 || distance < nearestDistance))
            {
                if (nearest == null)
                {
                    nearest = new HitAbleInfo();
                }

                nearest.hitAble = hitAble;
                nearest.distance = distance;
                nearestDistance = distance;
            }
        }
        return nearest;
    }

    public HitAbleInfo[] GetPlayersInCircles(Vector3 globalPosition, float radius, bool useColliderBounds, bool sorted = false)
    {
        if (GameManager.Instance.PlayerCount == 0)
            return new HitAbleInfo[0];

        List<HitAbleInfo> players = new List<HitAbleInfo>();

        for (int i = 0; i < GameManager.Instance.GetPlayers().Length; i++)
        {
            if (GameManager.Instance.GetPlayers()[i] == null)
                continue;
            HitAble hitAble = GameManager.Instance.GetPlayers()[i];

            float distance = DistanceBetween(globalPosition, hitAble, useColliderBounds);
            if (distance < radius)
            {
                players.Add(new HitAbleInfo()
                    {
                        hitAble = hitAble,
                        distance = distance
                    });
            }
        }
        if (sorted)
        {
            players.Sort();
        }
        return players.ToArray();
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
            SpawnedHitAbles.Add(new HitAbleInfo { hitAble = script });
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
            if (SpawnedHitAbles[i].Transform == transform)
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
        Instance.SpawnedHitAbles.Add(new HitAbleInfo { hitAble = hitAble });
    }

    public static bool ContainsHitAble(HitAble hitAble)
    {
        for (int i = 0; i < Instance.SpawnedHitAbles.Count; i++)
        {
            if(Instance.SpawnedHitAbles[i].hitAble == hitAble)
            {
                return true;
            }
        }
        return false;
    }

    public static void RemoveHitAble(HitAble hitAble)
    {
        Instance.RemoveInfo(hitAble.transform);
    }
}
