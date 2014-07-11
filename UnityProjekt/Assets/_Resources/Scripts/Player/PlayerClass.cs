using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum AttributeType
{
    HEALTH,
    HEALTHREG,
    ARMOR,
    ATTACKSPEED,
    DAMAGE,
    MAXMOVESPEED,
    MOVEMENTCHANGE,
    JUMPPOWER,
    MOREJUMPPOWER,
    SPELLVAMP,
    COUNT
}

[System.Serializable]
public class Attribute
{
    public string Name = "Name";
    public float Value = 0f;
    public float ValuePerLevel = 0f;
    public float valueMultiply = 1.0f;

    public float ValuePerSkillPoint = 0f;

    public Attribute(string name, float value, float valuePerLevel, float valuePerSkillPoint)
    {
        Name = name;
        Value = value;
        ValuePerLevel = valuePerLevel;
        ValuePerSkillPoint = valuePerSkillPoint;
    }

    public void AddMult(float amount)
    {
        valueMultiply += amount;
    }

    public void AddValue(float amount)
    {
        Value += amount;
    }

    public void LevelUp()
    {
        Value += ValuePerLevel;
    }

    public void SkillUp()
    {
        Value += ValuePerSkillPoint;
    }
}

public class PlayerClass : MonoBehaviour
{
    public string Name = "KlassenName";
    public string Description = "Beschreibung";

    public Attribute[] Attributes =
    {
        new Attribute("Health", 10f, 5f, 10f),
        new Attribute("Health Regen", 1f, 0.1f, 0.2f),
        new Attribute("Armor", 10f, 2f, 5f),
        new Attribute("Attack Speed", 1f, 0.1f, 0.2f),
        new Attribute("Damage", 10f, 2f, 4f),
        new Attribute("Max Movement Speed", 10f, 0.1f, 0.2f),
        new Attribute("Movement Change", 50f, 2f, 4f),
        new Attribute("Jump Power", 10f, 0.1f, 0.2f),
        new Attribute("More Jump Power", 5f, 0.2f, 0.5f),
        new Attribute("Spell Vampire", 0.0f, 0.0f, 0.1f)
    };

    public PlayerSkill[] playerSkills;

    #region PlayerBuffs

    public List<PlayerBuff> playerBuffs;

    public void AddBuff(PlayerBuff buff)
    {
        PlayerBuff oldBuff = playerBuffs.FirstOrDefault(o => o.BuffName == buff.BuffName);
        PlayerBuff newBuff = null;
        if (oldBuff == null)
        {
            newBuff = buff.Clone();
            playerBuffs.Add(newBuff);
        }
        else
        {
            newBuff = oldBuff;
        }

        if(newBuff != null)
            newBuff.StartBuff(this);
    }

    public void UpdateAllBuffs()
    {
        for (int i = 0; i < playerBuffs.Count; i++)
        {
            playerBuffs[i].UpdateBuff();
        }
    }

    public void EndBuff(PlayerBuff buff)
    {
        PlayerBuff oldBuff = playerBuffs.FirstOrDefault(o => o.BuffName == buff.BuffName);
        if (oldBuff != null)
        {
            oldBuff.EndBuff();
        }
    }

    public void EndAllBuffs()
    {
        for (int i = 0; i < playerBuffs.Count; i++)
        {
            playerBuffs[i].EndBuff();
        }
    }

    #endregion

    

    //Leben
    public float Health = 0.0f;

    private int skillsRunning = 0;
    public bool SkillRunning { get { return (skillsRunning > 0); } }

    [Range(0f, 4f)]
    public float GravityMultiply = 3.0f;

    [Range(0.5f, 2.0f)]
    public float playerWidth = 0.8f;

    [Range(0.5f, 3f)]
    public float playerHeight = 1f;

    [Range(0.01f, 1.0f)]
    public float footHeight = 0.1f;

    [Range(1, 3)]
    public int MaxJumpCount = 1;
    private int currentJumpNumber = 0;

    public Transform playerTransform { get; set; }

    public Vector2 overrideVelocity = Vector2.zero;

    public PlayerController playerControl { get; protected set; }

    public int skillPoints = 0;

    public bool damageImune = false;

    public Texture classThumbnail;

    public void UpdateClass()
    {
        foreach (PlayerSkill skill in playerSkills)
        {
            skill.UpdateSkill(this);
        }

        UpdateAllBuffs();

        Health += GetAttributeValue(AttributeType.HEALTHREG) * Time.deltaTime;
        Health = Mathf.Clamp(Health, 0, GetAttributeValue(AttributeType.HEALTH));
    }

    public void FixedUpdateClass()
    {
        overrideVelocity = Vector2.zero;

        foreach (PlayerSkill skill in playerSkills)
        {
            skill.FixedUpdateSkill(this);
        }
    }

    public void Init(PlayerController playerControl)
    {
        playerTransform = playerControl.transform;
        this.playerControl = playerControl;
    }

    public void ResetPlayerClass()
    {
        Health = GetAttributeValue(AttributeType.HEALTH);
        skillPoints = 0;
    }

    public void SkillFinished(PlayerSkill skill)
    {
        if (skill.PreventsMovement)
            playerControl.RemoveSkillPreventingMovement();
        if (skill.PreventsDamage)
            playerControl.RemoveSkillMakingImune();
        if (skill.MovesPlayer)
            playerControl.RemoveSkillOverridingMovement();
        if (skill.PreventsUsingSkills)
            playerControl.RemoveSkillPreventingSkillUsement();

        skillsRunning--;
    }

    public void LevelUp()
    {
        skillPoints++;
        foreach (Attribute attribute in Attributes)
        {
            attribute.LevelUp();
        }
        foreach (PlayerSkill skill in playerSkills)
        {
            skill.UpdateAttributesOnLevelUp();
        }
    }

    public void SkillUpAttribute(int id)
    {
        if (skillPoints <= 0)
            return;

        skillPoints--;
        GetAttribute(id).SkillUp();
    }

    public float GetAttributeValue(AttributeType type)
    {
        return Attributes[(int)type].Value * Attributes[(int)type].valueMultiply;
    }
    public float GetAttributeValue(int id)
    {
        return Attributes[id].Value * Attributes[id].valueMultiply;
    }

    public Attribute GetAttribute(AttributeType type)
    {
        return Attributes[(int)type];
    }
    public Attribute GetAttribute(int id)
    {
        return Attributes[id];
    }

    public bool Jump(bool grounded)
    {
        //If we fly and did not jump yet something went wrong
        //Probably fall from the edge or something
        //So first jump is "deleted"
        if (currentJumpNumber == 0 && !grounded)
        {
            currentJumpNumber++;
        }

        currentJumpNumber++;

        if (currentJumpNumber > MaxJumpCount)
            return false;

        return true;
    }

    public void ResetJump() 
    {
        currentJumpNumber = 0;
    }

    //Grounded is used to make the player stop controlling and no gravity
    //while the thing runs
    public bool UseSkill(int skillID)
    {
        PlayerSkill skill = playerSkills[skillID];

        if (skill.IsReady())
        {
            skill.Do(this);

            if (skill.SkillRunTime > 0)
            {
                skillsRunning++;
            }

            if (skill.PreventsMovement)
                playerControl.AddSkillPreventingMovement();
            if (skill.PreventsDamage)
                playerControl.AddSkillMakingImune();
            if (skill.MovesPlayer)
                playerControl.AddSkillOverridingMovement();
            if (skill.PreventsUsingSkills)
                playerControl.AddSkillPreventingSkillUsement();

            return true;
        }
        
        return false; ;
    }
}
