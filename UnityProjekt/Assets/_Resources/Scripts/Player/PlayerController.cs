using System.Text;
using UnityEngine;

[System.Serializable]
public class PlayerController : MonoBehaviour
{

    #region Public Members
    public int PlayerId = 0;
    public string Name = "Player1";
    public int Money = 0;

    public int MoneyPerSecond = 1;
    private float MoneyTimer = 0f;

    public PlayerClass PlayerClass;

    [Range(0f, 1.0f)]
    public float MinXMovement = 0.1f;

    public float PrevNeededExperience = 0;
    public float NeededExperience = 100f;
    public float NeededExperiencePerLevel = 50f;
    public float NeededExperienceMultPerLevel = 1.5f;
    public float ExperiencePerDamageDone = 2.0f;
    public float MinUpMotionForExtraJump = 0.2f;

    public float friction = 1f;

    public string JumpInput = "_JUMP",
                    Skill1Input = "_SKILL1",
                    Skill2Input = "_SKILL2",
                    Skill3Input = "_SKILL3",
                    Skill4Input = "_SKILL4";

    public LayerMask GroundCheckLayer;

    public bool Crouching { get { return _crouching; } }

    #endregion

    #region Private Member
    public Animator _animator;

    [SerializeField]
    private bool jumping = false;

    public int Level = 0;
    public float CurrentExperience = 0;
    public Vector2 _currentVelocity;
    private Vector2 _currentInput;
    public bool Grounded;

    [SerializeField]
    private Vector2 groundNormal = Vector2.up;

    [SerializeField]
    private bool _crouching = false;

    [SerializeField]
    private Vector2 _savedVelocity = Vector2.zero;

    public int _skillMakesImune = 0;
    private bool Imune { get { return _skillMakesImune > 0; } }
    public int _skillPreventsMovement = 0;
    private bool CanMove { get { return _skillPreventsMovement == 0; } }
    public int _skillOverridesMovement = 0;
    private bool MovedBySkill { get { return _skillOverridesMovement > 0; } }
    public int _skillPreventsSkillUsement = 0;
    private bool CanUseSkill { get { return _skillPreventsSkillUsement == 0; } }

    //Some Const used
    private const float Half = 0.5f;

    public float CheckPointCooldown = 10.0f;
    [SerializeField]
    private float checkPointTimer = 0f;

    public float BackToCheckPointCooldown = 20.0f;
    [SerializeField]
    private float backToCheckPointTimer = 0f;

    public float ProcentageCheckpointTimer
    {
        get
        {
            return checkPointTimer / CheckPointCooldown;
        }
    }

    #endregion

    public Transform SpotlightTransform;

    public byte[] LastSave;

    public void Init()
    {
        PlayerClass.Init(this);

        Listen();

        transform.position = GameManager.GetLevelSpawnPoint();

        ResetPlayer();
    }

    void OnEnable()
    {
        Listen();
    }

    void OnDisable()
    {
        UnListen();
    }

    void OnDestroy()
    {
        UnListen();
    }

    public void Listen()
    {
        GameEventHandler.OnDamageDone += DamageDone;
        GameEventHandler.OnPause += OnPause;
        GameEventHandler.OnResume += OnResume;
    }

    public void UnListen()
    {
        GameEventHandler.OnDamageDone -= DamageDone;
        GameEventHandler.OnPause -= OnPause;
        GameEventHandler.OnResume -= OnResume;
    }

    private void ResetPlayer()
    {
        _currentVelocity = Vector2.zero;
        _currentInput = Vector2.zero;
        Grounded = false;

        PlayerClass.CurrentHealth = PlayerClass.GetAttributeValue(AttributeType.HEALTH);

        PlayerClass.ResetPlayerClass();

        LastSave = LevelSerializer.SaveObjectTree(gameObject);
    }

    public void CreateCheckpoint()
    {
        LastSave = LevelSerializer.SaveObjectTree(gameObject);
        checkPointTimer = 0;
    }

    public void ResetToCheckpoint()
    {
        LevelSerializer.LoadObjectTree(LastSave);
        LastSave = LevelSerializer.SaveObjectTree(gameObject);
        backToCheckPointTimer = BackToCheckPointCooldown;
    }

	// Update is called once per frame
	void Update () {
        if (GameManager.Instance.GamePaused)
            return;

        MoneyTimer += Time.deltaTime;
        if(MoneyTimer > 1f)
        {
            MoneyTimer -= 1f;
            Money += MoneyPerSecond;
        }

        if (checkPointTimer < CheckPointCooldown)
            checkPointTimer += Time.deltaTime;
        else if (checkPointTimer > CheckPointCooldown)
            checkPointTimer = CheckPointCooldown;
        else if (InputController.GetClicked(PlayerID() + "_CREATECHECKPOINT"))
        {
            CreateCheckpoint();
        }

        if(backToCheckPointTimer > 0)
            backToCheckPointTimer -= Time.deltaTime;
        else if (InputController.GetClicked(PlayerID() + "_BACKTOCHECKPOINT"))
        {
            ResetToCheckpoint();
        }

	    if (CanMove)
	    {
	        _currentInput.x = Mathf.Abs(InputController.GetValue(PlayerID() + "_RIGHT")) -
	                         Mathf.Abs(InputController.GetValue(PlayerID() + "_LEFT"));

	        _crouching = InputController.GetDown(PlayerID() + "_CROUCH");
	    }
	    else
	    {
	        _currentInput.x = 0;
	    }

        PlayerClass.UpdateClass();

        if (CanUseSkill)
            TryUseSkill();

        UpdateAnimator();
	}

