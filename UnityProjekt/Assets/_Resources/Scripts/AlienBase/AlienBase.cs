using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[ExecuteInEditMode]
public class AlienBase : MonoBehaviour
{

    public List<AlienBaseState> StateOrder;

    public string[] BaseStateList = new string[0];

    private int currentState = 0;

    public float currentTime = 0;

    public List<GrowingPart> parts = new List<GrowingPart>();

    public bool UpdateGrowingParts = false;

    public float StartUpTime = 0;

    #region MembersForEditor

    public int currentTestState = 0;

    public float StartTime = 0;

    public float EndTime = 0;

    public float minNeededTime = 0.0f;

    public float maxNeededTime = 120.0f;

    #endregion

    public float GetMinTime(int stateIndex)
    {
        if (stateIndex >= StateOrder.Count || StateOrder.Count == 0 || stateIndex < 0)
            return minNeededTime;
        return StateOrder[stateIndex].startTime;
    }
    public float GetMaxTime(int stateIndex)
    {
        if (stateIndex >= StateOrder.Count || StateOrder.Count == 0 || stateIndex < 0)
            return maxNeededTime;
        return StateOrder[stateIndex].endTime;
    }

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        currentTime = StartUpTime;

        foreach (var alienBaseState in StateOrder)
        {
            alienBaseState.Reset();

            if (alienBaseState.startTime <= currentTime && alienBaseState.endTime >= currentTime)
            {
                currentState = StateOrder.IndexOf(alienBaseState);
                alienBaseState.currentTime = currentTime;
            }
        }

        UpdateParts();
    }

    public void TryAutomaticGrowingSettings()
    {
        Hirarchy hirarchy = GetHirarchy(transform);

        while (true)
        {
            List<GrowingPart> currentGrowingParts = hirarchy.NextParts();

            foreach (var currentPart in currentGrowingParts)
            {
                currentPart.myBase = this;
                currentPart.currentBaseStateIndex = StateOrder.Count;
                currentPart.UpdateMinMaxTime();

                currentPart.startTime = currentPart.minTime + 
                    hirarchy.CurrentProzent * (currentPart.maxTime - currentPart.minTime);

                currentPart.endTime = currentPart.startTime +
                                      hirarchy.ProzentPerPart * (currentPart.maxTime - currentPart.minTime);

#if UNITY_EDITOR
                EditorUtility.SetDirty(currentPart);
#endif
            }

            if (hirarchy.IsLast)
            {
                break;
            }
        }
    }

    public Hirarchy GetHirarchy(Transform trans)
    {
        Hirarchy hirarchy = new Hirarchy();

        AddPartsForLevel(trans, hirarchy, 0);

        return hirarchy;
    }

    public void AddPartsForLevel(Transform searchTransform, Hirarchy hirarchy, int index)
    {
        int nextIndex = index;
        foreach (Transform childTransform in searchTransform)
        {
            if (childTransform.GetComponent<GrowingPart>())
            {
                if (nextIndex == index)
                    nextIndex++;

                hirarchy.AddPart(childTransform.GetComponent<GrowingPart>(), index);
            }

            AddPartsForLevel(childTransform, hirarchy, nextIndex);
        }
    }

    

    // Update is called once per frame
	void Update () {
	    if (Application.isPlaying)
	    {
            if (StateOrder[currentState].Update())
            {
                NextState();
            }

            currentTime = StateOrder[currentState].currentTime;
	        UpdateParts();
	    }
	    else
	    {
            if (UpdateGrowingParts)
            {
                UpdateParts();
            }
	    }
	}

    public void UpdateParts()
    {
        foreach (var growingPart in parts.ToArray())
        {
            if (growingPart)
            {
                growingPart.UpdateMinMaxTime();
                growingPart.UpdateScale();
            }
            else
            {
                parts.Remove(growingPart);
            }
        }
    }

    public int StateCount
    {
        get { return StateOrder.Count; }
    }

    private void NextState()
    {
        if(currentState < StateOrder.Count - 1)
            currentState++;
    }

}

public class Hirarchy
{

    private int currentIndex = -1;

    public List<List<GrowingPart>> GrowingParts = new List<List<GrowingPart>>();

    public void AddPart(GrowingPart newPart, int index)
    {
        NewLevel(index);
        GrowingParts[index].Add(newPart);
    }

    public void NewLevel(int index)
    {
        if (index >= GrowingParts.Count)
        {
            GrowingParts.Add(new List<GrowingPart>());
        }
    }

    public bool IsLast
    {
        get { return currentIndex == GrowingParts.Count-1; }
    }

    public List<GrowingPart> NextParts()
    {
        currentIndex ++;
        return GrowingParts[currentIndex];
    }

    public float CurrentProzent
    {
        get { return (float)currentIndex/GrowingParts.Count; }
    }

    public float ProzentPerPart
    {
        get { return 1.0f/GrowingParts.Count; }
    }
}
