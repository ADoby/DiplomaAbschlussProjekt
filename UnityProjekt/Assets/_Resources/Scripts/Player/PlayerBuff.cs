using UnityEngine;

[System.Serializable]
public class PlayerBuff
{
    public string BuffName = "";
    public AttributeType attribute;

    public float BuffAbsAmount = 0f;
    public float BuffRelAmount = 0f;

    public float BuffRunTime = 0f;
    public float runTimer = 0.0f;

    private bool BuffRunning = false;

    public PlayerClass PlayerClass { protected get; set; }

    public PlayerBuff Clone()
    {
        PlayerBuff buff = new PlayerBuff();
        buff.BuffName = BuffName;
        buff.attribute = attribute;
        buff.BuffAbsAmount = BuffAbsAmount;
        buff.BuffRelAmount = BuffRelAmount;
        buff.BuffRunTime = BuffRunTime;
        return buff;
    }

    public virtual void StartBuff(PlayerClass player)
    {
        PlayerClass = player;
        if (!BuffRunning)
        {
            PlayerClass.GetAttribute(attribute).AddValue(BuffAbsAmount);
            PlayerClass.GetAttribute(attribute).AddMult(BuffRelAmount);
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
            PlayerClass.GetAttribute(attribute).AddValue(-BuffAbsAmount);
            PlayerClass.GetAttribute(attribute).AddMult(-BuffRelAmount);
            BuffRunning = false;
        }
    }

    public virtual void Renew()
    {
        runTimer = BuffRunTime;
        BuffRunning = true;
    }
}
