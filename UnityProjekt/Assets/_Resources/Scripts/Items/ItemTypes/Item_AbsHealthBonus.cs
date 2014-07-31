using UnityEngine;

[System.Serializable]
public class Item_AbsHealthBonus : Item
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
            return "<color=" + colors[prefixID] + ">" + Name + "</color>\n<color=#226622>Health bonus: " + AbsoluteHealth.ToString("###0") + "</color>";
        }
    }

    public float AbsoluteHealth = 1000f;

    public float AbsoluteHealthPerLevel = 100f;

    public override void UpdateStats(float value)
    {
        base.UpdateStats(value);
        AbsoluteHealth *= value;
    }

    public override void Start(PlayerClass playerClass)
    {
        base.Start(playerClass);
        AbsoluteHealth += AbsoluteHealthPerLevel * Value * playerClass.playerControl.Level;

        playerClass.GetAttribute(AttributeType.HEALTH).AddValue(AbsoluteHealth);
    }

    public override void OnPlayerLevelUp(PlayerClass playerClass)
    {
        base.OnPlayerLevelUp(playerClass);
        playerClass.GetAttribute(AttributeType.HEALTH).AddValue(AbsoluteHealthPerLevel * Value);
    }
}