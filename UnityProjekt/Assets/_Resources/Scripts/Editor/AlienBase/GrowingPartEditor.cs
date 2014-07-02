using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GrowingPart))]
public class GrowingPartEditor : Editor {

    [SerializeField]
    private GrowingPart myTarget;

    public override void OnInspectorGUI()
    {
        myTarget = (GrowingPart) target;

        if (!myTarget.myBase)
        {
            EditorGUILayout.LabelField("Click \"Update Growing Part List\" in the AlienBase inspector,");
            return;
        }

        if (myTarget.myBase.BaseStateList.Length != 0)
        {
            myTarget.currentBaseStateIndex = EditorGUILayout.Popup(myTarget.currentBaseStateIndex, myTarget.myBase.BaseStateList);
        }
        
        EditorGUILayout.MinMaxSlider(new GUIContent("Growing Time"), ref myTarget.startTime, ref myTarget.endTime, myTarget.minTime, myTarget.maxTime);

        myTarget.startTime = EditorGUILayout.FloatField("Start Time:", myTarget.startTime);
        myTarget.endTime = EditorGUILayout.FloatField("End Time:", myTarget.endTime);

        EditorGUILayout.LabelField("Current Time: " + myTarget.currentTime);

        myTarget.startScale = EditorGUILayout.Vector3Field("Start Scale", myTarget.startScale);
        myTarget.endScale = EditorGUILayout.Vector3Field("End Scale", myTarget.endScale);

        if (GUILayout.Button("Reset Scales"))
        {
            myTarget.ResetScales();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(myTarget);
        }
    }
}
