using UnityEngine;

//Only an attack without projectil
//Its an sphere or raycast or something
public class BasicAttack : PlayerSkill
{
    public string Projectile;
    public Vector2 ShootingPosition;
    public Vector2 CrouchShootingPosition;

    public float DamageMult = 1.0f;

    public BasicAttack(string name, float skillCooldown)
        : base(name, skillCooldown)
    {

    }

    public override void Do(PlayerClass player)
    {
        base.Do(player);

        //Shoot Or Something
        Vector3 position = new Vector3(ShootingPosition.x * player.playerTransform.localScale.x, ShootingPosition.y, 0);
        if (player.playerControl.crouching)
        {
            position = new Vector3(CrouchShootingPosition.x * player.playerTransform.localScale.x, CrouchShootingPosition.y, 0);
        }
        GameObject go = GameObjectPool.Instance.Spawn(Projectile, player.playerTransform.position + position, player.playerTransform.rotation);
        go.SendMessage("SetDirection", Vector2.right * player.playerTransform.localScale.x);
        go.SendMessage("SetPlayer", player.playerControl);
        go.SendMessage("SetDamage", DamageMult * player.GetAttributeValue(AttributeType.DAMAGE));
    }
}
