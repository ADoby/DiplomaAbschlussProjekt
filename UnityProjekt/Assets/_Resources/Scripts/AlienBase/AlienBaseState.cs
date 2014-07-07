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

    public string Name = "BaseState";

    public float startTime = 0.0f, endTime = 0.0f;

    public float healthBonus = 0.0f;
    private float timer = 0.0f;

    private AlienBase myBase;

    public void SetBase(AlienBase newBase)
    {
        myBase = newBase;
    }

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

    public AlienBaseState(AlienBase value)
    {
        myBase = value;
    }

    public void Reset()
    {
        timer = 0;
    }

    public void Spawn()
    {
        var rnd = Random.value;
        foreach (var item in spawnInfos)
        {
            if (rnd < item.weight)
            {
                if (myBase)
                {
                    EntitySpawnManager.Spawn(item.poolName, myBase.transform.position, true);
                }
                   
                return;
            }
            rnd -= item.weight;
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

