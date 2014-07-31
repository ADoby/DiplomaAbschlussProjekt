using UnityEngine;

[System.Serializable]
public class PlayerBuff
{
    public string BuffName = "";
    public AttributeType attribute;

    public float BuffAbsAmount = 0f;
    public float BuffAbsPerLevelAmount = 0f;
    public float BuffRelAmount = 0f;
    public float BuffRelPerLevelAmount = 0f;

    public float BuffRunTime = 0f;
    public float runTimer = 0.0f;

    [SerializeField]
    private bool BuffRunning = false;

    [SerializeField]
    private PlayerClass playerClass;
    public PlayerClass PlayerClass 
    {
        protected get
        {
            return playerClass;
        }
        set
        {
            playerClass = value;
        }
    }

    public float AbsAmountAdded = 0f;
    public float RelAmountAdded = 0f;

    public PlayerBuff Clone()
    {
        PlayerBuff buff = new PlayerBuff();
        buff.BuffName = BuffName;
        buff.attribute = attribute;
        buff.BuffAbsAmount = BuffAbsAmount;
        buff.BuffAbsPerLevelAmount = BuffAbsPerLevelAmount;
        buff.BuffRelAmount = BuffRelAmount;
        buff.BuffRelPerLevelAmount = BuffRelPerLevelAmount;
        buff.BuffRunTime = BuffRunTime;
        return buff;
    }

    public virtual void StartBuff(PlayerClass player)
    {
        PlayerClass = player;
        if (!BuffRunning)
        {
            AbsAmountAdded = BuffAbsAmount + BuffAbsPerLevelAmount * player.playerControl.Level;
            RelAmountAdded = BuffRelAmount + BuffRelPerLevelAmount * player.playerControl.Level;

            PlayerClass.GetAttribute(attribute).AddValue(AbsAmountAdded);
            PlayerClass.GetAttribute(attribute).AddMult(RelAmountAdded);
        }

        Renew();
    }

    public virtual void UpdateBuff()
    {
        if (BuffRunning)
        {
            runTimer -= Time.deltaTime;
            if (runTimer <= 0)
            {
                EndBuff();
            }
        }
    }

    public virtual void EndBuff()
    {
        if (BuffRunning)
        {
            PlayerClass.GetAttribute(attribute).AddValue(-AbsAmountAdded);
            PlayerClass.GetAttribute(attribute).AddMult(-RelAmountAdded);
            BuffRunning = false;
        }
    }

    public virtual void Renew()
    {
        runTimer = BuffRunTime;
        BuffRunning = true;
    }
}
