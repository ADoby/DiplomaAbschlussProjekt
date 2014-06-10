using UnityEngine;
using System.Collections;

public class GameEventHandler : MonoBehaviour {

    public delegate void GameEvent();
    public static event GameEvent OnPause;
    public static event GameEvent OnResume;
    public static event GameEvent Reset;

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
}
