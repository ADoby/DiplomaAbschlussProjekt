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

	public Transform target;

	public Vector3 targetPos;

	

	public float MaxSightRange = 20f;

	public Quaternion wantedRotation;
	public float RotateSpeed = 2.0f;

	public float MaxMovementAngle = 60f;

	public float MaxMovementSpeed = 4.0f;

	public float ExplosionRange = 5f;
	public float force;

	public float RandomUpDownTimer = 0f;
	public float RandomUpDownTiming = 2f;

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

	public float UpdateRotationTime = 1.0f;
	public float UpdateRotationTimer = 0f;

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

		UpdateRotationTimer += Time.deltaTime;
		if (UpdateRotationTimer >= UpdateRotationTime)
		{
			UpdateRotation();
			UpdateRotationTimer = 0f;
		}

		transform.rotation = Quaternion.Lerp(transform.rotation, wantedRotation, UpdateRotationTimer * RotateSpeed);
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

		if (target)
		{
			if (Physics2D.Raycast(transform.position, (target.collider2D.bounds.center - transform.position), MaxSightRange, sightLayer).transform != target)
			{
				//target out of sight;
				target = null;
			}
		}
		if (!target)
		{
			UpdateTargetTimer += Time.deltaTime;
			if (UpdateTargetTimer >= UpdateTargetTime)
			{
				StartCoroutine(FindNewTarget());
				UpdateTargetTimer = 0f;
			}
		}
		if (target)
		{
			targetPos = target.position;
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

	}

	public void UpdateRotation()
	{
		if (target)
		{
			targetPos += transform.up * RightAndLeftFront() * 2.0f;
		}
		else
		{
			targetPos += transform.up * RightAndLeftFront();
			targetPos += Vector3.up * FrontUpAndDown();
		}

		Vector3 dir = targetPos - transform.position;
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
		Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, MaxSightRange, targetLayer);
		for (int i = 0; i < hits.Length; i++)
		{
			if (Physics2D.Raycast(transform.position, (hits[i].bounds.center - transform.position), MaxSightRange, sightLayer).transform == hits[i].transform)
			{
				target = hits[i].transform;
				break;
			}
		}
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
		RaycastHit2D right = Physics2D.Raycast(transform.position, transform.right + transform.up, dist, navLayer);
		RaycastHit2D left = Physics2D.Raycast(transform.position, transform.right - transform.up, dist, navLayer);

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
			upOrDown = Mathf.Lerp(upOrDown, 0f, Time.deltaTime * TargetChangeSpeed);
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

		rightOrLeft = Mathf.Lerp(rightOrLeft, flyingDirectionX, Time.deltaTime * TargetChangeSpeed);

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
		Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, ExplosionRange, targetLayer);
		foreach (var item in collider)
		{
			if (item.GetComponent<HitAble>())
			{
				HitAble target = item.GetComponent<HitAble>();

				float distanceToTarget = Vector2.Distance(item.transform.position, transform.position);

				float damageMult = (ExplosionRange - distanceToTarget);

				target.Damage(damageMult * damage);
				target.Hit(item.transform.position, (item.transform.position - transform.position), force);

				GameEventHandler.TriggerDamageDone(player, damage);
			}
		}

		AudioEffectController.Instance.PlayOneShot(explosion, transform.position);
		GameObjectPool.Instance.Spawns(hitEffektPoolName, transform.position, Quaternion.identity);

		GameObjectPool.Instance.Despawn(poolName, gameObject);
	}
}
