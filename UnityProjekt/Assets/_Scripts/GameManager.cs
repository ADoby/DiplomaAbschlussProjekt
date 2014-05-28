using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    #region Singleton

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }

    #endregion

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartSingleplayerGame()
    {

    }

    public void StartMultiplayerGame()
    {

    }
}
