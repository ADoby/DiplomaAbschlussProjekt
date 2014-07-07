using UnityEngine;
using System.Collections;

public class BaseCollider : MonoBehaviour
{

    public EnemieBase enemieBase;

    public void Damage(float amount)
    {
        enemieBase.Damage(amount);
    }

    public void Hit(Vector3 position)
    {
        enemieBase.Hit(position);
    }
}
