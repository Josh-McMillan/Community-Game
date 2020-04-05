using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(PopInPanel))]
public class PopInPanelEditor : ImageEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();


        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Pop Scale In/Out Settings");
       // EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("popTime"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("popInDelay"));
       // EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("popScaleTween"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startScale"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Fade In/Out Settings");
       // EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeTime"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeInDelay"));
        // EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
       // EditorGUILayout.BeginHorizontal();
        //This is a mult for the closing window speed
        EditorGUILayout.PropertyField(serializedObject.FindProperty("closeTimeMult"));
        //"Does this panel opening and closing ignor time.Scale"
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ignorTimeScale"));
       // EditorGUILayout.EndHorizontal();


        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
