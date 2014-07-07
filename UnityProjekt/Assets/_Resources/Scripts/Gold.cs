using UnityEngine;
using System.Collections;

public class Gold : MonoBehaviour {

    public string poolName = "Gold";
    public int amount = 10;

    public Collider2D worldCollider;

    public float waitTimer = 0f, waitTime = 3f;

    public float maxSpeed = 10f;
    private float speed = 0f;
    public float speedChange = 1f;

    public float gravity = 3f;

    public void Reset()
    {
        waitTimer = waitTime;
        worldCollider.enabled = true;
        rigidbody2D.gravityScale = gravity;
        speed = 0f;
    }

    void Update()
    {
        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0)
        {
            rigidbody2D.gravityScale = 0f;

            speed += speedChange * Time.deltaTime;
            speed = Mathf.Clamp(speed, 0, maxSpeed);

            worldCollider.enabled = false;
            Vector3 diff = (transform.position - GameManager.Instance.MainPlayer.transform.position);
            transform.position -= diff.normalized * Time.deltaTime * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other && other.gameObject && other.gameObject.tag == "Player")
        {
            other.gameObject.SendMessage("AddMoney", amount, SendMessageOptions.DontRequireReceiver);
            EntitySpawnManager.Despawn(poolName, gameObject);
        }
    }
}
