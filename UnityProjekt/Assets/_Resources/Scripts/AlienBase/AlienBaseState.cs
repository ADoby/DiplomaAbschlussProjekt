using UnityEngine;

[System.Serializable]
public class AlienBaseState
{
    public bool attacking, spawning;

    public string Name = "BaseState";

    public float startTime = 0.0f, endTime = 0.0f;

    public float healthBonus = 0.0f;
    private float timer = 0.0f;

    private AlienBase myBase;

    public float currentTime
    {
        get { return timer + startTime; }
        set
        {
            if (value <= endTime && value >= startTime)
            {
                timer = value;
            }
        }
    }

    public AlienBaseState(AlienBase value)
    {
        myBase = value;
    }

    public void Reset()
    {
        timer = 0;

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

