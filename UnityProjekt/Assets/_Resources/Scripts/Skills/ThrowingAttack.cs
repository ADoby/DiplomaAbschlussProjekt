using UnityEngine;
using System.Collections;

public class ThrowingAttack : PlayerSkill {

	public string Projectile;
    public Vector2 ShootingPosition;

    public float DamageMult = 1.0f;

    public int amount = 1;

    public float randomPositioning;

    public ThrowingAttack(string name, float skillCooldown)
        : base(name, skillCooldown)
    {

    }

    public override void Do(PlayerClass player)
    {
        base.Do(player);

        for (int i = 0; i < amount; i++)
        {
            //Shoot Or Something
            GameObject go = GameObjectPool.Instance.Spawns(Projectile, player.playerTransform.position + new Vector3(ShootingPosition.x * player.playerTransform.localScale.x, ShootingPosition.y, 0) + Random.insideUnitSphere * randomPositioning, player.playerTransform.rotation);
            go.SendMessage("SetDirection", Vector2.right * player.playerTransform.localScale.x, SendMessageOptions.DontRequireReceiver);
            go.SendMessage("SetPlayer", player.playerControl, SendMessageOptions.DontRequireReceiver);
            go.SendMessage("SetDamage", DamageMult * player.GetAttributeValue(AttributeType.DAMAGE), SendMessageOptions.DontRequireReceiver);
        }
    }
}
