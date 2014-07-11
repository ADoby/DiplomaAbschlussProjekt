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

    public int currentState = 0;

    public float currentTime = 0;

    public List<GrowingPart> parts = new List<GrowingPart>();

    public bool UpdateGrowingParts = false;

    public float StartUpTime = 0;

    public bool UpdateTime = true;

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

    void Awake()
    {
        GameEventHandler.OnPause += OnPause;
        GameEventHandler.OnResume += OnResume;
    }

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        currentTime = StartUpTime;
        
        FindCorrectState();

        UpdateParts();
    }

    void OnPause()
    {
        enabled = false;
    }

    void OnResume()
    {
        enabled = true;
    }

    public void FindCorrectState()
    {
        for (int index = 0; index < StateOrder.Count; index++)
        {
            var alienBaseState = StateOrder[index];
            alienBaseState.Reset();
            alienBaseState.BaseTransform = transform;

            if (alienBaseState.startTime <= currentTime && alienBaseState.endTime >= currentTime)
            {
                currentState = StateOrder.IndexOf(alienBaseState);
                alienBaseState.currentTime = currentTime;
            }
        }
    }

    public void TryAutomaticGrowingSettings()
    {
        Hirarchy hirarchy = GetHirarchy(transform);

        while (true)
        {
            List<GrowingPart> currentGrowingParts = hirarchy.NextParts();

            for (int index = 0; index < currentGrowingParts.Count; index++)
            {
                var currentPart = currentGrowingParts[index];
                currentPart.currentBaseStateIndex = StateOrder.Count;
                currentPart.UpdateMinMaxTime(this);

                currentPart.startTime = currentPart.minTime +
                                        hirarchy.CurrentProzent*(currentPart.maxTime - currentPart.minTime);

                currentPart.endTime = currentPart.startTime +
                                      hirarchy.ProzentPerPart*(currentPart.maxTime - currentPart.minTime);

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

    public void SetCurrentTime(float newTime)
    {
        currentTime = Mathf.Clamp(newTime, minNeededTime, maxNeededTime);
        if (currentTime < StateOrder[currentState].startTime || currentTime > StateOrder[currentState].endTime)
        {
            FindCorrectState();
        }
        UpdateParts();
    }


    // Update is called once per frame
	void Update () {
	    if (Application.isPlaying)
	    {
	        StateOrder[currentState].Update();

	        if (UpdateTime)
	        {
                currentTime += Time.deltaTime;
                currentTime = Mathf.Clamp(currentTime, minNeededTime, maxNeededTime);

	            if (currentTime > StateOrder[currentState].endTime)
	            {
	                NextState();
	            }
	        }
            
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
        for (int index = 0; index < parts.ToArray().Length; index++)
        {
            var growingPart = parts.ToArray()[index];
            if (growingPart)
            {
                growingPart.UpdateMinMaxTime(this);
                growingPart.UpdateScale(currentTime);
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
