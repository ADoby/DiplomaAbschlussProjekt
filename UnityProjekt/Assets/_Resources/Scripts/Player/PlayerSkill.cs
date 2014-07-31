using UnityEngine;

[System.Serializable]
public class PlayerSkill : MonoBehaviour
{
    //Default Attributes
    public string Name = "Name";
    [Range(0f, 100f)]
    public float SkillCooldown = 1.0f;
    [SerializeField]
    protected float CooldownTimer = 0.0f;

    public float Cooldown
    {
        get
        {
            return CooldownTimer;
        }
    }

    public float SkillRunTime = 0f;
    [SerializeField]
    protected float SkillRunTimer = 0.0f;
    [SerializeField]
    protected bool skillRunning = false;

    public bool PreventsMovement = false;
    public bool PreventsDamage = false;
    public bool PreventsUsingSkills = false;
    public bool MovesPlayer = false;

    public PlayerClass PlayerClass { protected get; set; }

    public PlayerSkill(string name, float skillCooldown)
    {
        Name = name;
        SkillCooldown = skillCooldown;
    }

    public virtual void UpdateSkill(PlayerClass player)
    {
        CooldownTimer -= Time.deltaTime * player.GetAttributeValue(AttributeType.ATTACKSPEED);
        CooldownTimer = Mathf.Clamp(CooldownTimer, 0f, SkillCooldown);

        if (Running())
        {
            SkillRunTimer -= Time.deltaTime * player.GetAttributeValue(AttributeType.ATTACKSPEED);
            if (SkillRunTimer <= 0)
            {
                SkillFinished(player);
                skillRunning = false;
                PlayerClass.SkillFinished(this);
            }
        }
        
    }

    public virtual void SkillFinished(PlayerClass player)
    {
        
    }

    public virtual void Do(PlayerClass player)
    {
        PlayerClass = player;

        CooldownTimer = SkillCooldown;
        SkillRunTimer = SkillRunTime;

        skillRunning = true;
    }

    public virtual void UpdateAttributesOnLevelUp()
    {

    }

    public bool Running()
    {
        return skillRunning;
    }

    public bool IsReady()
    {
        return (CooldownTimer <= 0);
    }

    public virtual void FixedUpdateSkill(PlayerClass playerClass)
    {
        
    }
}
