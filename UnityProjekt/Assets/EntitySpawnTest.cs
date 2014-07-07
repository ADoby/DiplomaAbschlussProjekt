using UnityEngine;
using System.Collections;

public class EntitySpawnTest : MonoBehaviour
{

    public Transform spawnPosition;
    public Transform shootPosition;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if (GUILayout.Button("Spawn"))
        {
            EntitySpawnManager.Spawn("Enemie1", spawnPosition.position);
        }
        if (GUILayout.Button("Kill"))
        {
            EntitySpawnManager.Spawn("Grenade", shootPosition.position);
        }
    }
}
