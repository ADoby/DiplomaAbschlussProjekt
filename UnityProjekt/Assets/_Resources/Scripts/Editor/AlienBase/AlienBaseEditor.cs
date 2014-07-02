using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(AlienBase))]
public class AlienBaseEditor : Editor
{
    [SerializeField]
    private AlienBase myTarget;

    [SerializeField]
    private Vector2 scrollValue = Vector2.zero;

    private int currentStateIndex = 0;
    private int deleteStateIndex = -1;

    private bool showTestingGroup;

    public void UpdatePartList()
    {
        foreach (var growingPart in myTarget.parts.ToArray())
        {
            if(growingPart == null)
            {
                myTarget.parts.Remove(growingPart);
            }
            else if (growingPart.myBase != myTarget)
            {
                myTarget.parts.Remove(growingPart);
            }
        }
        myTarget.parts.Clear();
        if (myTarget)
        {
            foreach (var part in myTarget.GetComponentsInChildren<GrowingPart>())
            {
                if (!myTarget.parts.Contains(part))
                {
                    part.myBase = myTarget;
                    //part.endScale = part.transform.localScale;
                    myTarget.parts.Add(part);
                }
                
            }
        }
    }

    private void UpdateNeededReferences()
    {
        if (myTarget.StateOrder == null)
            myTarget.StateOrder = new List<AlienBaseState>();

        if (myTarget.parts.Count == 0)
            UpdatePartList();

        showTestingGroup = myTarget.UpdateGrowingParts;
    }

