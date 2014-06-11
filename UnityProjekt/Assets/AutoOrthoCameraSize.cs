using UnityEngine;
using System.Collections;

public class AutoOrthoCameraSize : MonoBehaviour
{

    public float GridSize = 32;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    Camera.main.orthographicSize = (Screen.height/GridSize)/2f;
	}
}
