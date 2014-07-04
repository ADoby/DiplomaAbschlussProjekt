using UnityEngine;

public class PlayerController : MonoBehaviour
{

    #region Public Members
    public int PlayerId = 0;
    public string Name = "Player1";
    public int Money = 0;

    public PlayerClass PlayerClass;

    [Range(0f, 1.0f)]
    public float MinXMovement = 0.1f;

    public float PrevNeededExperience = 0;
    public float NeededExperience = 100f;
    public float NeededExperienceMultPerLevel = 1.5f;
    public float ExperiencePerDamageDone = 2.0f;
    public float MinUpMotionForExtraJump = 0.2f;

    public string JumpInput = "_JUMP",
                    Skill1Input = "_SKILL1",
                    Skill2Input = "_SKILL2",
                    Skill3Input = "_SKILL3",
                    Skill4Input = "_SKILL4";

    public LayerMask GroundCheckLayer;

    public bool Crouching { get { return _crouching; } }

    #endregion

    #region Private Member
    private Animator _animator;

    public int Level { get; protected set; }
    public float CurrentExperience { get; protected set; }
    private Vector2 _currentVelocity;
    private Vector2 _currentInput;
    public bool Grounded;

    private bool _crouching = false;

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

    #endregion

    public Transform SpotlightTransform;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

	void Start () 
    {
        Init();
	}

    private void Init()
    {
        PlayerClass.Init(this);

        GameEventHandler.OnDamageDone += DamageDone;
        GameEventHandler.OnPause += OnPause;
        GameEventHandler.OnResume += OnResume;
        GameEventHandler.Reset += OnReset;

        ResetPlayer();
    }

    private void ResetPlayer()
    {
        _currentVelocity = Vector2.zero;
        _currentInput = Vector2.zero;
        Grounded = false;

        PlayerClass.Health = PlayerClass.GetAttributeValue(AttributeType.HEALTH);

        //Back to SpawnPoint
        transform.position = GameManager.GetSpawnPosition();

        PlayerClass.ResetPlayerClass();
    }

    public void OnReset()
    {
        ResetPlayer();
    }

	// Update is called once per frame
	void Update () {
        if (GameManager.Instance.GamePaused)
            return;

	    if (CanMove)
	    {
	        _currentInput.x = Mathf.Abs(InputController.GetValue(PlayerID() + "_RIGHT")) -
	                         Mathf.Abs(InputController.GetValue(PlayerID() + "_LEFT"));

	        _crouching = InputController.GetDown(PlayerID() + "_CROUCH");

	        if (_animator)
	            _animator.SetBool("Crouching", _crouching);
	    }
	    else
	    {
	        _currentInput.x = 0;
	    }

        PlayerClass.Update();

        if (CanUseSkill)
            TryUseSkill();
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

    void LateUpdate()
    {
        if (GameManager.Instance.GamePaused)
            return;

        _currentVelocity = rigidbody2D.velocity;

        PlayerClass.LateUpdate();

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

    private void UpdateLookDirection()
    {
        if (_currentVelocity.x < 0 && transform.localScale.x == 1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (_currentVelocity.x > 0 && transform.localScale.x == -1)
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
        //if (anim)
        //anim.SetFloat("Speed", Mathf.Abs(currentVelocity.x));

    }

    public void LevelUp()
    {
        //Do Cool Effekt LOL
        Level++;
        PrevNeededExperience = NeededExperience;
        NeededExperience *= NeededExperienceMultPerLevel;

        //Level UP
        PlayerClass.LevelUp();
    }

    public void Damage(float damage)
    {
        if (!PlayerClass.damageImune)
        {
            PlayerClass.Health -= damage;
        }
        
        if (PlayerClass.Health <= 0)
        {
            //Dead
            ResetPlayer();
        }
    }

    public void DamageDone(PlayerController player, float damage)
    {
        if (player == this)
        {
            CurrentExperience += ExperiencePerDamageDone * damage;
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
        if (_currentVelocity.y > MinUpMotionForExtraJump)
        {
            //If we move upwards not grounded
            return false;
        }

        Debug.DrawLine(transform.position - transform.right * PlayerClass.playerWidth * Half, transform.position + transform.right * PlayerClass.playerWidth * Half + (-Vector3.up * PlayerClass.footHeight));

        bool grounded = Physics2D.OverlapArea(transform.position - transform.right * PlayerClass.playerWidth * Half, transform.position + transform.right * PlayerClass.playerWidth * Half + (-Vector3.up * PlayerClass.footHeight), GroundCheckLayer);
        if (grounded)
        {
            if (!Grounded)
            {
                _currentVelocity.y = 0;
            }
            PlayerClass.ResetJump();
        }
        return grounded;
    }

    private bool CheckUp()
    {
        if (_currentVelocity.y > 0)
        {
            if (Physics2D.OverlapArea(transform.position - transform.right * PlayerClass.playerWidth * Half + transform.up * PlayerClass.playerHeight, transform.position + transform.right * PlayerClass.playerWidth * Half + Vector3.up * PlayerClass.playerHeight + Vector3.up * PlayerClass.footHeight, GroundCheckLayer))
            {
                _currentVelocity.y = 0;
                return true;
            }
        }
        return false;
    }

    private bool TryJump()
    {
        if (InputController.GetClicked(PlayerID() + JumpInput) && PlayerClass.Jump(Grounded))
        {
            Jump();
            return true;
        }
        
        if (InputController.GetDown(PlayerID() + JumpInput) && _currentVelocity.y > MinUpMotionForExtraJump)
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
        _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, PlayerClass.GetAttributeValue(AttributeType.MAXMOVESPEED) * _currentInput.x, Time.deltaTime * PlayerClass.GetAttributeValue(AttributeType.MOVEMENTCHANGE));
    }

    private void Jump()
    {
        _currentVelocity.y += PlayerClass.GetAttributeValue(AttributeType.JUMPPOWER);
    }

    private bool UseSkill(int skillID)
    {
        bool result = PlayerClass.UseSkill(skillID);
        return result;
    }

    private void Gravity()
    {
        if (!Grounded)
        {
            _currentVelocity += Physics2D.gravity * PlayerClass.GravityMultiply * Time.fixedDeltaTime;
        }
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
