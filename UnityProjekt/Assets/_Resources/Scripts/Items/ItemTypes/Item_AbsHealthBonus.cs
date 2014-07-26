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
            return "<color=" + colors[prefixID] + ">" + Name + "</color>\nHealth bonus: " + AbsoluteHealth.ToString("###0");
        }
    }

    public float AbsoluteHealth = 1000f;

    public override void UpdateStats(float value)
    {
        AbsoluteHealth *= value;
    }

    public override void Start(PlayerClass playerClass)
    {
        base.Start(playerClass);

        playerClass.GetAttribute(AttributeType.HEALTH).AddValue(AbsoluteHealth);
    }
}