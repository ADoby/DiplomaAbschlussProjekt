using UnityEngine;
using System.Collections;

public class SimpleMovement : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
        float inputX = 0;
        inputX = Mathf.Abs(InputController.GetValue("1_RIGHT")) - Mathf.Abs(InputController.GetValue("1_LEFT"));
	    //rigidbody2D.AddForce(Vector2.right*inputX);
        transform.Translate(Vector2.right * inputX);
	}
}
