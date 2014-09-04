using UnityEngine;
using System.Collections;

public enum ItemType
{
    Item25,
    Item50,
    Item75,
    Item100
}

public class RandomItem : MonoBehaviour {

    public string poolName = "RandomItem";

    public ItemType itemType;

    public Collider2D worldCollider;

    public float waitTime = 2f;

    public float currentSpeed = 0f;
    public float SpeedChange = 1.0f;
    public float maxSpeed = 20f;

    public float gravity = 3f;

    [SerializeField]
    private bool flyToPlayer = false;

    public bool disableColliderOnFlight = true;

    public void SetPoolName(string newPoolName)
    {
        poolName = newPoolName;
    }

    public void Reset()
    {
        worldCollider.enabled = true;
        rigidbody2D.gravityScale = gravity;
        flyToPlayer = false;
        currentSpeed = 0f;
        Invoke("FlyToPlayer", waitTime);
    }

    void FlyToPlayer()
    {
        flyToPlayer = true;
        if (disableColliderOnFlight)
            worldCollider.enabled = false;
    }

    void FixedUpdate()
    {
        if (flyToPlayer)
        {
            rigidbody2D.gravityScale = 0f;
            currentSpeed = Mathf.Clamp(currentSpeed + Time.fixedDeltaTime * SpeedChange, 0f, maxSpeed);
            Vector3 diff = (transform.position - GameManager.Instance.MainPlayer.transform.position).normalized;
            rigidbody2D.AddForce(-diff * currentSpeed * Time.fixedDeltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other && other.gameObject && other.gameObject.tag == "Player")
        {
            other.gameObject.SendMessage("GenerateItem", itemType, SendMessageOptions.DontRequireReceiver);
            EntitySpawnManager.Despawn(poolName, gameObject, true);
        }
    }
}
