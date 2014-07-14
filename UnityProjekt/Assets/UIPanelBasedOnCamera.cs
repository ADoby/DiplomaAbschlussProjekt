using UnityEngine;
using System.Collections;

public class UIPanelBasedOnCamera : MonoBehaviour {

    public Camera cam;
    public UIPanel panel;


	// Use this for initialization
	void Start () {
	    panel.RelativePosition = new Vector2(cam.rect.x, cam.rect.y);
        panel.RelativeSize = new Vector2(cam.rect.width, cam.rect.height);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
