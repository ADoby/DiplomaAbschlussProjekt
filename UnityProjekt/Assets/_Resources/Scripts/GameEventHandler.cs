using UnityEngine;
using System.Collections;

public class GameEventHandler : MonoBehaviour {

    public delegate void DamageEvent(PlayerController player, Damage damage);
    public static event DamageEvent OnDamageDone;

    public delegate void EnemieEvent(Transform sender, HitAbleInfo target);
    public static event EnemieEvent FoundTarget;

    public delegate void EntityControllerEvent(HitAble entity);
    public static event EntityControllerEvent EntityDied;
    public static event EntityControllerEvent EntitySpawned;

    public delegate void GameEvent();
    public static event GameEvent OnPause;
    public static event GameEvent OnResume;
    public static event GameEvent ResetLevel;
    public static event GameEvent OnCreateCheckpoint;
    public static event GameEvent OnResetToCheckpoint;

    public static event GameEvent StopControllerMenu;

    public delegate void CoopEvent(PlayerController player, string message);
    public static event CoopEvent PlayerLeft;
    public static event CoopEvent PlayerJoined;

    public delegate void CameraMovedEvent(Vector2 direction);
    public static event CameraMovedEvent CameraMoved;

    public static void TriggerPlayerJoined(PlayerController player, string message)
    {
        if (PlayerJoined != null)
        {
            PlayerJoined(player, message);
        }
    }

    public static void TriggerPlayerLeft(PlayerController player, string message)
    {
        if (PlayerLeft != null)
        {
            PlayerLeft(player, message);
        }
    }

    public static void TriggerCameraMoved(Vector2 direction)
    {
        if (CameraMoved != null)
        {
            CameraMoved(direction);
        }
    }

    public static void TriggerFoundTarget(Transform sender, HitAbleInfo target)
    {
        if (FoundTarget != null)
        {
            FoundTarget(sender, target);
        }
    }

	public static void TriggerDamageDone(PlayerController player, Damage damage){
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

    public static void TriggerResetLevel()
    {
        if (ResetLevel != null)
        {
            ResetLevel();
        }
    }

    public static void TriggerEntityDied(HitAble entity)
    {
        if (EntityDied != null)
        {
            EntityDied(entity);
        }
    }

    public static void TriggerEnemieSpawned(HitAble entity)
    {
        if (EntitySpawned != null)
        {
            EntitySpawned(entity);
        }
    }

    public static void TriggerStopControllerMenu()
    {
        if (StopControllerMenu != null)
        {
            StopControllerMenu();
        }
    }

    public static void TriggerCreateCheckpoint()
    {
        if (OnCreateCheckpoint != null)
        {
            OnCreateCheckpoint();
        }
    }

    public static void TriggerResetToCheckpoint()
    {
        if (OnResetToCheckpoint != null)
        {
            OnResetToCheckpoint();
        }
    }
}

