using UnityEngine;
using System.Collections;

public class EnemieSpawner : MonoBehaviour {

    public Transform levelFloor;

    public float chance = 0.3f;
    public float spawnTime = 1.0f;
    public bool paused = false;

    public float defaultHealth = 100f;
    public float defaultDamage = 2f;

    public float timedHealthMult = 1.0f;
    public float timedDamageMult = 1.0f;

    public float timedValue = 0f;

    public int livingEnemies = 0;

    public int maxEnemies = 30;

	// Use this for initialization
	void Start () {
        StartCoroutine(CheckSpawn());

        GameEventHandler.OnPause += OnPause;
        GameEventHandler.OnResume += OnResume;
        GameEventHandler.EnemieDied += EnemieDied;
	}

    public void EnemieDied(EnemieController enemie)
    {
        livingEnemies--;
    }

    public void OnPause()
    {
        paused = true;
    }

    public void OnResume()
    {
        paused = false;
    }

    IEnumerator CheckSpawn()
    {
        if (!paused)
        {
            timedValue += spawnTime;


            if (livingEnemies < maxEnemies)
            {
                BoxCollider2D[] floors = levelFloor.GetComponentsInChildren<BoxCollider2D>();

                if (floors.Length != 0)
                {
                    BoxCollider2D item;

                    do
                    {
                        item = floors[Random.Range(0, floors.Length - 1)];
                    } while (Random.value < chance);

                    float y = item.transform.position.y + item.size.y / 2f + item.center.y;
                    float minX = item.transform.position.x - (item.size.x * item.transform.localScale.x) / 2f + item.center.x;
                    float maxX = item.transform.position.x + (item.size.x * item.transform.localScale.x) / 2f + item.center.x;

                    float x = Random.Range(minX, maxX);

                    GameObject go = GameObjectPool.Instance.Spawn("Enemie1", new Vector2(x, y), Quaternion.identity);
                    go.SendMessage("SetHealth", timedHealthMult * timedValue + defaultHealth);
                    go.SendMessage("SetDamage", timedDamageMult * timedValue + defaultDamage);

                    livingEnemies++;
                }
            }
        }
        
        yield return new WaitForSeconds(spawnTime);
        StartCoroutine(CheckSpawn());
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
