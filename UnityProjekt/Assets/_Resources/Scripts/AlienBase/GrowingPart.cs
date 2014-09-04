using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class GrowingPart : MonoBehaviour
{
    public float minTime, maxTime;

    public float startTime = -1, endTime = -1;

    public Vector3 startScale = new Vector3(0,0,0);
    public Vector3 endScale = new Vector3(1,1,1);

    public int currentBaseStateIndex = -1;

    public bool manualUserScale = false;

    public string[] BaseStateList;

    public float currentTime = -1;

    public Vector3 wantedScale;

    public int UpdateScaleCounter = 0;

    void Start()
    {
        UpdateTime(startTime, true);
    }

    void Update()
    {
        UpdateScaleCounter++;
        if (UpdateScaleCounter == EnemieBase.UpdateScaleEveryFrames)
        {
            UpdateScaleCounter = 0;
            UpdateScale(EnemieBase.GrowingSpeed * Time.deltaTime * EnemieBase.UpdateScaleEveryFrames);
        }
    }

    public void UpdateMinMaxTime(AlienBase myBase)
    {
        if (currentBaseStateIndex < 0 || currentBaseStateIndex > myBase.StateCount)
            currentBaseStateIndex = myBase.StateCount;

        minTime = myBase.GetMinTime(currentBaseStateIndex);
        maxTime = myBase.GetMaxTime(currentBaseStateIndex);

        if (startTime < minTime || startTime == -1)
            startTime = minTime;

        if (endTime > maxTime || endTime == -1)
            endTime = maxTime;

        if (startTime >= endTime)
        {
            if (startTime - 0.5f >= minTime)
            {
                startTime -= 0.5f;
            }
            else
            {
                startTime = minTime;
            }

            if (startTime >= endTime && endTime != maxTime)
            {
                endTime = startTime + 0.5f;
                if (endTime >= maxTime)
                {
                    endTime = maxTime;
                }
            }
        }
    }

    public void ResetScales()
    {
        startScale = new Vector3(0, 0, 0);
        endScale = new Vector3(1, 1, 1);

        UpdateTime(startTime, true);
    }

    public void UpdateTime(float newTime, bool instantScale = false)
    {
        currentTime = Mathf.Clamp(newTime, startTime, endTime);
        RecalculateScale();

        if (instantScale)
        {
            UpdateScale(1f);
        }
    }

    public void RecalculateScale()
    {
        float prozent = (currentTime - startTime) / (endTime - startTime);
        if (float.IsNaN(prozent))
        {
            prozent = 0f;
        }

        prozent = (float)Math.Round(prozent, 3);

        wantedScale = startScale + (endScale - startScale) * prozent;
    }

    public void UpdateScale(float value)
    {
        transform.localScale = Vector3.Slerp(transform.localScale, wantedScale, value);
    }
}
