using UnityEngine;
using System.Collections;
using System;

public enum PlayerState
{
    STANDING,
    MOVING,
    USINGSKILL,
    COUNT
}

public class PlayerController : MonoBehaviour {

    public int playerID = 0;

    //Something
    private float _half = 0.5f;

    public Animator anim;

    public string Name = "Player1";

    [SerializeField]
    public PlayerClass playerClass;

    [Range(0f, 1.0f)]
    public float minMovement = 0.1f;

    public int level;
    public float currentExperience = 0f;
    public float prevNeededExperience = 0;
    public float neededExperience = 100f;
    public float neededExperienceMultPerLevel = 1.5f;
    public float ExperiencePerDamageDone = 2.0f;

    public string   JumpInput   = "_JUMP", 
                    Skill1Input = "_SKILL1", 
                    Skill2Input = "_SKILL2", 
                    Skill3Input = "_SKILL3", 
                    Skill4Input = "_SKILL4";

    public LayerMask GroundCheckLayer;

    //Private:
    public PlayerState currentState;
    public Vector2 currentVelocity;
    public Vector2 currentInput;
    public bool grounded;

    public bool crouching = false;

    public Vector2 lastVelocity = Vector2.zero;

	// On Start we ask our selfs if we are the owner
    // of this object, if not, deactivate the script
	void Start () {

        if (this.enabled)
        {
            Init();
        }
	}

    private void Init()
    {
        playerClass.Init(transform);

        currentState = PlayerState.STANDING;
        currentVelocity = Vector2.zero;
        currentInput = Vector2.zero;
        grounded = false;

        GameEventHandler.OnDamageDone += DamageDone;
        GameEventHandler.OnPause += OnPause;
        GameEventHandler.OnResume += OnResume;
        GameEventHandler.Reset += OnReset;

        playerClass.playerControl = this;

        StartPos = transform.position;
    }

    private Vector3 StartPos = Vector3.zero;

    public void OnReset()
    {
        playerClass.Health = playerClass.GetAttributeValue(AttributeType.HEALTH);
        transform.position = StartPos;

    }

