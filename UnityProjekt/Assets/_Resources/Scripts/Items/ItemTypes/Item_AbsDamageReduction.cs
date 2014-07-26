using UnityEngine;

[System.Serializable]
public class Item_AbsDamageReduction : Item
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
            return "<color=" + colors[prefixID] + ">" + Name + "</color>\nDamage reduction: " + AbsoluteDamageReduction.ToString("##0");
        }
    }

    public float AbsoluteDamageReduction = 200f;

    public override void UpdateStats(float value)
    {
        AbsoluteDamageReduction *= value;
    }

    public override float OnPlayerGetsDamage(PlayerClass playerClass, float damage)
    {
        return Mathf.Clamp(damage - AbsoluteDamageReduction, 0, damage);
    }
}
