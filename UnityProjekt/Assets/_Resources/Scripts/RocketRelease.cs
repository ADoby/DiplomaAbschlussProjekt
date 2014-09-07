using UnityEngine;
using System.Collections;

public class RocketRelease : PlayerSkill
{

    public string Projectile;
    public Vector3 ShootingPosition;

    public float DamageMult = 1.0f;

    public int amount = 1;

    public RocketRelease(string name, float skillCooldown)
        : base(name, skillCooldown)
    {

    }

    public override void Do(PlayerClass player)
    {
        base.Do(player);

        for (int i = 0; i < amount; i++)
        {
            GameObject go = EntitySpawnManager.InstantSpawn(Projectile, player.playerTransform.position + ShootingPosition, Quaternion.FromToRotation(Vector3.up, Vector3.left), countEntity:false);
            go.GetComponent<Rocket>().Impulse(Vector3.up * 5.0f);
            go.GetComponent<Rocket>().player = player.playerControl;
            go.GetComponent<Rocket>().damage = player.GetAttributeValue(AttributeType.DAMAGE) * DamageMult;
        }
    }
}
