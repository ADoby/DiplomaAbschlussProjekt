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

        if (myTarget.BaseStateList != null && myTarget.BaseStateList.Length != 0)
        {
            myTarget.currentBaseStateIndex = EditorGUILayout.Popup(myTarget.currentBaseStateIndex, myTarget.BaseStateList);
        }
        
        EditorGUILayout.MinMaxSlider(new GUIContent("Growing Time"), ref myTarget.startTime, ref myTarget.endTime, myTarget.minTime, myTarget.maxTime);

        myTarget.startTime = EditorGUILayout.FloatField("Start Time:", myTarget.startTime);
        myTarget.endTime = EditorGUILayout.FloatField("End Time:", myTarget.endTime);

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
