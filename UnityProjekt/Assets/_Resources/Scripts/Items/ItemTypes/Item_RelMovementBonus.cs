using UnityEngine;

[System.Serializable]
public class Item_RelMovementBonus : Item
{
    protected override string[] names
    {
        get
        {
            return new string[] { "Boots", "Booster", "Swifter" };
        }
    }

    public override string Description
    {
        get
        {
            return "<color=" + colors[prefixID] + ">" + Name + "</color>\n<color=#226622>Movement mult: " + RelMovementBonus.ToString("##0%") + "</color>";
        }
    }

    public float RelMovementBonus = 0.5f;

    public override void UpdateStats(float value)
    {
        base.UpdateStats(value);
        RelMovementBonus *= value;
    }

    public override void Start(PlayerClass playerClass)
    {
        base.Start(playerClass);

        playerClass.GetAttribute(AttributeType.MAXMOVESPEED).AddMult(RelMovementBonus);
        playerClass.GetAttribute(AttributeType.MOVEMENTCHANGE).AddMult(RelMovementBonus);
    }
}
