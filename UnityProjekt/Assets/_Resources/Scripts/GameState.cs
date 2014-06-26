using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {


    public string LevelName;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartGame()
    {

    }
    public void StartGame(string levelName)
    {
        LevelName = levelName;
    }

    IEnumerator LoadLevel()
    {
        AsyncOperation async = Application.LoadLevelAdditiveAsync(LevelName);
        yield return async;
        Debug.Log("Loading complete");

        var controller = Object.FindObjectOfType<GameController>();
    }
}
