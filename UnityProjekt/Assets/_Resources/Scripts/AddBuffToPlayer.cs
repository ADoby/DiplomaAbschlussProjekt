using UnityEngine;
using System.Collections;

public class AddBuffToPlayer : PlayerSkill 
{
    public PlayerBuff buff;

    public AddBuffToPlayer(string name, float skillCooldown)
        : base(name, skillCooldown)
    {

    }

    public override void Do(PlayerClass player)
    {
        base.Do(player);
        player.AddBuff(buff);
    }
}
