using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {

	public PlayerController player;
	public float damage = 10.0f;
	public SoundEffect explosion;
	public string hitEffektPoolName = "GrenadeExplosion";

	public string poolName = "SelfFindingRocket";

	public LayerMask targetLayer;
	public LayerMask sightLayer;
	public LayerMask navLayer;

	public float WantedWallDistance = 5f;

	public int flyingDirectionX = 0;
	public int flyingDirectionY = 0;

	public Collider2D target;
	public Transform targetTransform;

	public Vector3 targetPos;

	

	public float MaxSightRange = 20f;

	public Quaternion wantedRotation;
	public float RotateSpeed = 2.0f;

	public float MaxMovementAngle = 60f;

	public float MaxMovementSpeed = 4.0f;

	public float ExplosionRange = 5f;
	public float force;

	public float upOrDown = 0;
	public float rightOrLeft = 0;

	public float currentSpeed;
	public float DiffAngle;

	public float TargetChangeSpeed = 2.0f;

	public float MinDistanceForMaxSpeed = 1.0f;

	public bool exploded = false;

	

	public Vector3 savedVelocity;

	#region Performance

	public float LifeTime = 10.0f;
	public float LifeTimer = 0f;

	public float UpdateTargetTime = 1.0f;
	public float UpdateTargetTimer = 0f;

	public float FindNewTargetTime = 1f;
	public float FindNewTargetTimer = 0f;

	public float UpdateRandomTime = 1.0f;
	public float UpdateRandomTimer = 0f;

	#endregion

	// Use this for initialization
	void Reset () {
		target = null;
		collider2D.enabled = true;
		targetPos = transform.position;
		exploded = false;
		LifeTimer = 0f;
		NewRandomFlyingDirection();
		FindNewTarget();
	}

	void Start()
	{
		GameEventHandler.OnPause += OnGamePaused;
		GameEventHandler.OnResume += OnGameResumed;
	}

	public void OnGamePaused()
	{
		savedVelocity = rigidbody2D.velocity;
		rigidbody2D.velocity = Vector3.zero;
	}

	public void OnGameResumed()
	{
		rigidbody2D.velocity = savedVelocity;
	}

	public void NewRandomFlyingDirection()
	{
		flyingDirectionX = Random.value > 0.5f ? 1 : -1;
		flyingDirectionY = Random.value > 0.5f ? 1 : -1;
	}

	// Update is called once per frame
	void Update () 
	{
		if (GameManager.GamePaused)
			return;

		LifeTimer += Time.deltaTime;
		if (LifeTimer >= LifeTime)
		{
			//Die
			TryExplode();
			return;
		}

		UpdateTarget();

		transform.rotation = Quaternion.Lerp(transform.rotation, wantedRotation, Time.deltaTime * RotateSpeed);
	}

	void FixedUpdate()
	{
		if (GameManager.GamePaused)
			return;

		UpdateVelocity();
	}

	public bool SomeThingInDirection()
	{
		return Physics2D.Raycast(transform.position, Vector3.right * flyingDirectionX, WantedWallDistance, navLayer);
	}

	public void Impulse(Vector2 force)
	{
		rigidbody2D.AddForce(force, ForceMode2D.Impulse);
	}

	public void UpdateTarget()
	{
		FindNewTargetTimer += Time.deltaTime;
		UpdateTargetTimer += Time.deltaTime;
		if (UpdateTargetTimer >= UpdateTargetTime)
		{
			if (target)
			{
				if (Physics2D.Raycast(transform.position, (target.bounds.center - transform.position), MaxSightRange, sightLayer).collider != target)
				{
					//target out of sight;
					target = null;
				}
			}
			if (!target)
			{
				if (FindNewTargetTimer >= FindNewTargetTime)
				{
					StartCoroutine(FindNewTarget());
					FindNewTargetTime = 0f;
				}
			}
			UpdateTargetTimer = 0f;
		}
		
		if (target)
		{
			targetPos = targetTransform.position;
			UpdateRotation();
		}
		else
		{
			UpdateRandomTimer += Time.deltaTime;
			if (UpdateRandomTimer >= UpdateRandomTime)
			{
				UpdateRandomTimer = 0f;
				FindRandomPos();
			}
		}
	}

	public float RightLeftAmount = 3f;
	public float UpDownAmount = 3f;

	public void FindRandomPos()
	{
		UpOrDown();
		RightOrLeft();

		targetPos = transform.position;
		targetPos += Vector3.right * rightOrLeft * RightLeftAmount;
		targetPos += Vector3.up * UpOrDownValue() * UpDownAmount;
		UpdateRotation();
	}

	public void UpdateRotation()
	{
		Vector3 addToTargetPos = Vector3.zero;
		if (target)
		{
			addToTargetPos += transform.up * RightAndLeftFront(2f) * 1.0f;
		}
		else
		{
			addToTargetPos += transform.up * RightAndLeftFront();
			addToTargetPos += Vector3.up * FrontUpAndDown();
		}

		Vector3 dir = (targetPos + addToTargetPos) - transform.position;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		wantedRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		
	}

	public float distance = 0f;

	public void UpdateVelocity()
	{
		DiffAngle = Quaternion.Angle(transform.rotation, wantedRotation);
		currentSpeed = Mathf.Abs(Mathf.Clamp((DiffAngle - MaxMovementAngle) / MaxMovementAngle, 0f, 1f)-1f);
		currentSpeed *= MaxMovementSpeed;

		distance = Vector3.Distance(transform.position, targetPos);

		currentSpeed *= (Mathf.Clamp(distance, 0f, MinDistanceForMaxSpeed) / MinDistanceForMaxSpeed);

		rigidbody2D.AddForce(transform.right * currentSpeed * Time.fixedDeltaTime);
	}

	IEnumerator FindNewTarget()
	{
		HitAbleInfo[] enemiesInRange = EntitySpawnManager.Instance.GetHitAbleInCircle(transform.position, MaxSightRange);
		for (int i = 0; i < enemiesInRange.Length; i++)
		{
			HitAble enemie = enemiesInRange[i].hitAble;
			if (Physics2D.Raycast(transform.position, (enemie.usedCollider.bounds.center - transform.position), MaxSightRange, sightLayer).collider == enemie.usedCollider)
			{
				target = enemie.usedCollider;
				targetTransform = enemie.transform;
				break;
			}
		}

		/*
		Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, MaxSightRange, targetLayer);
		for (int i = 0; i < hits.Length; i++)
		{
			if (Physics2D.Raycast(transform.position, (hits[i].bounds.center - transform.position), MaxSightRange, sightLayer).transform == hits[i].transform)
			{
				target = hits[i].transform;
				break;
			}
		}
		*/
		yield return null;
	}

	public float FrontUpAndDown(float rangeMult = 1f)
	{
		float dist = (WantedWallDistance * rangeMult);
		RaycastHit2D up = Physics2D.Raycast(transform.position, Vector3.right * flyingDirectionX + Vector3.up * 0.5f, dist, navLayer);
		RaycastHit2D down = Physics2D.Raycast(transform.position, Vector3.right * flyingDirectionX + Vector3.down * 0.5f, dist, navLayer);

		return (Mathf.Abs(up.distance - dist) / dist) - (Mathf.Abs(down.distance - dist) / dist);
	}

	public float RightAndLeftFront(float rangeMult = 1f)
	{
		float dist = (WantedWallDistance * rangeMult);
		RaycastHit2D right = Physics2D.Raycast(transform.position, transform.right * 0.25f + transform.up, dist, navLayer);
		RaycastHit2D left = Physics2D.Raycast(transform.position, transform.right * 0.25f - transform.up, dist, navLayer);

		return (Mathf.Abs(right.distance - dist) / dist) - (Mathf.Abs(left.distance - dist) / dist);
	}

	public float UpOrDownValue(float rangeMult = 1f)
	{
		float dist = (WantedWallDistance * rangeMult);
		RaycastHit2D up = Physics2D.Raycast(transform.position, Vector3.up, dist, navLayer);
		RaycastHit2D down = Physics2D.Raycast(transform.position, Vector3.down, dist, navLayer);

		return (Mathf.Abs(up.distance - dist) / dist) - (Mathf.Abs(down.distance - dist) / dist);
	}

	public float RightOrLeftValue(float rangeMult = 1f)
	{
		float dist = (WantedWallDistance * rangeMult);
		RaycastHit2D right = Physics2D.Raycast(transform.position, Vector3.right, dist, navLayer);
		RaycastHit2D left = Physics2D.Raycast(transform.position, Vector3.left, dist, navLayer);

		return (Mathf.Abs(right.distance - dist) / dist) - (Mathf.Abs(left.distance - dist) / dist);
	}

	public float UpOrDown(float rangeMult = 1f)
	{
		float dist = (WantedWallDistance * rangeMult);
		RaycastHit2D up = Physics2D.Raycast(transform.position, Vector3.up, dist, navLayer);
		RaycastHit2D down = Physics2D.Raycast(transform.position, Vector3.down, dist, navLayer);

		if (!up && !down)
		{
			upOrDown = Mathf.Lerp(upOrDown, 0f, UpdateRandomTime * TargetChangeSpeed);
		}
		else
		{
			//upOrDown = 0;
			if (up)
				upOrDown = Mathf.Lerp(upOrDown, -Mathf.Abs(up.distance - dist) / dist, Time.deltaTime * TargetChangeSpeed);
			if (down)
				upOrDown = Mathf.Lerp(upOrDown, Mathf.Abs(down.distance - dist) / dist, Time.deltaTime * TargetChangeSpeed);
		}

		return upOrDown;
	}

	public float RightOrLeft(float rangeMult = 1f)
	{
		float dist = (WantedWallDistance * rangeMult);

		RaycastHit2D right = Physics2D.Raycast(transform.position, Vector3.right, dist, navLayer);
		RaycastHit2D left = Physics2D.Raycast(transform.position, Vector3.left, dist, navLayer);

		if (right)
		{
			flyingDirectionX = -1;
		}
		if (left)
		{
			flyingDirectionX = 1;
		}

		rightOrLeft = Mathf.Lerp(rightOrLeft, flyingDirectionX, UpdateRandomTime * TargetChangeSpeed);

		return rightOrLeft;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		TryExplode();
	}

	public void TryExplode()
	{
		if (!exploded)
		{
			collider2D.enabled = true;
			exploded = true;
			Explode();
		}
	}

	public void Explode()
	{
		HitAbleInfo[] collider = EntitySpawnManager.Instance.GetHitAbleInCircle(transform.position, ExplosionRange);

		foreach (var item in collider)
		{
			float distanceToTarget = Vector2.Distance(item.transform.position, transform.position);

			float damageMult = (ExplosionRange - distanceToTarget);

            item.hitAble.Damage(new Damage()
            {
                type = DamageType.EXPLOSION,
                amount = damageMult * damage,
                DamageFromAPlayer = true,
                player = player,
                other = player.transform
            });
			item.hitAble.Hit(item.transform.position, (item.transform.position - transform.position), force);
		}

		AudioEffectController.Instance.PlayOneShot(explosion, transform.position);
		EntitySpawnManager.InstantSpawn(hitEffektPoolName, transform.position, Quaternion.identity, countEntity:false);

		GameObjectPool.Instance.Despawn(poolName, gameObject);
	}
}
