using UnityEngine;

[System.Serializable]
public class Item_RelDamageReduction : Item
{
    protected override string[] names
    {
        get
        {
            return new string[] { "Glove", "Helmet", "Shield" };
        }
    }

    public override string Description
    {
        get
        {
            return "<color=" + colors[prefixID] + ">" + Name + "</color>\nDamage reduction: " + RelativeDamageReduction.ToString("#0%");
        }
    }

    public float RelativeDamageReduction = 0.15f;

    public override void UpdateStats(float value)
    {
        RelativeDamageReduction *= value;
    }

    public override float OnPlayerGetsDamage(PlayerClass playerClass, float damage)
    {
        return Mathf.Clamp(damage - damage * RelativeDamageReduction, 0, damage);
    }
}