	// Update is called once per frame
	void Update () {
        if (Game.Paused)
            return;

        currentInput.x = Mathf.Abs(InputController.GetValue(PlayerID() + "_RIGHT")) - Mathf.Abs(InputController.GetValue(PlayerID() + "_LEFT"));

        if (InputController.GetDown(PlayerID() + "_CROUCH"))
        {
            crouching = true;
        }
        else
        {
            crouching = false;
        }

        if (anim)
        {
            anim.SetBool("Crouching", crouching);
        }

        playerClass.Update();

        TryUseSkill();
        
        switch (currentState)
        {
            case PlayerState.STANDING:
                Standing();
                break;
            case PlayerState.MOVING:
                Moving();
                break;
            case PlayerState.USINGSKILL:
                UsingSkill();
                break;
            case PlayerState.COUNT:
            //Should not happen
            default:
                //Should not happen
                throw new ArgumentOutOfRangeException();
        }

        //We did change the velocity probably
        rigidbody2D.velocity = currentVelocity;
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

    public string PlayerID()
    {
        return (playerID + 1).ToString();
    }

    

    void LateUpdate()
    {
        if (Game.Paused)
            return;

        currentVelocity = rigidbody2D.velocity;

        playerClass.LateUpdate();

        //if (anim)
            //anim.SetFloat("Speed", Mathf.Abs(currentVelocity.x));

        if (playerClass.overrideVelocity != Vector2.zero)
            currentVelocity = playerClass.overrideVelocity * transform.localScale.x; //localScale means look direction

        if (currentVelocity.x < 0 && transform.localScale.x == 1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (currentVelocity.x > 0 && transform.localScale.x == -1)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void LevelUp()
    {

        //LevelUpGUI.OpenPlayer(this);

        //Do Cool Effekt LOL
        level++;
        prevNeededExperience = neededExperience;
        neededExperience *= neededExperienceMultPerLevel;

        //Level UP
        playerClass.LevelUp();
        playerClass.UpdateAttributes();
    }

    public void Damage(float damage)
    {
        if (!playerClass.damageImune)
        {
            playerClass.Health -= damage;
        }
        
        if (playerClass.Health <= 0)
        {
            //Dead
        }
    }

    public void DamageDone(PlayerController player, float damage)
    {
        if (player == this)
        {
            currentExperience += ExperiencePerDamageDone * damage;
        }
        if (currentExperience >= neededExperience)
        {
            LevelUp();
        }
    }

    private void Standing()
    {
        Move();

        grounded = CheckGround();
        CheckUp();

        TryJump();

        if (currentVelocity.magnitude > 0.1f)
        {
            currentState = PlayerState.MOVING;
        }

        Gravity();
    }

    //We change the currentVelocity based on currentInput;
    private void Moving()
    {
        Move();

        grounded = CheckGround();
        CheckUp();

        TryJump();
        
        if (currentVelocity.magnitude < 0.1f)
        {
            currentState = PlayerState.STANDING;
        }

        Gravity();
    }

    private void UsingSkill()
    {
        if (!playerClass.SkillRunning)
        {
            currentState = PlayerState.MOVING;
            playerClass.damageImune = false;
        }
    }

    //new One with AREA
    private bool CheckGround()
    {
        if (currentVelocity.y > minUpMotionForExtraJump)
        {
            //If we move upwards not grounded
            return false;
        }
        bool grounded = Physics2D.OverlapArea(transform.position - transform.right * playerClass.playerWidth * _half, transform.position + transform.right * playerClass.playerWidth * _half + (-Vector3.up * playerClass.footHeight), GroundCheckLayer.value);
        if (grounded)
        {
            if (!this.grounded)
            {
                currentVelocity.y = 0;
            }
            playerClass.ResetJump();
        }
        return grounded;
    }

    private void CheckUp()
    {
        if (currentVelocity.y > minUpMotionForExtraJump)
        {
            if (Physics2D.OverlapArea(transform.position - transform.right * playerClass.playerWidth * _half + transform.up * playerClass.playerHeight, transform.position + transform.right * playerClass.playerWidth * _half + Vector3.up * playerClass.playerHeight + (Vector3.up * playerClass.footHeight), GroundCheckLayer.value))
            {
                currentVelocity.y = 0;
            }
        }
    }

    public float minUpMotionForExtraJump = 0.2f;

    private bool TryJump()
    {
        if (InputController.GetClicked(PlayerID() + JumpInput) && playerClass.Jump(grounded))
        {
            Jump();
            return true;
        }
        else if (InputController.GetDown(PlayerID() + JumpInput) && currentVelocity.y > minUpMotionForExtraJump)
        {
            currentVelocity.y += playerClass.GetAttributeValue(AttributeType.MOREJUMPPOWER) * Time.deltaTime;
        }
        return false;
    }

    private bool TryUseSkill()
    {
        if (playerClass.SkillRunning)
            return false;

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
        currentVelocity.x = Mathf.Lerp(currentVelocity.x, playerClass.GetAttributeValue(AttributeType.MAXMOVESPEED) * currentInput.x, Time.deltaTime * playerClass.GetAttributeValue(AttributeType.MOVEMENTCHANGE));
    }

    private void Jump()
    {
        currentVelocity.y += playerClass.GetAttributeValue(AttributeType.JUMPPOWER);
    }

    private bool UseSkill(int skillID)
    {
        bool result = playerClass.UseSkill(skillID, ref grounded);
        if (grounded)
        {
            currentState = PlayerState.USINGSKILL;
        }
        return result;
    }

    private void Gravity()
    {
        if (!grounded)
        {
            currentVelocity += Physics2D.gravity * playerClass.GravityMultiply * Time.deltaTime;
        }
        else
        {
            //Damit wir nicht über dem Boden fliegen
            //currentVelocity.y -= Time.deltaTime;
        }
    }

    public int Money = 0;

    public void AddMoney(int amount)
    {
        Money += amount;
    }
}