    public override void OnInspectorGUI()
    {
        myTarget = (AlienBase)target;

        UpdateNeededReferences();

        currentStateIndex = 0;

        GUILayout.Space(5);

        showTestingGroup = EditorGUILayout.BeginToggleGroup("Testing: ", showTestingGroup);
        myTarget.UpdateGrowingParts = showTestingGroup;

        if (showTestingGroup)
        {
            GUILayout.Label("Testing:");
            if (GUILayout.Button("Update Growing Part List"))
            {
                UpdatePartList();
            }
            GUILayout.Label("Growing Parts Count: " + myTarget.parts.Count);

            if (GUILayout.Button("Try Automatic Growing Settings"))
            {
                myTarget.TryAutomaticGrowingSettings();
            }

            if (myTarget.StateOrder.Count != 0)
            {
                myTarget.currentTestState = EditorGUILayout.Popup(myTarget.currentTestState, myTarget.BaseStateList);
                myTarget.currentTime = EditorGUILayout.Slider("Current Time", myTarget.currentTime, myTarget.StartTime, myTarget.EndTime);
            }

            
        }
        EditorGUILayout.EndToggleGroup();

        GUILayout.Space(10);
        GUILayout.Label("Set Up:");

        myTarget.minNeededTime = EditorGUILayout.FloatField("Min Time", myTarget.minNeededTime);
        myTarget.maxNeededTime = EditorGUILayout.FloatField("Max Time", myTarget.maxNeededTime);

        GUILayout.Space(10);

        GUILayout.Label("Base States:");
        scrollValue = EditorGUILayout.BeginScrollView(scrollValue, GUILayout.MaxHeight(300));

        foreach (AlienBaseState state in myTarget.StateOrder)
        {
            DrawInspectorForState(state);

            GUILayout.Space(5);

            currentStateIndex++;
        }

        if (deleteStateIndex != -1)
        {
            DeleteState(deleteStateIndex);
            deleteStateIndex = -1;
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add State"))
        {
            NewState();
        }

        if (GUI.changed)
        {
            if (myTarget.minNeededTime < 0)
                myTarget.minNeededTime = 0;

            UpdateMinMaxTime();

            myTarget.UpdateParts();

            EditorUtility.SetDirty(myTarget);
        }
    }

    private void UpdateMinMaxTime()
    {
        if (myTarget.currentTestState < 0 || myTarget.currentTestState > myTarget.StateCount)
            myTarget.currentTestState = myTarget.StateCount;

        myTarget.StartTime = myTarget.GetMinTime(myTarget.currentTestState);
        myTarget.EndTime = myTarget.GetMaxTime(myTarget.currentTestState);

        if (myTarget.currentTime < myTarget.StartTime)
            myTarget.currentTime = myTarget.StartTime;

        if (myTarget.currentTime > myTarget.EndTime)
            myTarget.currentTime = myTarget.EndTime;
    }

    private void UpdateStateList()
    {
        myTarget.BaseStateList = new string[myTarget.StateOrder.Count+1];

        for (int i = 0; i < myTarget.StateOrder.Count; i++)
        {
            myTarget.BaseStateList[i] = myTarget.StateOrder[i].Name;
        }

        myTarget.BaseStateList[myTarget.StateOrder.Count] = "All";
    }

    private void DeleteState(int index)
    {
        myTarget.StateOrder.RemoveAt(index);
        if (myTarget.currentTestState > myTarget.StateOrder.Count)
        {
            myTarget.currentTestState = myTarget.StateOrder.Count;
        }
        UpdateStateList();
    }

    private void NewState()
    {
        AlienBaseState state = new AlienBaseState(myTarget);
        state.Name = "BaseState " + myTarget.StateOrder.Count;

        if (myTarget.StateOrder.Count != 0)
        {
            state.startTime = myTarget.StateOrder[myTarget.StateOrder.Count - 1].endTime;
        }
        else
        {
            state.startTime = myTarget.minNeededTime;
        }

        state.endTime = myTarget.maxNeededTime;

        if (myTarget.currentTestState == myTarget.StateOrder.Count)
        {
            myTarget.currentTestState = myTarget.StateOrder.Count + 1;
        }

        myTarget.StateOrder.Add(state);

        UpdateStateList();
    }

    private void DrawInspectorForState(AlienBaseState state)
    {

        state.Name = EditorGUILayout.TextField(state.Name);

        state.healthBonus = EditorGUILayout.IntSlider("HealthBonus", (int)state.healthBonus, 0, 300);

        EditorGUILayout.MinMaxSlider(new GUIContent("Start/End Time"), ref state.startTime, ref state.endTime, myTarget.minNeededTime, myTarget.maxNeededTime);
        state.startTime = EditorGUILayout.FloatField("Start Time:", state.startTime);
        state.endTime = EditorGUILayout.FloatField("End Time:", state.endTime);

        if (GUILayout.Button("Delete State"))
        {
            deleteStateIndex = currentStateIndex;
        }

        if (GUI.changed)
        {

            if (state.endTime > myTarget.maxNeededTime)
                myTarget.maxNeededTime = state.endTime;
            if (state.startTime < 0)
                state.startTime = 0;
            if (state.startTime < myTarget.minNeededTime)
                myTarget.minNeededTime = state.startTime;

            if (currentStateIndex == 0)
            {
                UpdateStateTimeAfter(currentStateIndex);
            }
            else if (currentStateIndex + 1 == myTarget.StateOrder.Count)
            {
                UpdateStateTimeBefore(currentStateIndex);
            }
            else if (currentStateIndex > 0 || currentStateIndex < myTarget.StateOrder.Count - 1)
            {
                UpdateStateTimeBefore(currentStateIndex);
                UpdateStateTimeAfter(currentStateIndex);
            }
        }
    }

    private void UpdateStateTimeBefore(int index)
    {
        if (index >= 1)
        {
            AlienBaseState state = myTarget.StateOrder[index];
            AlienBaseState lastState = myTarget.StateOrder[index - 1];

            lastState.endTime = state.startTime;

            if (lastState.endTime < lastState.startTime)
            {
                lastState.startTime = lastState.endTime;
            }

            UpdateStateTimeBefore(index - 1);
        }
    }

    private void UpdateStateTimeAfter(int index)
    {
        if (index < myTarget.StateOrder.Count-1)
        {
            AlienBaseState state = myTarget.StateOrder[index];
            AlienBaseState nextState = myTarget.StateOrder[index + 1];

            nextState.startTime = state.endTime;

            if (nextState.startTime > nextState.endTime)
            {
                nextState.endTime = nextState.startTime;
            }

            UpdateStateTimeAfter(index + 1);
        }
    }
}
