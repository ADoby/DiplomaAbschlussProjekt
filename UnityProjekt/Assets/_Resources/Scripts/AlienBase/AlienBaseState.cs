using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnInfo
{
    public string poolName = "";
    public float weight = 1.0f;
}

[System.Serializable]
public class AlienBaseState
{
    public bool attacking, spawning;

    [SerializeField]
    public List<SpawnInfo> spawnInfos = new List<SpawnInfo>();

    public float spawnCooldown = 2.0f;
    private float spawnTimer = 0.0f;

    public float Range = 10f;

    public string Name = "BaseState";

    public float startTime = 0.0f, endTime = 0.0f;

    public float healthBonus = 0.0f;
    private float timer = 0.0f;

    public LayerMask mask;

    public Transform BaseTransform { get; set; }

    public float currentTime
    {
        get { return timer + startTime; }
        set
        {
            if (value <= endTime && value >= startTime)
            {
                timer = value;
            }
        }
    }

    public void Reset()
    {
        timer = 0;
    }

    public void Spawn()
    {
        if (Physics2D.OverlapCircle(BaseTransform.position, Range, mask))
        {
            var rnd = Random.value;
            foreach (var item in spawnInfos)
            {
                if (rnd < item.weight)
                {
                    EntitySpawnManager.Spawn(item.poolName, BaseTransform.position, Quaternion.identity);

                    return;
                }
                rnd -= item.weight;
            }
        }
    }

    public void Update()
    {
        if (spawning)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                Spawn();
                spawnTimer = spawnCooldown;
            }
        }
    }
}

