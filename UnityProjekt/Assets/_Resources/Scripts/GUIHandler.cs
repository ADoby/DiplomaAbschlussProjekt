using UnityEngine;
using System.Collections;

public class GUIHandler : MonoBehaviour {

    public bool GUIOpened = false;

    public GameObject game;
    public GameObject gui;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GUIOpened = !GUIOpened;
            game.SetActive(!GUIOpened);
            gui.SetActive(GUIOpened);
        }
	}
}
