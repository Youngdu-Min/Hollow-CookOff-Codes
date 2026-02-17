using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomButtonKey))]
public class CustomButtonEditor : Editor
{
    SerializedProperty triggerKey;

    private void OnEnable()
    {
        triggerKey = serializedObject.FindProperty("triggerKey");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
       // EditorGUILayout.PropertyField(triggerKey);
       // serializedObject.ApplyModifiedProperties();

    }
}
