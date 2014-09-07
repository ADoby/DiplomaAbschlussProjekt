using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#region Enums
public enum AttributeType
{
    HEALTH,
    HEALTHREG,
    ATTACKSPEED,
    DAMAGE,
    MAXMOVESPEED,
    JUMPPOWER,
    MOREJUMPPOWER,
    SPELLVAMP,
    COUNT
}

public enum DamageType
{
    MEELE,
    RANGED,
    EXPLOSION
}

#endregion

[System.Serializable]
public struct Damage
{
    public DamageType type;
    public float amount;
    public Transform other;

    public bool DamageFromAPlayer;
    public PlayerController player;
}

[System.Serializable]
public class DamageResistence
{
    public DamageType damageType;
    public float Procentage = 1.0f;
}

#region Attributes


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

#endregion

[System.Serializable]
public class PlayerClass : MonoBehaviour
{
    public string Name = "KlassenName";

    [Multiline(4)]
    public string Description = "Beschreibung";
    [SerializeField]
    public Attribute[] Attributes =
    {
        new Attribute() { Name = "Health",              Value = 100f, ValuePerLevel = 25f,  ValuePerSkillPoint = 100f,   MaxValue = 999999f, MaxSkillUps = 200 },
        new Attribute() { Name = "Health Regeneration", Value = 5f,   ValuePerLevel = 0.5f, ValuePerSkillPoint = 1f,     MaxValue = 2000f,   MaxSkillUps = 50 },
        new Attribute() { Name = "Attack Speed",        Value = 1f,   ValuePerLevel = 0f,   ValuePerSkillPoint = 0.05f,  MaxValue = 4f,      MaxSkillUps = 20 },
        new Attribute() { Name = "Damage",              Value = 10f,  ValuePerLevel = 2f,   ValuePerSkillPoint = 5f,     MaxValue = 5000f,   MaxSkillUps = 100 },
        new Attribute() { Name = "Movement Speed",      Value = 5f,   ValuePerLevel = 0f,   ValuePerSkillPoint = 0.5f,   MaxValue = 30f,     MaxSkillUps = 20 },
        new Attribute() { Name = "Jump Power",          Value = 5f,   ValuePerLevel = 0f,   ValuePerSkillPoint = 0.4f,   MaxValue = 25f,     MaxSkillUps = 30 },
        new Attribute() { Name = "More Jump Power",     Value = 5f,   ValuePerLevel = 0f,   ValuePerSkillPoint = 0.2f,   MaxValue = 15f,     MaxSkillUps = 20 },
        new Attribute() { Name = "Vampire",             Value = 0f,   ValuePerLevel = 0f,   ValuePerSkillPoint = 0.005f, MaxValue = 0.25f,   MaxSkillUps = 20 }
    };
    [SerializeField]
    public PlayerSkill[] playerSkills;

    #region PlayerBuffs
    [SerializeField]
    public List<PlayerBuff> playerBuffs;

    [SerializeField]
    public DamageResistence[] resitences =
    {
        new DamageResistence() { damageType = DamageType.MEELE, Procentage = 1.0f },
        new DamageResistence() { damageType = DamageType.RANGED, Procentage = 1.0f },
        new DamageResistence() { damageType = DamageType.EXPLOSION, Procentage = 1.0f }
    };

    [ContextMenu("Reset Resistences")]
    public void ResetResistence()
    {
        resitences = new DamageResistence[]
    {
        new DamageResistence() { damageType = DamageType.MEELE, Procentage = 1.0f },
        new DamageResistence() { damageType = DamageType.RANGED, Procentage = 1.0f },
        new DamageResistence() { damageType = DamageType.EXPLOSION, Procentage = 1.0f }
    };
    }