    public void OnResume()
    {
        rigidbody2D.velocity = _savedVelocity;
    }

    public void OnPause()
    {
        _savedVelocity = rigidbody2D.velocity;
        rigidbody2D.velocity = Vector2.zero;
    }

    public string PlayerID()
    {
        return (PlayerId + 1).ToString();
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.GamePaused)
            return;

        _currentVelocity = rigidbody2D.velocity;

        PlayerClass.FixedUpdateClass();

        if (CanMove)
        {
            Moving();
        }

        if(MovedBySkill)
        {
            //Add Velocity from skill in player look direction
            _currentVelocity = PlayerClass.overrideVelocity * transform.localScale.x;
        }

        UpdateLookDirection();

        UpdateAnimator();

        rigidbody2D.velocity = _currentVelocity;
    }

    public void StopPlayer()
    {
        rigidbody2D.velocity = Vector2.zero;
    }

    private void UpdateLookDirection()
    {
        if (_currentInput.x < -0.05f && transform.localScale.x == 1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (_currentInput.x > 0.05f && transform.localScale.x == -1)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (SpotlightTransform)
        {
            SpotlightTransform.localRotation = Quaternion.Euler(new Vector3(SpotlightTransform.localRotation.eulerAngles.x,
            transform.localScale.x * 90, SpotlightTransform.localRotation.eulerAngles.z));
        }
        
    }

    private void UpdateAnimator()
    {
        if (_animator)
        {
            _animator.SetBool("Crouching", _crouching);

            _animator.SetFloat("Speed", Mathf.Abs(_currentVelocity.x));
        }
    }

    public void LevelUp()
    {
        //Do Cool Effekt LOL
        Level++;
        PrevNeededExperience = NeededExperience;
        //NeededExperience *= NeededExperienceMultPerLevel;

        NeededExperience += NeededExperiencePerLevel * (Level * NeededExperienceMultPerLevel);

        //Level UP
        PlayerClass.LevelUp();

        if (CurrentExperience >= NeededExperience)
        {
            LevelUp();
        }
    }

    public void Damage(float damage)
    {
        if (!PlayerClass.damageImune)
        {
            damage = PlayerClass.OnPlayerGetsDamage(damage);
            PlayerClass.CurrentHealth -= damage;
            PlayerClass.OnPlayerDamaged(damage);
        }
        
        if (PlayerClass.CurrentHealth <= 0)
        {
            //Dead
            ResetToCheckpoint();
        }
    }

    public void DamageDone(PlayerController player, float damage)
    {
        if (player == this)
        {
            PlayerClass.OnPlayerDidDamage(damage);
            CurrentExperience += ExperiencePerDamageDone * damage;
            PlayerClass.CurrentHealth += damage * PlayerClass.GetAttributeValue(AttributeType.SPELLVAMP);
        }
        if (CurrentExperience >= NeededExperience)
        {
            LevelUp();
        }
    }

    private void Moving()
    {
        Move();

        Grounded = CheckGround();
        if (CheckUp() && Grounded)
        {
            //Squashed (Dead?)
        }

        TryJump();

        Gravity();
    }


    //new One with AREA
    private bool CheckGround()
    {
        if (_currentVelocity.y > PlayerClass.GetAttributeValue(AttributeType.JUMPPOWER)/2f)
        {
            //If we move upwards not grounded
            //return false;
        }
        bool grounded = Physics2D.OverlapArea(transform.position - transform.right * PlayerClass.playerWidth * Half, transform.position + transform.right * PlayerClass.playerWidth * Half + (-Vector3.up * PlayerClass.footHeight), GroundCheckLayer);
        if (grounded)
        {
            PlayerClass.ResetJump();
        }
        return grounded;
    }

    private bool CheckUp()
    {
        
        if (Physics2D.OverlapArea(transform.position - transform.right * PlayerClass.playerWidth * Half + transform.up * PlayerClass.playerHeight, transform.position + transform.right * PlayerClass.playerWidth * Half + Vector3.up * PlayerClass.playerHeight + Vector3.up * PlayerClass.footHeight, GroundCheckLayer))
        {
            //_currentVelocity.y = 0;
            return true;
        }
        return false;
    }

    private bool TryJump()
    {
        if (_currentVelocity.y < MinUpMotionForExtraJump)
        {
            jumping = false;
        }

        if (Grounded && !jumping && InputController.GetDown(PlayerID() + JumpInput) && PlayerClass.Jump(Grounded))
        {
            Jump();
            jumping = true;
            return true;
        }

        if (jumping && InputController.GetDown(PlayerID() + JumpInput))
        {
            _currentVelocity.y += PlayerClass.GetAttributeValue(AttributeType.MOREJUMPPOWER) * Time.fixedDeltaTime;
            return true;
        }
        return false;
    }

    private bool TryUseSkill()
    {
        //Check Input for skill usement
        if (InputController.GetDown(PlayerID() + Skill1Input) && UseSkill(0))
        {
            return true;
        }
        if (InputController.GetDown(PlayerID() + Skill2Input) && UseSkill(1))
        {
            return true;
        }
        if (InputController.GetDown(PlayerID() + Skill3Input) && UseSkill(2))
        {
            return true;
        }
        if (InputController.GetDown(PlayerID() + Skill4Input) && UseSkill(3))
        {
            return true;
        }
        return false;
    }

    private void Move()
    {
        groundNormal = Vector2.right;
        int inputDirection = (int)Mathf.Clamp(_currentInput.x * 1000, -1, 1);
        int walkDirection = (int)Mathf.Clamp(_currentVelocity.x * 1000, -1, 1);
        float speedChangeMult = 1f;
        float maxSpeedMult = 1f;

        if (Grounded)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.up, Vector3.down, 5f, GroundCheckLayer);

            if (hit)
            {
                groundNormal.x = Mathf.Abs(hit.normal.y);
                groundNormal.y = -Mathf.Abs(hit.normal.x) * inputDirection;
            }

            if (hit.normal.x * inputDirection < 0)
            {
                //When we run uphill slow down maxSpeed, because walking uphill is hard
                maxSpeedMult = Mathf.Clamp(Mathf.Clamp(groundNormal.x - 0.8f, 0f, 1f) * 5f, 0f, 1f);
            }
            else
            {
                //_currentVelocity.x -= _currentVelocity.x * friction;
            }
        }
        else
        {
            Vector3 startPoint = transform.position + transform.right * PlayerClass.playerWidth * Half * inputDirection + 
                                Vector3.up * PlayerClass.playerHeight;
            Vector3 endPoint = transform.position + transform.right * PlayerClass.playerWidth * Half * inputDirection +
                                transform.right * 0.2f * inputDirection;
            
            Debug.DrawLine(startPoint, endPoint);

            if (Physics2D.OverlapArea(startPoint, endPoint, GroundCheckLayer))
            {
                speedChangeMult = 0f;
                if (!CheckUp())
                {
                    //speedChangeMult = 2f;
                    //groundNormal.x = 0;
                    //groundNormal.y = 1 * inputDirection;
                    _currentVelocity.y = 4f;
                }
                else
                {
                    //speedChangeMult = 0f;
                }
            }
                
        }

        Vector2 change = speedChangeMult * groundNormal * _currentInput.x * Time.fixedDeltaTime * 
                         PlayerClass.GetAttributeValue(AttributeType.MOVEMENTCHANGE);

        if (Mathf.Abs((_currentVelocity + change).x) > PlayerClass.GetAttributeValue(AttributeType.MAXMOVESPEED) * maxSpeedMult)
        {
            float clampMagnitude = 1f - Mathf.Clamp(Mathf.Abs((_currentVelocity + change).x) -
                          PlayerClass.GetAttributeValue(AttributeType.MAXMOVESPEED), 0f, 1f);
            change = Vector2.ClampMagnitude(change, clampMagnitude);
        }

        _currentVelocity += change;

        float SlowDownXValue = walkDirection * Time.fixedDeltaTime * (PlayerClass.GetAttributeValue(AttributeType.MOVEMENTCHANGE) / 2f);
        if (Mathf.Abs(_currentVelocity.x) - Mathf.Abs(SlowDownXValue) <= 0)
        {
            _currentVelocity.x = 0;
        }
        else
        {
            _currentVelocity.x -= SlowDownXValue;
        }
    }

    private void Jump()
    {
        _currentVelocity.y = PlayerClass.GetAttributeValue(AttributeType.JUMPPOWER);
    }

    private bool UseSkill(int skillID)
    {
        bool result = PlayerClass.UseSkill(skillID);
        return result;
    }

    private void Gravity()
    {
        _currentVelocity += Physics2D.gravity * PlayerClass.GravityMultiply * Time.fixedDeltaTime;
    }

    public void AddMoney(int amount)
    {
        if (amount < 0) return;
        Money += amount;
    }

    public void AddSkillPreventingSkillUsement()
    {
        _skillPreventsSkillUsement++;
    }

    public void AddSkillPreventingMovement()
    {
        _skillPreventsMovement++;
    }

    public void AddSkillOverridingMovement()
    {
        _skillOverridesMovement++;
    }

    public void AddSkillMakingImune()
    {
        _skillMakesImune++;
    }

    public void RemoveSkillPreventingSkillUsement()
    {
        _skillPreventsSkillUsement--;
    }

    public void RemoveSkillPreventingMovement()
    {
        _skillPreventsMovement--;
    }

    public void RemoveSkillOverridingMovement()
    {
        _skillOverridesMovement--;
    }

    public void RemoveSkillMakingImune()
    {
        _skillMakesImune--;
    }
}
