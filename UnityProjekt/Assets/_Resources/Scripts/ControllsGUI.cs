using UnityEngine;
using System.Collections;

public class ControllsGUI : MonoBehaviour {

    public float time = 2.0f;
    private float timer = 0.0f;

    private string input = "";
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        
	}

    void OnGUI()
    {
        if (timer > 0)
        {
            var e = Event.current;
            if (e.isKey && e.keyCode != KeyCode.None)
            {
                Debug.Log("Detected key code: " + e.keyCode);
                input = System.String.Format("{0}", e.keyCode);
                timer = 0;
            }
        }

        GUILayout.BeginArea(new Rect(100, 100, 200, 400));

        if (GUILayout.Button("Click"))
        {
            timer = time;
        }

        GUILayout.Label("Ding: " + input);

        GUILayout.EndArea();
    }
}
