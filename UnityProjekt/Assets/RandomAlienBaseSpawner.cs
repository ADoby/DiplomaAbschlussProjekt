using UnityEngine;
using System.Collections;

public class RandomAlienBaseSpawner : MonoBehaviour {

    [SerializeThis]
    public SpawnInfo[] basePoolNames;

    public float spawnChance = 0.05f;
    public float maxDistanceDown = 5f;
    public float minSpace = 3f;

    public LayerMask GroundLayer;
    public LayerMask BaseLayer;

    public float minCD = 7f;
    public float maxCD = 20f;
    public float timer = 0f;

    public Vector2 Distances;

    void Start()
    {
        timer = Random.Range(minCD, maxCD);
    }

    void Update()
    {
        if (GameManager.GamePaused)
            return;

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = Random.Range(minCD, maxCD);
            TrySpawn();
        }
    }

    void TrySpawn()
    {
        if (Random.value < spawnChance)
        {
            var rnd = Random.value;
            for (int i = 0; i < basePoolNames.Length; i++)
            {
                if (rnd < basePoolNames[i].weight)
                {
                    TrySpawning(basePoolNames[i].poolName, transform.position - 
                        transform.right * Distances.x - 
                        transform.up * Distances.y + 
                        transform.right * (Random.value * Distances.x * 2f) + 
                        transform.up * (Random.value * Distances.y * 2f));
                    return;
                }
                rnd -= basePoolNames[i].weight;
            }
        }
    }

    public float spaceTop = 5f;

    public void TrySpawning(string poolName, Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position + Vector3.up, Vector3.down, maxDistanceDown, GroundLayer);

        if (hit)
        {
            if (Physics2D.Raycast(hit.point, hit.normal, spaceTop, GroundLayer))
                return;

            if (!Physics2D.OverlapCircle(hit.point, minSpace, BaseLayer))
            {
                EntitySpawnManager.Spawn(poolName, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            }

        }
    }
}
