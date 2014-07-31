using UnityEngine;

[System.Serializable]
public class Item_GoldPerSecond : Item
{
    protected override string[] names
    {
        get
        {
            return new string[] { "GoldShacle" };
        }
    }

    public override string Description
    {
        get
        {
            return "<color=" + colors[prefixID] + ">" + Name + "</color>\n<color=#226622>Geld per second: " + GoldPerSecond.ToString("#0") + "</color>";
        }
    }

    public int GoldPerSecond = 20;

    public override void UpdateStats(float value)
    {
        base.UpdateStats(value);
        GoldPerSecond = (int)(GoldPerSecond * value);
    }

    public override void Start(PlayerClass playerClass)
    {
        base.Start(playerClass);

        playerClass.playerControl.MoneyPerSecond += GoldPerSecond;
    }
}
