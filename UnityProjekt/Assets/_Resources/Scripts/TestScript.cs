using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

    private float timer = 0f;
    public float time = 10f;

    void Awake()
    {
        Debug.Log("Awake");
    }

    // Use this for initialization
	void Start () 
    {
        Debug.Log("Start");
	}
	
	// Update is called once per frame
	void Update () 
    {
        timer = Mathf.Clamp(timer + Time.deltaTime, 0, time);
        if (timer == time)
        {

        }
	}

    void OnGUI()
    {
        GUILayout.Label("Progress: " + (timer / time).ToString("##0%"));
    }

    void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    void OnDisable()
    {
        Debug.Log("OnDisable");
    }

    void OnDestroy()
    {
        Debug.Log("OnDestryo");
    }
}
