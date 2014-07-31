using UnityEngine;

[System.Serializable]
public class AOEAttributeBoost : AttributeBoost
{
    public AOEAttributeBoost(string name, float skillCooldown)
        : base(name, skillCooldown)
    {

    }

    public float Range = 10f;

}
