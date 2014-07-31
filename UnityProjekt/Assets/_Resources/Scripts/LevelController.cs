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

    public ParallaxLayer[] ParallaxLayer;

    public float NeededLevelDamage = 20000f;

    void OnEnable()
    {
        GameEventHandler.CameraMoved += OnCameraMoved;
    }

    void OnDisable()
    {
        GameEventHandler.CameraMoved -= OnCameraMoved;
    }

    void OnDestroy()
    {
        GameEventHandler.CameraMoved -= OnCameraMoved;
    }

    void Start()
    {
        gameObject.SetActive(false);
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
}
