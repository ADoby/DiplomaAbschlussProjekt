using UnityEngine;

[System.Serializable]
public class BoostJump : PlayerSkill
{
    public Vector2 direction;

    public BoostJump(string name, float skillCooldown)
        : base(name, skillCooldown)
    {

    }

    public override void SkillFinished(PlayerClass player)
    {
        player.playerControl.StopPlayer();
    }

    public override void FixedUpdateSkill(PlayerClass player)
    {
        base.FixedUpdateSkill(player);

        if(SkillRunTimer > 0f)
            player.overrideVelocity += direction * player.GetAttributeValue(AttributeType.ATTACKSPEED);

    }

    public override void Do(PlayerClass player)
    {
        base.Do(player);

        player.overrideVelocity += direction * player.GetAttributeValue(AttributeType.ATTACKSPEED);
    }
}

