using UnityEngine;
using System.Collections;

public class Drop : MonoBehaviour {

    public string poolName = "Drop";

    public Collider2D worldCollider;

    public float waitTime = 3f;

    public float maxSpeed = 20f;
    public float currentSpeed = 0f;

    public float SpeedChange = 1.0f;

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
        currentSpeed = 0f;
        flyToPlayer = false;
        Invoke("FlyToPlayer", waitTime);
    }

    void FlyToPlayer()
    {
        flyToPlayer = true;
        if(disableColliderOnFlight)
            worldCollider.enabled = false;
    }

    void FixedUpdate()
    {
        if (flyToPlayer)
        {
            rigidbody2D.gravityScale = 0f;
            currentSpeed = Mathf.Clamp(currentSpeed + Time.fixedDeltaTime * SpeedChange, 0f, maxSpeed);
            Vector3 direction = -(transform.position - GameManager.Instance.MainPlayer.TargetingPosition).normalized;
            rigidbody2D.AddForce(direction * currentSpeed);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TriggerHit(other);
        EntitySpawnManager.Despawn(poolName, gameObject, true);
    }

    public virtual void TriggerHit(Collider2D other)
    {

    }
}
