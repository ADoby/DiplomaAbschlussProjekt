using UnityEngine;
using System.Collections;

public class RandomItem25 : MonoBehaviour {

    public string poolName = "RandomItem25";

    public Collider2D worldCollider;

    public float waitTime = 3f;

    public float maxSpeed = 12f;

    public float gravity = 3f;

    [SerializeField]
    private bool flyToPlayer = false;

    public bool disableColliderOnFlight = true;

    public void Reset()
    {
        worldCollider.enabled = true;
        rigidbody2D.gravityScale = gravity;
        flyToPlayer = false;
        Invoke("FlyToPlayer", waitTime);
    }

    void FlyToPlayer()
    {
        flyToPlayer = true;
        if (disableColliderOnFlight)
            worldCollider.enabled = false;
    }

    void Update()
    {
        if (flyToPlayer)
        {
            rigidbody2D.gravityScale = 0f;
            Vector3 diff = (transform.position - GameManager.Instance.MainPlayer.transform.position);
            rigidbody2D.AddForce(-diff * maxSpeed);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other && other.gameObject && other.gameObject.tag == "Player")
        {
            other.gameObject.SendMessage("GenerateItem25", SendMessageOptions.DontRequireReceiver);
            EntitySpawnManager.Despawn(poolName, gameObject, true);
        }
    }
}
