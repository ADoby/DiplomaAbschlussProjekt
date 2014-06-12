using UnityEngine;
using System.Collections;

public class SpawningBase : MonoBehaviour {

    public string[] spawningPoolNames;


    public float spawnTime = 3f;
    public float DistanceCheck = 10f;

    private float spawnTimer = 0f;

    public float checkForPlayerTime = 2f;

    private bool seePlayer = false;

    public LayerMask checkPlayerMask;

	// Use this for initialization
	void Start () {
        if (spawningPoolNames.Length == 0)
            enabled = false;

        StartCoroutine(CheckForPlayer());
	}

    IEnumerator CheckForPlayer()
    {

        seePlayer = Physics2D.OverlapArea(transform.position + Vector3.left * DistanceCheck + Vector3.up * 2f, transform.position + Vector3.right * DistanceCheck, checkPlayerMask);

        yield return new WaitForSeconds(checkForPlayerTime);
        StartCoroutine(CheckForPlayer());
    }

	// Update is called once per frame
	void Update () {
        spawnTimer -= Time.deltaTime;

        if (seePlayer && spawnTimer <= 0)
        {
            spawnTimer = spawnTime;
            int enemieID = Random.Range(0, spawningPoolNames.Length-1);

            GameObject go = GameObjectPool.Instance.Spawn(spawningPoolNames[enemieID], transform.position + Vector3.up, Quaternion.identity);
            go.SendMessage("Reset", SendMessageOptions.DontRequireReceiver);
        }
	}
}
