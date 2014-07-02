using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class GrowingPart : MonoBehaviour
{
    public AlienBase myBase;

    public float minTime, maxTime;

    public float startTime = -1, endTime = -1;

    public Vector3 startScale = new Vector3(0,0,0);
    public Vector3 endScale = new Vector3(1,1,1);

    public float currentTime = 0;

    public int currentBaseStateIndex = -1;

    public bool manualUserScale = false;

    public void UpdateMinMaxTime()
    {
        if (!myBase)
            return;

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

        UpdateScale();
    }

    public void UpdateScale()
    {
        if (!myBase)
            return;

        currentTime = myBase.currentTime;

        currentTime = Mathf.Clamp(currentTime, startTime, endTime);

        float prozent = (currentTime - startTime) / (endTime - startTime);

        prozent = (float)Math.Round(prozent, 3);

        Vector3 wantedScale = startScale + (endScale - startScale) * prozent;

        if (float.IsNaN(wantedScale.x))
            wantedScale.x = 1;
        if (float.IsNaN(wantedScale.y))
            wantedScale.y = 1;
        if (float.IsNaN(wantedScale.z))
            wantedScale.z = 1;

        transform.localScale = wantedScale;
    }
}
