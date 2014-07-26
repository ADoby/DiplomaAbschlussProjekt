using UnityEngine;

[System.Serializable]
public class Item_RelHealthBonus : Item
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
            return "<color=" + colors[prefixID] + ">" + Name + "</color>\nHealth bonus: " + RelativeHealth.ToString("#0%");
        }
    }

    public float RelativeHealth = 0.1f;

    public override void UpdateStats(float value)
    {
        RelativeHealth *= value;
    }

    public override void Start(PlayerClass playerClass)
    {
        base.Start(playerClass);

        playerClass.GetAttribute(AttributeType.HEALTH).AddMult(RelativeHealth);
    }
}
