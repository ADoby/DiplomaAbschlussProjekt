using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    //Default Attributes
    public string Name = "Name";
    [Range(0f, 100f)]
    public float SkillCooldown = 1.0f;
    protected float skillTimer = 0.0f;

    public float skillRunTime = 0f;
    protected float skillRunTimer = 0.0f;

    public bool makesDamageImune = false;

    public PlayerSkill(string name, float skillCooldown)
    {
        Name = name;
        SkillCooldown = skillCooldown;
    }

    public virtual void UpdateSkill(PlayerClass player)
    {
        skillTimer -= Time.deltaTime * player.GetAttributeValue(AttributeType.ATTACKSPEED);
        skillRunTimer -= Time.deltaTime * player.GetAttributeValue(AttributeType.ATTACKSPEED);
    }

    public virtual void Do(PlayerClass player)
    {
        skillTimer = SkillCooldown;
        skillRunTimer = skillRunTime;
    }

    public virtual void UpdateAttributes(PlayerClass player)
    {

    }

    public bool Running()
    {
        return (skillRunTimer > 0);
    }

    public bool isReady()
    {
        return (skillTimer <= 0);
    }

    public virtual void LateUpdateSkill(PlayerClass playerClass)
    {
        
    }
}
