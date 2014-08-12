using UnityEngine;
using System.Collections;

[System.Serializable]
public class Game : MonoBehaviour {

    public int CurrentSpawnedEntityCount = 0;
    public int MaxSpawnedEntityCount = 40;

    public int DefaultLevel = 0;
    public int currentLevel = 0;

    public bool CanSpawnEntity
    {
        get { return (CurrentSpawnedEntityCount < MaxSpawnedEntityCount); }
    }

    public float CurrentDifficulty = 0;
    public float DifficultyValue = 0;
    public float DifficultEveryXSecond = 5f;

    public float CurrentLevelDamage = 0f;
}
