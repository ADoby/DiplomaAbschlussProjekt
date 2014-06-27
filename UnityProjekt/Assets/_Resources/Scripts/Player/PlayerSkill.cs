using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    //Default Attributes
    public string Name = "Name";
    [Range(0f, 100f)]
    public float SkillCooldown = 1.0f;
    protected float CooldownTimer = 0.0f;

    public float SkillRunTime = 0f;
    protected float SkillRunTimer = 0.0f;
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

        if (Running())
        {
            SkillRunTimer -= Time.deltaTime * player.GetAttributeValue(AttributeType.ATTACKSPEED);
            if (SkillRunTimer <= 0)
            {
                skillRunning = false;
                PlayerClass.SkillFinished(this);
            }
        }
        
    }

    public virtual void Do(PlayerClass player)
    {
        PlayerClass = player;

        CooldownTimer = SkillCooldown;
        SkillRunTimer = SkillRunTime;

        skillRunning = true;
    }

    public virtual void UpdateAttributes(PlayerClass player)
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

    public virtual void LateUpdateSkill(PlayerClass playerClass)
    {
        
    }
}
