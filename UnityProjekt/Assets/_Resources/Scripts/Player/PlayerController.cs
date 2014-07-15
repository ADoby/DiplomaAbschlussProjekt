using System.Text;
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
    public Animator _animator;

    private bool jumping = false;

    public int Level { get; protected set; }
    public float CurrentExperience { get; protected set; }
    public Vector2 _currentVelocity;
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
        //_animator = GetComponent<Animator>();
        //PlayerClass = (PlayerClass) Object.Instantiate(PlayerClass); 
    }

	void Start () 
    {
        Init();
	}

    private void Init()
    {
        PlayerClass.Init(this);

        //GameManager.Instance.AddPlayer(this);

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

        PlayerClass.CurrentHealth = PlayerClass.GetAttributeValue(AttributeType.HEALTH);

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

        if (InputController.GetClicked(PlayerID() + "_SKILLMENU"))
        {
            LevelUpGUI.Instance.OpenWithPlayer(this);
            //AddSkillMakingImune();
            //AddSkillOverridingMovement();
            //AddSkillPreventingMovement();
            //AddSkillPreventingSkillUsement();
        }

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

        PlayerClass.UpdateClass();

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
            PlayerClass.CurrentHealth -= damage;
        }
        
        if (PlayerClass.CurrentHealth <= 0)
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
        if (_currentVelocity.y > 0)
        {
            if (Physics2D.OverlapArea(transform.position - transform.right * PlayerClass.playerWidth * Half + transform.up * PlayerClass.playerHeight, transform.position + transform.right * PlayerClass.playerWidth * Half + Vector3.up * PlayerClass.playerHeight + Vector3.up * PlayerClass.footHeight, GroundCheckLayer))
            {
                //_currentVelocity.y = 0;
                return true;
            }
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

    public Vector2 normal;

    private void Move()
    {
        normal = Vector2.right;
        int inputDirection = (int)Mathf.Clamp(_currentInput.x * 1000, -1, 1);
        int walkDirection = (int)Mathf.Clamp(_currentVelocity.x * 1000, -1, 1);
        float speedChangeMult = 1f;
        float maxSpeedMult = 1f;

        if (Grounded)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.up, Vector3.down, 5f, GroundCheckLayer);

            if (hit)
            {
                normal.x = Mathf.Abs(hit.normal.y);
                normal.y = -Mathf.Abs(hit.normal.x) * inputDirection;
            }

            if (hit.normal.x * inputDirection < 0)
            {
                //When we run uphill slow down maxSpeed, because walking uphill is hard
                maxSpeedMult = Mathf.Clamp(Mathf.Clamp(normal.x - 0.8f, 0f, 1f) * 5f, 0f, 1f);
            }
        }
        else
        {
            Vector3 startPoint = transform.position + transform.right * PlayerClass.playerWidth * Half * inputDirection + 
                                Vector3.up * PlayerClass.playerHeight;
            Vector3 endPoint = transform.position + transform.right * PlayerClass.playerWidth * Half * inputDirection +
                                transform.right * 0.2f * inputDirection;
            
            Debug.DrawLine(startPoint, endPoint);
            
            if(Physics2D.OverlapArea(startPoint, endPoint, GroundCheckLayer))
                speedChangeMult = 0;
        }
        
        Vector2 change = speedChangeMult * normal * _currentInput.x * Time.fixedDeltaTime * 
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
        

        if (inputDirection == 0 || inputDirection + walkDirection == 0)
        {
            //_currentVelocity.x = Mathf.Lerp(_currentVelocity.x, 0,
            //    Time.fixedDeltaTime*PlayerClass.GetAttributeValue(AttributeType.MOVEMENTCHANGE)/10f);
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
