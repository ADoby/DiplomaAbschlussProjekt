using UnityEngine;

[System.Serializable]
public class Item_Vampire : Item
{
    protected override string[] names
    {
        get
        {
            return new string[] { "Vampiric Knive", "Vampiric Teeth", "Vampiric Gun" };
        }
    }

    public override string Description
    {
        get
        {
            return "<color=" + colors[prefixID] + ">" + Name + "</color>\nVampire: " + Vampire.ToString("#0%");
        }
    }

    public float Vampire = 0.25f;

    public override void UpdateStats(float value)
    {
        Vampire *= value;
    }

    public override void OnPlayerDidDamage(PlayerClass playerClass, float damage)
    {
        playerClass.Heal(damage * Vampire);
    }
}
