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
            return "<color=" + colors[prefixID] + ">" + Name + "</color>\n<color=#226622>Damage reduction: " + AbsoluteDamageReduction.ToString("##0") + "</color>";
        }
    }

    public float AbsoluteDamageReduction = 100f;

    public float AbsoluteDamageReductionPerLevel = 5f;

    public override void UpdateStats(float value)
    {
        base.UpdateStats(value);
        AbsoluteDamageReduction *= value;
    }

    public override void Start(PlayerClass playerClass)
    {
        base.Start(playerClass);
        AbsoluteDamageReduction += AbsoluteDamageReductionPerLevel * Value * playerClass.playerControl.Level;
    }

    public override void OnPlayerGetsDamage(PlayerClass playerClass, ref Damage damage)
    {
        damage.amount = Mathf.Max(0.0f, damage.amount - AbsoluteDamageReduction);
    }

    public override void OnPlayerLevelUp(PlayerClass playerClass)
    {
        base.OnPlayerLevelUp(playerClass);

        AbsoluteDamageReduction += AbsoluteDamageReductionPerLevel * Value;
    }
}
