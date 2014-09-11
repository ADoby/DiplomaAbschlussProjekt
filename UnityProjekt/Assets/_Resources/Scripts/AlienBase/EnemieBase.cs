using System.Globalization;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemieBase : HitAble
{
    public bool StartFullHealth = false;
    public bool ResetFullHealth = false;

    public LayerMask GroundLayer;

    #region Performance_Values

    [SerializeField]
    public static float GrowingSpeed = 5f;
    [SerializeField]
    public static int UpdateScaleEveryFrames = 3;

    #endregion

    public bool Debug = false;

    public override void Start()
    {
        base.Start();
        healthBar.UpdateBar(true);

        if (StartFullHealth)
        {
            HealFull();
        }

        if (Debug && EntitySpawnManager.ContainsHitAble(this))
            EntitySpawnManager.AddHitAble(this);

        //Correct Positioning based on floor normal
        RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.up, -transform.up, 2f, GroundLayer);
        if (hit)
        {
            transform.localRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
    }

    public override void Reset()
    {
        base.Reset();

        CurrentHealth = 0f;
        if (ResetFullHealth)
        {
            HealFull();
        }
    }

}
