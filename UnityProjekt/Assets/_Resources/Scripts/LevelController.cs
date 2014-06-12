using UnityEngine;
using System.Collections;

[System.Serializable]
public class ParallaxLayer
{
    public Transform transform;
    public Vector2 movement;
}

public class LevelController : MonoBehaviour {

    public Transform LevelSpawnPoint;
    public Transform SpawnFloorsParent;

    public ParallaxLayer[] ParallaxLayer;

    void Awake()
    {
        GameEventHandler.CameraMoved += OnCameraMoved;
    }

    private void OnCameraMoved(Vector2 direction)
    {
        foreach (var item in ParallaxLayer)
        {
            direction.x *= item.movement.x;
            direction.y *= item.movement.y;
            item.transform.Translate(direction);
        }
    }

    public void Reset()
    {

    }

	void Start () {
	
	}
	
	void Update () {
	
	}
}
