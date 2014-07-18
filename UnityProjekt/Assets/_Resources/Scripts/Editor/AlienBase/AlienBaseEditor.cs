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
        for (int index = 0; index < myTarget.parts.ToArray().Length; index++)
        {
            var growingPart = myTarget.parts.ToArray()[index];
            if (growingPart == null)
            {
                myTarget.parts.Remove(growingPart);
            }
        }
        myTarget.parts.Clear();
        if (myTarget)
        {
            for (int index = 0; index < myTarget.GetComponentsInChildren<GrowingPart>().Length; index++)
            {
                var part = myTarget.GetComponentsInChildren<GrowingPart>()[index];
                if (!myTarget.parts.Contains(part))
                {
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

        myTarget.StartUpTime = EditorGUILayout.Slider("StartUpTime", myTarget.StartUpTime, myTarget.minNeededTime, myTarget.maxNeededTime);

        EditorGUILayout.Popup(myTarget.currentState, myTarget.BaseStateList);

        GUILayout.Space(10);

        GUILayout.Label("Base States:");
        scrollValue = EditorGUILayout.BeginScrollView(scrollValue, GUILayout.MaxHeight(300), GUILayout.MinHeight(200));

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

        if (myTarget.StartUpTime < myTarget.StartTime)
            myTarget.StartUpTime = myTarget.StartTime;
        if (myTarget.StartUpTime > myTarget.EndTime)
            myTarget.StartUpTime = myTarget.EndTime;
    }

    private void UpdateStateList()
    {
        myTarget.BaseStateList = new string[myTarget.StateOrder.Count+1];

        for (int i = 0; i < myTarget.StateOrder.Count; i++)
        {
            myTarget.BaseStateList[i] = myTarget.StateOrder[i].Name;
        }

        myTarget.BaseStateList[myTarget.StateOrder.Count] = "All";

        for (int index = 0; index < myTarget.parts.Count; index++)
        {
            var growingPart = myTarget.parts[index];
            growingPart.BaseStateList = myTarget.BaseStateList;
        }
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
        AlienBaseState state = new AlienBaseState();
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

    private Vector2 spawningInfoScrollValue = Vector2.zero;

    private void DrawInspectorForState(AlienBaseState state)
    {

        state.Name = EditorGUILayout.TextField(state.Name);

        EditorGUILayout.LabelField(myTarget.transform.childCount.ToString());

        state.healthBonus = EditorGUILayout.IntSlider("HealthBonus", (int)state.healthBonus, 0, 300);

        EditorGUILayout.MinMaxSlider(new GUIContent("Start/End Time"), ref state.startTime, ref state.endTime, myTarget.minNeededTime, myTarget.maxNeededTime);
        state.startTime = EditorGUILayout.FloatField("Start Time:", state.startTime);
        state.endTime = EditorGUILayout.FloatField("End Time:", state.endTime);

        state.spawning = EditorGUILayout.Toggle("Spawning", state.spawning);
        if (state.spawning)
        {
            state.spawnCooldown = EditorGUILayout.FloatField("Spawn Cooldown:", state.spawnCooldown);

            spawningInfoScrollValue = EditorGUILayout.BeginScrollView(spawningInfoScrollValue, GUILayout.MaxHeight(300), GUILayout.MinHeight(80));

            state.Range = EditorGUILayout.FloatField("Range:", state.Range);

            state.mask = LayerMaskField("Mask:", state.mask, false);

            SpawnInfo deleteInfo = null;

            if(state.spawnInfos == null)
                state.spawnInfos = new List<SpawnInfo>();

            foreach (var spawnInfo in state.spawnInfos)
            {
                spawnInfo.poolName = EditorGUILayout.TextField("PoolName:", spawnInfo.poolName);
                spawnInfo.weight = EditorGUILayout.FloatField("SpawnWeight:", spawnInfo.weight);
                if (GUILayout.Button("Delete SpawnInfo"))
                {
                    deleteInfo = spawnInfo;
                }
            }

            if (deleteInfo != null)
            {
                state.spawnInfos.Remove(deleteInfo);
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Add SpawnInfo"))
            {
                state.spawnInfos.Add(new SpawnInfo());
            }

        }

        if (GUILayout.Button("Delete State"))
        {
            deleteStateIndex = currentStateIndex;
        }

        if (GUI.changed)
        {
            if (state.startTime < 0)
                state.startTime = 0;

            if (currentStateIndex == 0)
            {
                state.startTime = myTarget.minNeededTime;
                if(myTarget.StateCount == 1)
                    state.endTime = myTarget.maxNeededTime;

                UpdateStateTimeAfter(currentStateIndex);
            }
            else if (currentStateIndex + 1 == myTarget.StateOrder.Count)
            {
                state.endTime = myTarget.maxNeededTime;
                UpdateStateTimeBefore(currentStateIndex);
            }
            else if (currentStateIndex > 0 || currentStateIndex < myTarget.StateOrder.Count - 1)
            {
                UpdateStateTimeBefore(currentStateIndex);
                UpdateStateTimeAfter(currentStateIndex);
            }
            EditorUtility.SetDirty(myTarget);
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



    public static List<string> layers;
    public static List<int> layerNumbers;
    public static string[] layerNames;
    public static long lastUpdateTick;

    /** Displays a LayerMask field.
     * \param showSpecial Use the Nothing and Everything selections
     * \param selected Current LayerMask
     * \version Unity 3.5 and up will use the EditorGUILayout.MaskField instead of a custom written one.
     */
    public static LayerMask LayerMaskField(string label, LayerMask selected, bool showSpecial)
    {

        //Unity 3.5 and up

        if (layers == null || (System.DateTime.Now.Ticks - lastUpdateTick > 10000000L && Event.current.type == EventType.Layout))
        {
            lastUpdateTick = System.DateTime.Now.Ticks;
            if (layers == null)
            {
                layers = new List<string>();
                layerNumbers = new List<int>();
                layerNames = new string[4];
            }
            else
            {
                layers.Clear();
                layerNumbers.Clear();
            }

            int emptyLayers = 0;
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);

                if (layerName != "")
                {

                    for (; emptyLayers > 0; emptyLayers--) layers.Add("Layer " + (i - emptyLayers));
                    layerNumbers.Add(i);
                    layers.Add(layerName);
                }
                else
                {
                    emptyLayers++;
                }
            }

            if (layerNames.Length != layers.Count)
            {
                layerNames = new string[layers.Count];
            }
            for (int i = 0; i < layerNames.Length; i++) layerNames[i] = layers[i];
        }

        selected.value = EditorGUILayout.MaskField(label, selected.value, layerNames);

        return selected;
    }
}
