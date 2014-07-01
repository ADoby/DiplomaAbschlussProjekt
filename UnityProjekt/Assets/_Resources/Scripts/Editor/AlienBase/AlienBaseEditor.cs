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

    private int currentStateIndex = 0;

    public override void OnInspectorGUI()
    {
        myTarget = (AlienBase)target;

        currentStateIndex = 0;

        if(myTarget.StateOrder == null)
            myTarget.StateOrder = new List<AlienBaseState>();

        foreach (AlienBaseState state in myTarget.StateOrder)
        {
            DrawInspectorForState(state);

            EditorGUILayout.Space();

            currentStateIndex++;
        }

        if (GUILayout.Button("Add State"))
        {
            myTarget.StateOrder.Add(new AlienBaseState(myTarget));
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(myTarget);
        }
    }

    private void DrawInspectorForState(AlienBaseState state)
    {
        state.healthBonus = EditorGUILayout.IntSlider("HealthBonus", (int)state.healthBonus, 0, 300);

        EditorGUILayout.MinMaxSlider(new GUIContent("Start/End Time"), ref state.startTime, ref state.endTime, 0, 240);
        state.startTime = EditorGUILayout.FloatField("Start Time:", state.startTime);
        state.endTime = EditorGUILayout.FloatField("End Time:", state.endTime);

        if (GUI.changed)
        {
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
