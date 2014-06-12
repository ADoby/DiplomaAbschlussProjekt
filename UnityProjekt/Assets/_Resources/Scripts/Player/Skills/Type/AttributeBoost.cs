using UnityEngine;

public class AttributeBoost : PlayerSkill
{
    public AttributeBoost(string name, float skillCooldown)
        : base(name, skillCooldown)
    {

    }

    public AttributeType Type = AttributeType.HEALTH;
    public float Boost = 0f;
    public float BoostPerLevel = 0f;
}

