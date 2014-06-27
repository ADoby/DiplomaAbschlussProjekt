using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    #region Singleton

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if(!instance)
                instance = FindObjectOfType<GameManager>();

            return instance;
        }
    }

    void Awake()
    {
        instance = this;

        GameEventHandler.OnPause += OnPause;
        GameEventHandler.OnResume += OnResume;
    }

    #endregion

    public Transform SpawnPosition;

    public List<PlayerController> Players;

    public void AddPlayer(PlayerController player)
    {
        if (!Players.Contains(player))
        {
            Players.Add(player);

            GameEventHandler.TriggerPlayerJoined(player, player.Name);
        }
    }

    public void RemovePlayer(PlayerController player)
    {
        if (Players.Contains(player))
        {
            Players.Remove(player);

            GameEventHandler.TriggerPlayerLeft(player, player.Name);
        }
    }

    public PlayerController MainPlayer
    {
        get
        {
            if (Players.Count > 0)
                return Players[0];

            return null;
        }

        set
        {
            if (Players.Count == 0)
                Players.Add(value);
            else
                Players[0] = value;
        }
    }

    public bool GamePaused = false;

    public static Vector3 GetSpawnPosition()
    {
        return Instance.SpawnPosition.position;
    }

    public void OnPause()
    {
        GamePaused = true;
    }

    public void OnResume()
    {
        GamePaused = false;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
