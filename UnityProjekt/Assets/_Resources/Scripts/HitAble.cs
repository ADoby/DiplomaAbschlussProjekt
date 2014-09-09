using UnityEngine;
using System.Collections;
using System.Linq;
#if (UNITY_EDITOR)
using UnityEditor;
#endif
[System.Flags]
public enum HitAbleType : int
{
    ALIEN = (1 << 0),
    PLAYER = (1 << 1),
    PROPS = (1 << 2)
}
#if (UNITY_EDITOR)
public class EnumFlagsAttribute : PropertyAttribute
{
    public EnumFlagsAttribute() { }
}

[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        _property.intValue = EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
    }
}

[CustomPropertyDrawer(typeof(HitAbleType))]
public class HitAbleCustomDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        _property.intValue = EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
    }
}
#endif
[System.Serializable]
public class HitAble : MonoBehaviour {

    public static bool CheckForBitInMask(int bit, int mask)
    {
        return ((mask & bit) != 0);
    }
    public static bool CheckForBitInMask(HitAbleType bit, HitAbleType mask)
    {
        return ((mask & bit) != 0);
    }

    public HitAbleType hitAbleType;

    public bool sendFurther = false;
    public HitAble reciever;

    public Collider2D[] usedCollider;

    public Vector3 ColliderBounds
    {
        get
        {
            return MainCollider.bounds.size;
        }
    }

    public Collider2D MainCollider
    {
        get
        {
            if (usedCollider.Length > 0)
            {
                return usedCollider[0];
            }
            return null;
        }
        set
        {
            if (usedCollider.Length == 0)
                usedCollider = new Collider2D[1];
            usedCollider[0] = value;
        }
    }

    public bool ColliderIsOneOfYours(Collider2D targetCollider)
    {
        if (!targetCollider)
            return false;
        for (int i = 0; i < usedCollider.Length; i++)
        {
            if (targetCollider == usedCollider[i])
                return true;
        }
        return false;
    }

    private Transform targetTransform;
    public Transform TargetTransform
    {
        get
        {
            if (!MainCollider)
                return null;
            if (!targetTransform)
                targetTransform = MainCollider.transform;

            return targetTransform;
        }
    }
    public Vector3 ColliderCenter
    {
        get
        {
            return MainCollider.bounds.center;
        }
    } 
    

    //public Vector3

    public bool SpawnPool = true;
    public string SpawnPoolName = "Blood";

    void Start()
    {
        if (!MainCollider)
        {
            if (collider2D)
            {
                MainCollider = collider2D;
            }
        }
    }

    public virtual void Hit(Vector3 HitPosition, Vector3 HitDirection, float forceAmount = 0f)
    {
        if (sendFurther && reciever)
            reciever.Hit(HitPosition, HitDirection, forceAmount);

        if (SpawnPool)
        {
            int layermask = 1 << gameObject.layer;
            RaycastHit2D hit = Physics2D.RaycastAll(HitPosition - HitDirection, HitDirection, HitDirection.magnitude * 2f, layermask).FirstOrDefault(o => o.collider == collider2D);
            if (hit)
            {
                EntitySpawnManager.Spawn(SpawnPoolName, HitPosition, Quaternion.FromToRotation(Vector3.back, hit.normal), countEntity:false, forceDirectSpawn:true);
                if (forceAmount != 0)
                {
                    Force(HitPosition, HitDirection.normalized, forceAmount);
                }
            }
        }

        
    }

    public virtual void Damage(Damage damage)
    {
        if (sendFurther && reciever)
            reciever.Damage(damage);
    }

    public void RecieveForce(Vector3 position, Vector3 direction, float amount)
    {
        Force(position, direction, amount);
    }

    protected virtual void Force(Vector3 position, Vector3 direction, float amount)
    {
        if (sendFurther && reciever)
            reciever.RecieveForce(position, direction, amount);

        if(rigidbody2D)
            rigidbody2D.AddForceAtPosition(direction * amount, position, ForceMode2D.Impulse);
    }
}
