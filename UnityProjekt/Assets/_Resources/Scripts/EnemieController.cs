using UnityEngine;
using System.Collections;

public class EnemieController : HitAble {

    public HealthBar healthBar;

    public float Health = 100f;
    public float maxHealth = 0f;
    
    public float FloorCheckDistance = 1.0f;
    public float FloorCheckLength = 0.5f;

    public float MyDamage = 5.0f;

    private int direction = 1;
    
    public float maxSpeed = 2.0f;
    public float speedChange = 2.0f;

    private float currentSpeed = 0.0f;

    public Transform target = null;

    public float findTargetDistance = 20.0f;
    public float distanceToTarget = 1.0f;
    public float findTargetTime = 1.0f;

    public LayerMask findTargetLayer;

    public string poolName = "";

    public float gravityMult = 3.0f;

    public Vector2 lastVelocity = Vector2.zero;

	// Use this for initialization
	void Start ()
    {
        Reset();

       
	}

    void Awake()
    {
        maxHealth = Health;

        GameEventHandler.FoundTarget += OnFoundTarget;
        GameEventHandler.OnPause += OnPause;
        GameEventHandler.OnResume += OnResume;
    }

    public void OnResume()
    {
        rigidbody2D.velocity = lastVelocity;
    }

    public void OnPause()
    {
        lastVelocity = rigidbody2D.velocity;
        rigidbody2D.velocity = Vector2.zero;
    }

    public void MoneySpawned(GameObject go)
    {
        if (go)
        {
            go.rigidbody2D.velocity = new Vector3(Random.Range(-4, 4), Random.Range(10, 20), 0);
        }
    }

    public override void Damage(float damage)
    {
        base.Damage(damage);

        Health -= damage;
        healthBar.UpdateBar(Health, maxHealth);
        if (Health <= 0)
        {
            int amount = Random.Range(3, 7);
            for (int i = 0; i < amount; i++)
            {
                EntitySpawnManager.Spawn("Gold", transform.position, Quaternion.identity, MoneySpawned, true);
            }

            //TODO Sterbe
            EntitySpawnManager.Despawn(poolName, gameObject, true);
            GameEventHandler.TriggerEnemieDied(this);
        }
    }

    public void Reset()
    {
        if (CheckFloorRight())
        {
            direction = -1;
        }
        Health = maxHealth;

        StartCoroutine(findTarget());

        lockTime = attackAnim.length;

        healthBar.Reset();
    }

    public AnimationClip attackAnim;

    public Animator anim;

    private float attackTimer = 0f;
    public float attackTime = 1.0f;

    private float lockTimer = 0f;
    private float lockTime = 0.5f;

    private bool targetLocked = false;

    public float attackDistance = 2.0f;

    public float randomTimer = 0f;
    public float randomTimeMin = 3.0f, randomTimeMax = 10f;

    private Vector3 targetPos = Vector3.zero;

	// Update is called once per frame
	void Update () {
        if (GameManager.Instance.GamePaused)
            return;

        if (target)
        {
            //We save last targetPos
            //Its like remembering where the target was the last time I saw him
            targetPos = target.position;

            //We check the distance to the target
            if (Vector3.Distance(target.position, transform.position) > findTargetDistance)
            {
                //Target is to far away, lets throw him out, we cant see him
                target = null;
            }
        }
        else
        {
            targetPos = transform.position;
        }

        if (Vector3.Distance(targetPos, transform.position) < distanceToTarget)
        {
            //DumpWalk updates the targetPos to something new;
            DumpWalk();
        }

        TargetWalk();

        currentSpeed = Mathf.Lerp(currentSpeed, direction * maxSpeed, speedChange * Time.deltaTime);

        attackTimer -= Time.deltaTime;
        if (!target)
        {
            targetLocked = false;
        }
        if (targetLocked)
        {
            if (Vector3.Distance(target.position, transform.position) > attackDistance)
            {
                targetLocked = false;

            }
            else
            {
                
                lockTimer -= Time.deltaTime;
                if (lockTimer <= 0)
                {
                    target.GetComponent<PlayerController>().Damage(MyDamage);
                    //Effekt

                    targetLocked = false;

                }
            }
            attackTimer = attackTime;
        }
        else
        {
            if (attackTimer <= 0)
            {
                Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, attackDistance, findTargetLayer);

                //Collider2D[] collider = Physics2D.OverlapAreaAll(transform.position + new Vector3(-attackDistance / 2f, 2, 0), transform.position + new Vector3(attackDistance/2f, 2, 0), findTargetLayer);
                foreach (var item in collider)
                {
                    if (item.gameObject.GetComponent<PlayerController>())
                    {
                        lockTimer = lockTime;
                        targetLocked = true;

                        if (anim)
                        {
                            anim.SetTrigger("Attack");
                        }

                        target = item.transform;
                        break;
                    }
                }

            }
        }

