using UnityEngine;
using System.Collections;

[System.Serializable]
public class ParallaxLayer
{
	public static float Multiplier = 0.001f;

	public void Init()
	{
		StartPosition = transform.position;
		Difference = Vector3.zero;
	}

	public void UpdatePosition()
	{
		transform.position = Vector3.Lerp(transform.position, StartPosition + Difference * Multiplier * Movement, Time.deltaTime * Speed);
	}

	public bool enabled = true;
	public Transform transform;
	private Vector3 StartPosition;
	public Vector3 Difference;
	public float Movement = 1.0f;
	public float Speed = 2.0f;
}

public class Parallax : MonoBehaviour {


	public ParallaxLayer[] layer;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < layer.Length; i++)
		{
			if (!layer[i].transform)
			{
				layer[i].enabled = false;
				continue;
			}
				
			layer[i].Init();
		}
	}

	// Update is called once per frame
	void Update () 
	{
		Vector3 difference = Input.mousePosition - new Vector3(Screen.width, Screen.height, 0.0f) * 0.5f;

		for (int i = 0; i < layer.Length; i++)
		{
			layer[i].Difference = difference;
			layer[i].UpdatePosition();
		}
	}
}
