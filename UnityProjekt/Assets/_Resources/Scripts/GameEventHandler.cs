using UnityEngine;
using System.Collections;

public class GameEventHandler : MonoBehaviour {

    public delegate void DamageEvent(PlayerController player, float damage);
    public static event DamageEvent OnDamageDone;

    public delegate void EnemieEvent(Transform sender, Transform target);
    public static event EnemieEvent FoundTarget;

    public delegate void EnemieControllerEvent(EnemieController enemie);
    public static event EnemieControllerEvent EnemieDied;
    public static event EnemieControllerEvent EnemieSpawned;

    public delegate void GameEvent();
    public static event GameEvent OnPause;
    public static event GameEvent OnResume;
    public static event GameEvent Reset;

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

    public static void TriggerEnemieDied(EnemieController enemie)
    {
        if (EnemieDied != null)
        {
            EnemieDied(enemie);
        }
    }

    public static void TriggerEnemieSpawned(EnemieController enemie)
    {
        if (EnemieSpawned != null)
        {
            EnemieSpawned(enemie);
        }
    }

    public static void TriggerStopControllerMenu()
    {
        if (StopControllerMenu != null)
        {
            StopControllerMenu();
        }
    }
}

