using UnityEngine;
using System.Collections;

public class AddBuffToPlayer : PlayerSkill 
{
    public PlayerBuff[] buffs;

    public SpawnInfos[] Effects;

    public AddBuffToPlayer(string name, float skillCooldown)
        : base(name, skillCooldown)
    {

    }

    public override void Do(PlayerClass player)
    {
        base.Do(player);

        for (int i = 0; i < buffs.Length; i++)
        {
            player.AddBuff(buffs[i]);
        }

        for (int i = 0; i < Effects.Length; i++)
        {
            if (Effects[i].WantsToSpawn)
            {
                for (int a = 0; a < Effects[i].Amount; a++)
                {
                    GameObject go = EntitySpawnManager.InstantSpawn(Effects[i].Next().poolName, player.playerTransform.position + Vector3.up * 1.5f, Quaternion.identity, countEntity:false);
                    go.transform.parent = player.playerTransform;
                }
            }
        }
    }
}
