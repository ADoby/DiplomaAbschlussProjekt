using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemieController : HitAble {

	public HealthBar healthBar;

	[SerializeField]
	private float Health = 100f;
	public float StartMaxHealth = 100f;
	public float MaxHealthPerDifficulty = 10f;
	[SerializeField]
	private float MaxHealth = 100f;
	
	public float FloorCheckDistance = 1.0f;
	public float FloorCheckLength = 0.5f;

	public float MyDamage = 5.0f;
	public float DamagePerDifficulty = 2.0f;

	[SerializeField]
	private int direction = 1;
	
	public float maxSpeed = 2.0f;
	public float speedChange = 2.0f;

	[SerializeField]
	private float currentSpeed = 0.0f;

	public Transform target = null;

	public float findTargetDistance = 20.0f;
	public float distanceToTarget = 1.0f;

	public float findTargetTime = 1.0f;
	public float findTargetTimer = 0f;

	public LayerMask findTargetLayer;

	public string poolName = "";

	public float gravityMult = 3.0f;

	public Vector2 lastVelocity = Vector2.zero;

	public SpawnInfos[] Drops;

	public bool aggressive = false;

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

	public override void Damage(float damage)
	{
		base.Damage(damage);

		aggressive = true;

		Health -= damage;
		healthBar.UpdateBar(Health, MaxHealth);
		if (Health <= 0)
		{
			for (int i = 0; i < Drops.Length; i++)
			{
				if (Drops[i].WantsToSpawn)
				{
					for (int a = 0; a < Drops[i].Amount; a++)
					{
						EntitySpawnManager.Spawn(Drops[i].Next().poolName, transform.position, Quaternion.identity, queue: true);
					}
				}
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
		MaxHealth = StartMaxHealth + GameManager.Instance.CurrentDifficulty * MaxHealthPerDifficulty;
		Health = MaxHealth;

		healthBar.Reset();

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
			findTarget();
			findTargetTimer = 0;
		}

		float currentMaxSpeed = maxSpeed;
		float currentSpeedChance = speedChange;
		if (aggressive)
		{
			currentMaxSpeed *= 1.5f;
			currentSpeedChance *= 1.5f;
		}

		if (target)
		{
			targetPos = target.position;
			if (Vector3.Distance(target.position, transform.position) > findTargetDistance)
			{
				target = null;
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
					target.GetComponent<PlayerController>().Damage(MyDamage + GameManager.Instance.CurrentDifficulty * DamagePerDifficulty);
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
	}

	void FixedUpdate()
	{
		if (GameManager.GamePaused)
			return;

		rigidbody2D.velocity += Physics2D.gravity * Time.fixedDeltaTime;

		rigidbody2D.velocity = new Vector2(currentSpeed, rigidbody2D.velocity.y);
	}

	public void OnFoundTarget(Transform sender, Transform newTarget)
	{
		if (!target && newTarget && transform)
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

	void findTarget()
	{
		if (!target)
		{
			Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, findTargetDistance, findTargetLayer);
			if (targets.Length != 0)
			{
				for (int i = 0; i < targets.Length; i++)
				{
					if (Physics2D.Raycast(transform.position, targets[i].bounds.center, findTargetDistance, findTargetLayer).transform == targets[i].transform)
					{
						target = targets[i].transform;
						GameEventHandler.TriggerFoundTarget(transform, target);
					}
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