        if (anim)
        {
            anim.SetFloat("Forward", Mathf.Abs(direction));
        }

        if (direction == 1 && transform.localScale.x == -1)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else if (direction == -1 && transform.localScale.x == 1)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }

	}

    void FixedUpdate()
    {
        if (GameManager.Instance.GamePaused)
        {

            return;
        }
            

        rigidbody2D.velocity = new Vector2(currentSpeed, rigidbody2D.velocity.y);
    }

    public void OnFoundTarget(Transform sender, Transform newTarget)
    {
        if (!target && newTarget)
        {
            if (Vector3.Distance(newTarget.position, transform.position) > findTargetDistance)
            {
                target = newTarget;
            }
            else
            {
                target = sender;
                //Follow the sender, maybe we will find one then
            }
        }
    }

    IEnumerator findTarget()
    {
        if (!target)
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, findTargetDistance, findTargetLayer);
            if (targets.Length != 0)
            {
                target = targets[0].transform;
                GameEventHandler.TriggerFoundTarget(transform, target);
            }
        }

        yield return new WaitForSeconds(findTargetTime);
        StartCoroutine(findTarget());
    }

    private float turnTimer = 0f;
    public float MinTurnTime = 0.5f, MaxTurnTime = 1f;

    public float jumpDownDistance = 2.0f;
    public float jumpUpDistance = 2.0f;

    public float jumpPower = 5.0f;

    public LayerMask pathFindingLayer;

    public float chanceForRandomJump = 0.1f;

    public void TryJump()
    {
        if (Physics2D.Raycast(transform.position, Vector3.down, 0.05f, pathFindingLayer))
        {
            //Jump
            rigidbody2D.velocity = Vector2.up * jumpPower * rigidbody2D.gravityScale * jumpUpDistance;
        }
    }

    public void TargetWalk()
    {
        if ((targetPos - transform.position).x < -distanceToTarget && CheckFloorLeft())
        {
            if(Physics2D.Raycast(transform.position + Vector3.up * 0.1f, Vector3.left, 1f, pathFindingLayer))
            {
                TryJump();
            }
            if (rigidbody2D.velocity.y <= 0)
            {
                float random = Random.Range(0f, 1000f);
                if (random < chanceForRandomJump)
                {
                    TryJump();
                }
            }
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                turnTimer = Random.Range(MinTurnTime, MaxTurnTime);
                //links von uns
                direction = -1;
            }
        }
        else if ((targetPos - transform.position).x > distanceToTarget && CheckFloorRight())
        {
            if (Physics2D.Raycast(transform.position + Vector3.up * 0.1f, Vector3.right, 1f, pathFindingLayer))
            {
                TryJump();
            }
            if (rigidbody2D.velocity.y <= 0)
            {
                float random = Random.Range(0f, 1000f);
                if (random < chanceForRandomJump)
                {
                    TryJump();
                }
            }
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                
                turnTimer = Random.Range(MinTurnTime, MaxTurnTime);
                //links von uns
                direction = 1;
            }
            
        }
    }

    public void DumpWalk()
    {
        randomTimer -= Time.deltaTime;
        if (randomTimer <= 0)
        {
            direction *= -1;
            if (direction == 0)
                direction = Random.Range(-1, 1);
            randomTimer = Random.Range(randomTimeMin, randomTimeMax);
        }
        
        targetPos = transform.position + Vector3.right * direction * distanceToTarget * 2f;
    }

    public bool CheckFloorRight()
    {
        bool isFloor = Physics2D.Raycast(new Vector2(transform.position.x + FloorCheckDistance, transform.position.y), -Vector2.up, FloorCheckLength);
        bool isJumpDown = Physics2D.Raycast(new Vector2(transform.position.x + FloorCheckDistance, transform.position.y), -Vector2.up, jumpDownDistance);
        return (isFloor || isJumpDown);
    }

    public bool CheckFloorLeft()
    {
        bool isFloor = Physics2D.Raycast(new Vector2(transform.position.x - FloorCheckDistance, transform.position.y), -Vector2.up, FloorCheckLength);
        bool isJumpDown = Physics2D.Raycast(new Vector2(transform.position.x - FloorCheckDistance, transform.position.y), -Vector2.up, jumpDownDistance);
        return (isFloor || isJumpDown);
    }

    public void SetHealth(float newHealth)
    {
        maxHealth = newHealth;
        Health = newHealth;
    }
    public void SetDamage(float newDamage)
    {
        MyDamage = newDamage;
    }
}
