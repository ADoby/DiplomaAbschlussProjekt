using UnityEngine;

//Only an attack without projectil
//Its an sphere or raycast or something
[System.Serializable]
public class BasicAttack : PlayerSkill
{
    public string Projectile;
    public Vector2 ShootingPosition;
    public Vector2 CrouchShootingPosition;
    public Vector2 KnockBack;

    public float DamageMult = 1.0f;

    public int pierceAmount = 0;
    public float force = 0f;

    public BasicAttack(string name, float skillCooldown)
        : base(name, skillCooldown)
    {

    }

    public override void Do(PlayerClass player)
    {
        base.Do(player);

        //Shoot Or Something
        Vector3 position = new Vector3(ShootingPosition.x * player.playerTransform.localScale.x, ShootingPosition.y, 0);
        if (player.playerControl.Crouching)
        {
            position = new Vector3(CrouchShootingPosition.x * player.playerTransform.localScale.x, CrouchShootingPosition.y, 0);
        }
        GameObject go = GameObjectPool.Instance.Spawns(Projectile, player.playerTransform.position + position, player.playerTransform.rotation);
        go.SendMessage("SetDirection", Vector2.right * player.playerTransform.localScale.x, SendMessageOptions.DontRequireReceiver);
        go.SendMessage("SetPlayer", player.playerControl, SendMessageOptions.DontRequireReceiver);
        go.SendMessage("SetDamage", DamageMult * player.GetAttributeValue(AttributeType.DAMAGE), SendMessageOptions.DontRequireReceiver);
        go.SendMessage("SetPierceAmount", pierceAmount, SendMessageOptions.DontRequireReceiver);
        go.SendMessage("SetForce", force, SendMessageOptions.DontRequireReceiver);

        player.playerControl.rigidbody2D.MovePosition(player.playerControl.rigidbody2D.position + KnockBack * player.playerTransform.localScale.x * Time.fixedDeltaTime);
    }
}
