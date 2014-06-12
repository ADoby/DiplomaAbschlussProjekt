using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

    public static Game Instance;

    public Transform firstPlayer;
    public static bool Paused;

    public static Transform mainPlayer
    {
        get
        {
            return Instance.firstPlayer;
        }
    }

    void Awake()
    {
        Game.Instance = this;
    }

	// Use this for initialization
	void Start () {
        GameEventHandler.OnPause += OnPause;
        GameEventHandler.OnResume += OnResume;
	}

    public void OnPause()
    {
        Paused = true;
    }

    public void OnResume()
    {
        Paused = false;
    }
}
