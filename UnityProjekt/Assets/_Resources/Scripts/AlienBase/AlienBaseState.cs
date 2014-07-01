using UnityEngine;

[System.Serializable]
public class AlienBaseState
{
    public bool attacking, spawning;

    public string Name = "BaseState";

    private AlienBase myBase;

    public float startTime = 0.0f, endTime = 0.0f;

    public float healthBonus = 0.0f;
    private float timer = 0.0f;

    public float currentTime
    {
        get { return timer + startTime; }
    }

    public AlienBaseState(AlienBase owner)
    {
        myBase = owner;
    }

    public bool Update()
    {

        
        timer += Time.deltaTime;
        timer = Mathf.Clamp(timer, 0, endTime - startTime);
        if (timer == endTime - startTime)
        {
            return true;
        }
        return false;
    }

}

