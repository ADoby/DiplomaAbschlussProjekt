using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemieController : HitAble {

	
	public float FloorCheckDistance = 1.0f;
	public float FloorCheckLength = 0.5f;

	public float MyDamage = 5.0f;
	public float DamagePerDifficulty = 2.0f;


	[SerializeField]
	private int direction = 1;
	
	public float maxSpeed = 2.0f;
    public float MaxSpeedPerDifficulty = 0.2f;
    public float MaxMaxSpeed = 5f;
    public float CurrentMaxSpeed
    {
        get
        {
            return Mathf.Min(maxSpeed + MaxSpeedPerDifficulty * GameManager.Instance.CurrentDifficulty, MaxMaxSpeed);
        }
    }

	public float speedChange = 2.0f;
    public float SpeedChangePerDifficulty = 0.1f;

	[SerializeField]
	private float currentSpeed = 0.0f;

    public HitAbleInfo target = null;

	public float findTargetDistance = 20.0f;
	public float distanceToTarget = 1.0f;

	public float findTargetTime = 1.0f;
	public float findTargetTimer = 0f;

	public HitAbleType FindHitAbleMask;
    public HitAbleType AttackHitAbleMask;

    public LayerMask RaycastTargetMask;

	public float gravityMult = 3.0f;

	public Vector2 lastVelocity = Vector2.zero;


	[SerializeField]
	private float turnTimer = 0f;
	public float MinTurnTime = 0.5f, MaxTurnTime = 1f;

	public float jumpDownDistance = 2.0f;
	public float jumpUpDistance = 2.0f;

	public float jumpPower = 5.0f;

	public LayerMask pathFindingLayer;

	public float chanceForRandomJump = 0.1f;

	public Animator anim;

	[SerializeField]
	private float attackTimer = 0f;
	public float attackTime = 1.0f;

	[SerializeField]
	private float lockTimer = 0f;
	[SerializeField]
	private float lockTime = 0.5f;

	[SerializeField]
	private bool targetLocked = false;

	public float attackDistance = 2.0f;

	public float randomTimer = 0f;
	public float randomTimeMin = 3.0f, randomTimeMax = 10f;

	public float randomJumpTimer = 0f;
	public float randomJumpMinTime = 1f, randomJumpMaxTime = 3f;

	[SerializeField]
	private Vector3 targetPos = Vector3.zero;

	// Use this for initialization
	void Start ()
	{
		Reset();
	}

	void Awake()
	{
		Listen();
	}

	void OnEnable()
	{
		Listen();
	}

	void Listen()
	{
		GameEventHandler.FoundTarget += OnFoundTarget;
		GameEventHandler.OnPause += OnPause;
		GameEventHandler.OnResume += OnResume;
	}

	void OnDisable()
	{
		UnListen();
	}

	void OnDestroy()
	{
		UnListen();
	}

	void UnListen()
	{
		GameEventHandler.FoundTarget -= OnFoundTarget;
		GameEventHandler.OnPause -= OnPause;
		GameEventHandler.OnResume -= OnResume;
	}

	public void OnResume()
	{
		if (this == null)
		{
			UnListen();
		}


		rigidbody2D.isKinematic = false;
		rigidbody2D.velocity = lastVelocity;
	}

	public void OnPause()
	{
		if (this == null)
		{
			UnListen();
		}

		lastVelocity = rigidbody2D.velocity;
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.isKinematic = true;
	}

	public void MoneySpawned(GameObject go)
	{
		if (go)
		{
			go.rigidbody2D.velocity = new Vector3(Random.Range(-4, 4), Random.Range(10, 20), 0);
		}
	}   

	public void Reset()
	{
        base.Reset();
		if (CheckFloorRight())
		{
			direction = -1;
		}

		targetPos = transform.position;
	}

	// Update is called once per frame
	void Update () {
		if (GameManager.GamePaused)
			return;

		randomJumpTimer -= Time.deltaTime;

		findTargetTimer += Time.deltaTime;
		if (findTargetTimer > findTargetTime)
		{
			SearchForNewTarget();
			findTargetTimer = 0;
		}

        float currentMaxSpeed = CurrentMaxSpeed;
        float currentSpeedChance = speedChange + SpeedChangePerDifficulty * GameManager.Instance.CurrentDifficulty;
		if (GotHit)
		{
			currentMaxSpeed *= 1.5f;
			currentSpeedChance *= 1.5f;
		}

		if (target != null)
		{
            if (target.Transform == null)
            {
                target = null;
            }
            else
            {
                targetPos = target.Transform.position;
                if (Vector3.Distance(target.Transform.position, transform.position) > findTargetDistance)
                {
                    target = null;
                }
            }
		}
		else
		{
			if (Vector3.Distance(targetPos, transform.position) < distanceToTarget)
			{
				DumpPositionFinder();
			}
		}

		currentSpeed = Mathf.Lerp(currentSpeed, direction * currentMaxSpeed, currentSpeedChance * Time.deltaTime);

		TargetWalk();

		TryAttack();

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

	void TryAttack()
	{
		attackTimer -= Time.deltaTime;
		if (target == null)
		{
			targetLocked = false;
		}
		if (targetLocked)
		{
			if (Vector3.Distance(target.Transform.position, transform.position) > attackDistance)
			{
				targetLocked = false;

			}
			else
			{
				lockTimer -= Time.deltaTime;
				if (lockTimer <= 0)
				{
                    target.hitAble.Damage(
                        new Damage()
                        {
                            amount = MyDamage + GameManager.Instance.CurrentDifficulty * DamagePerDifficulty,
                            type = DamageType.MEELE,
                            other = transform
                        });
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
                HitAbleInfo nearestHitAble = EntitySpawnManager.Instance.GetNearestHitAbles(transform.position, AttackHitAbleMask, true, attackDistance);
                if (nearestHitAble.Transform)
                {
                    lockTimer = lockTime;
                    targetLocked = true;

                    if (anim)
                    {
                        anim.SetTrigger("Attack");
                    }

                    target = nearestHitAble;
                }
			}
		}
	}

    public float GravityScale = 2.0f;

	void FixedUpdate()
	{
		if (GameManager.GamePaused)
			return;

        rigidbody2D.velocity += Physics2D.gravity * Time.fixedDeltaTime * GravityScale;

		rigidbody2D.velocity = new Vector2(currentSpeed, rigidbody2D.velocity.y);
	}

	public void OnFoundTarget(Transform sender, HitAbleInfo newTarget)
	{
		if (target == null && newTarget != null && transform)
		{
            if (Vector3.Distance(sender.position, transform.position) > findTargetDistance)
			{
				target = newTarget;
			}
			else
			{
                target = new HitAbleInfo() { Transform = sender };
				//Follow the sender, maybe we will find one then
			}
		}
	}

	void SearchForNewTarget()
	{
		if (target == null)
		{
            HitAbleInfo[] playersInRange = EntitySpawnManager.Instance.GetPlayersInCircles(transform.position, findTargetDistance, true, true);
            for (int i = 0; i < playersInRange.Length; i++)
            {
                if (playersInRange[i].hitAble.ColliderIsOneOfYours(Physics2D.Raycast(transform.position, playersInRange[i].hitAble.ColliderCenter - transform.position, findTargetDistance, RaycastTargetMask).collider))
                {
                    target = playersInRange[i];
                    GameEventHandler.TriggerFoundTarget(transform, target);
                }
            }
		}
	}

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
		if ((targetPos - transform.position).x < -distanceToTarget)
		{
			if(Physics2D.Raycast(transform.position + Vector3.up * 0.1f, Vector3.left, 1f, pathFindingLayer))
			{
				TryJump();
			}
			if (rigidbody2D.velocity.y <= 0 && randomJumpTimer <= 0)
			{
				float random = Random.value;
				if (random < chanceForRandomJump)
				{
					randomJumpTimer = Random.Range(randomJumpMinTime, randomJumpMaxTime);
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
		else if ((targetPos - transform.position).x > distanceToTarget)
		{
			if (Physics2D.Raycast(transform.position + Vector3.up * 0.1f, Vector3.right, 1f, pathFindingLayer))
			{
				TryJump();
			}
			if (rigidbody2D.velocity.y <= 0 && randomJumpTimer <= 0)
			{
				float random = Random.value;
				if (random < chanceForRandomJump)
				{
					randomJumpTimer = Random.Range(randomJumpMinTime, randomJumpMaxTime);
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

	public void DumpPositionFinder()
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

}
