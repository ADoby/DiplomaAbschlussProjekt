using UnityEngine;
using System.Collections;

public class GameEventHandler : MonoBehaviour {

    public delegate void DamageEvent(PlayerController player, float damage);
    public static event DamageEvent OnDamageDone;

    public delegate void EnemieEvent(Transform sender, Transform target);
    public static event EnemieEvent FoundTarget;

    public delegate void GameEvent();
    public static event GameEvent OnPause;
    public static event GameEvent OnResume;
    public static event GameEvent Reset;
    public static event GameEvent EnemieDied;

    public delegate void CameraMovedEvent(Vector2 direction);
    public static event CameraMovedEvent CameraMoved;


    public static void TriggerCameraMoved(Vector2 direction)
    {
        if (CameraMoved != null)
        {
            CameraMoved(direction);
        }
    }

    public static void TriggerFoundTarget(Transform sender, Transform target)
    {
        if (FoundTarget != null)
        {
            FoundTarget(sender, target);
        }
    }

	public static void TriggerDamageDone(PlayerController player, float damage){
        if (OnDamageDone != null)
        {
            OnDamageDone(player, damage);
        }
    }

    public static void TriggerOnPause()
    {
        if (OnPause != null)
        {
            OnPause();
        }
    }
    public static void TriggerOnResume()
    {
        if (OnResume != null)
        {
            OnResume();
        }
    }

    public static void TriggerReset()
    {
        if (Reset != null)
        {
            Reset();
        }
    }

    public static void TriggerEnemieDied()
    {
        if (EnemieDied != null)
        {
            EnemieDied();
        }
    }
}
