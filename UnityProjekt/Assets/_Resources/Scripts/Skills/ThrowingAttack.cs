using UnityEngine;
using System.Collections;

public class ThrowingAttack : PlayerSkill {

	public string Projectile;
    public Vector2 ShootingPosition;

    public float DamageMult = 1.0f;

    public ThrowingAttack(string name, float skillCooldown)
        : base(name, skillCooldown)
    {

    }

    public override void Do(PlayerClass player)
    {
        base.Do(player);

        //Shoot Or Something
        GameObject go = GameObjectPool.Instance.Spawn(Projectile, player.playerTransform.position + new Vector3(ShootingPosition.x * player.playerTransform.localScale.x, ShootingPosition.y,0), player.playerTransform.rotation);
        go.SendMessage("SetDirection", Vector2.right * player.playerTransform.localScale.x);
        go.SendMessage("SetPlayer", player.playerControl);
        go.SendMessage("SetDamage", DamageMult * player.GetAttributeValue(AttributeType.DAMAGE));
    }
}