    [ContextMenu("Reset Attributes")]
    public void ResetAttributes()
    {
        Attributes = new Attribute[]
    {
        new Attribute() { Name = "Health",              Value = 100f, ValuePerLevel = 25f,  ValuePerSkillPoint = 100f,   MaxValue = 999999f, MaxSkillUps = 200 },
        new Attribute() { Name = "Health Regeneration", Value = 5f,   ValuePerLevel = 0.5f, ValuePerSkillPoint = 1f,     MaxValue = 2000f,   MaxSkillUps = 50 },
        new Attribute() { Name = "Attack Speed",        Value = 1f,   ValuePerLevel = 0f,   ValuePerSkillPoint = 0.05f,  MaxValue = 4f,      MaxSkillUps = 20 },
        new Attribute() { Name = "Damage",              Value = 10f,  ValuePerLevel = 2f,   ValuePerSkillPoint = 5f,     MaxValue = 5000f,   MaxSkillUps = 100 },
        new Attribute() { Name = "Movement Speed",      Value = 8f,   ValuePerLevel = 0f,   ValuePerSkillPoint = 0.5f,   MaxValue = 30f,     MaxSkillUps = 20 },
        new Attribute() { Name = "Jump Power",          Value = 10f,   ValuePerLevel = 0f,   ValuePerSkillPoint = 0.4f,   MaxValue = 25f,     MaxSkillUps = 30 },
        new Attribute() { Name = "More Jump Power",     Value = 5f,   ValuePerLevel = 0f,   ValuePerSkillPoint = 0.2f,   MaxValue = 15f,     MaxSkillUps = 20 },
        new Attribute() { Name = "Vampire",             Value = 0f,   ValuePerLevel = 0f,   ValuePerSkillPoint = 0.005f, MaxValue = 0.25f,   MaxSkillUps = 20 }
    };
    }

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

    [SerializeField]
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
    [SerializeField]
    private int currentJumpNumber = 0;

    public Transform playerTransform;

    public Vector2 overrideVelocity = Vector2.zero;

    public PlayerController playerControl { get; protected set; }

    public int skillPoints = 0;

    public bool damageImune = false;

    public Texture classThumbnail;

    public string UIItemPickupPoolName = "UIItemPickup";

    public void UpdateClass()
    {
        foreach (PlayerSkill skill in playerSkills)
        {
            skill.UpdateSkill(this);
        }

        UpdateAllBuffs();

        CurrentHealth += GetAttributeValue(AttributeType.HEALTHREG) * Time.deltaTime;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, GetAttributeValue(AttributeType.HEALTH));

        for (int i = 0; i < items.Count; i++)
        {
            items[i].Update(this);
        }
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

        for (int i = 0; i < playerSkills.Length; i++)
        {
            playerSkills[i] = (PlayerSkill)PlayerSkill.Instantiate(playerSkills[i]);
            playerSkills[i].transform.parent = transform;
        }
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
        OnPlayerLevelUp();
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
    private float GetAttributeValue(int id)
    {
        return Attributes[id].AbsoluteValue;
    }

    public Attribute GetAttribute(AttributeType type)
    {
        if ((int)type >= (int)AttributeType.COUNT)
        {
            Debug.Log("Someone tries to get ID: " + (int)type);
            return null;
        }
        return Attributes[(int)type];
    }
    private Attribute GetAttribute(int id)
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

            skillsRunning++;

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

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, GetAttributeValue(AttributeType.HEALTH));
    }

    [SerializeField]
    public List<Item> items = new List<Item>();

    public void AddItem(Item item)
    {
        items.Add(item);
        item.Start(this);

        GameObject go = EntitySpawnManager.InstantSpawn(UIItemPickupPoolName, transform.position, Quaternion.identity, countEntity:false);
        go.GetComponent<UIItemPickup>().text = item.Description;
    }

    public void RemoveItem(Item item)
    {
        //not Used
        
    }


    public virtual void OnPlayerGetsDamage(ref Damage damage)
    {
        for (int i = 0; i < resitences.Length; i++)
        {
            if (resitences[i].damageType == damage.type)
            {
                damage.amount *= resitences[i].Procentage;
                break;
            }
        }
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnPlayerGetsDamage(this, ref damage);
        }
    }
    public virtual void OnPlayerDamaged(Damage damage)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnPlayerDamaged(this, damage);
        }
    }
    public virtual void OnPlayerDoesDamage(ref Damage damage)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnPlayerDoesDamage(this, ref damage);
        }
    }
    public virtual void OnPlayerDidDamage(Damage damage)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnPlayerDidDamage(this, damage);
        }
    }
    public virtual void OnPlayerKilledEntity()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnPlayerKilledEntity(this);
        }
    }

    public virtual void OnPlayerLethalDamage(ref Damage damage)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnPlayerLethalDamage(this, ref damage);
        }
    }
    public virtual void OnPlayerDied()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnPlayerDied(this);
        }
    }
    public void OnPlayerLevelUp()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnPlayerLevelUp(this);
        }
    }
}
