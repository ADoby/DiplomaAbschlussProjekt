using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum AttributeType
{
    HEALTH,
    HEALTHREG,
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

    public float MinValue = 0f;
    public float MaxValue = 0f;

    public int MaxSkillUps = 100;
    public int currentSkillUps = 0;

    public bool CanSkillUp
    {
        get
        {
            return (currentSkillUps < MaxSkillUps);
        }
    }

    public float AbsoluteValue
    {
        get
        {
            return Mathf.Clamp(Value * valueMultiply, MinValue, MaxValue);
        }
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

    public bool SkillUp()
    {
        if (currentSkillUps < MaxSkillUps)
        {
            Value += ValuePerSkillPoint;
            currentSkillUps++;
            return true;
        }
        return false;
    }
}

public class PlayerClass : MonoBehaviour
{
    public string Name = "KlassenName";
    public string Description = "Beschreibung";

    public Attribute[] Attributes =
    {
        new Attribute() { Name = "Health",              Value = 100f, ValuePerLevel = 25f,  ValuePerSkillPoint = 100f,   MaxValue = 999999f, MaxSkillUps = 200 },
        new Attribute() { Name = "Health Regeneration", Value = 5f,   ValuePerLevel = 0.5f, ValuePerSkillPoint = 1f,     MaxValue = 2000f,   MaxSkillUps = 50 },
        new Attribute() { Name = "Attack Speed",        Value = 1f,   ValuePerLevel = 0f,   ValuePerSkillPoint = 0.05f,  MaxValue = 4f,      MaxSkillUps = 20 },
        new Attribute() { Name = "Damage",              Value = 10f,  ValuePerLevel = 2f,   ValuePerSkillPoint = 5f,     MaxValue = 5000f,   MaxSkillUps = 100 },
        new Attribute() { Name = "Movement Speed",      Value = 5f,   ValuePerLevel = 0f,   ValuePerSkillPoint = 0.5f,   MaxValue = 30f,     MaxSkillUps = 20 },
        new Attribute() { Name = "Acceleration",        Value = 30f,  ValuePerLevel = 0f,   ValuePerSkillPoint = 3f,     MaxValue = 200f,    MaxSkillUps = 40 },
        new Attribute() { Name = "Jump Power",          Value = 5f,   ValuePerLevel = 0f,   ValuePerSkillPoint = 0.4f,   MaxValue = 25f,     MaxSkillUps = 30 },
        new Attribute() { Name = "More Jump Power",     Value = 5f,   ValuePerLevel = 0f,   ValuePerSkillPoint = 0.2f,   MaxValue = 15f,     MaxSkillUps = 20 },
        new Attribute() { Name = "Vampire",             Value = 0f,   ValuePerLevel = 0f,   ValuePerSkillPoint = 0.005f, MaxValue = 0.25f,   MaxSkillUps = 20 }
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
    public float CurrentHealth = 0.0f;

    private int skillsRunning = 0;
    public bool SkillRunning { get { return (skillsRunning > 0); } }

    [Range(0f, 4f)]
    public float GravityMultiply = 2.0f;

    [Range(0.5f, 2.0f)]
    public float playerWidth = 1f;

    [Range(0.5f, 3f)]
    public float playerHeight = 2f;

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

        CurrentHealth += GetAttributeValue(AttributeType.HEALTHREG) * Time.deltaTime;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, GetAttributeValue(AttributeType.HEALTH));
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
        CurrentHealth = GetAttributeValue(AttributeType.HEALTH);
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

        if (GetAttribute(id).SkillUp())
        {
            skillPoints--;
        }
    }

    public float GetAttributeValue(AttributeType type)
    {
        return GetAttributeValue((int)type);
    }
    public float GetAttributeValue(int id)
    {
        return Attributes[id].AbsoluteValue;
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
