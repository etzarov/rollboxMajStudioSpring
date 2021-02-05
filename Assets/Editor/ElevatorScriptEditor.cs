using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ElevatorScript))]
public class ElevatorScriptEditor : Editor
{
    SerializedProperty startPos;
    SerializedProperty endPos;

    private void OnEnable()
    {
        startPos = serializedObject.FindProperty("PlatStart");
        endPos = serializedObject.FindProperty("PlatEnd");
    }

    public override void OnInspectorGUI()
    {
       
        ElevatorScript scr = (ElevatorScript)target;

        Rigidbody2D rb = scr.GetComponent<Rigidbody2D>();
        scr.rb = rb;

        base.OnInspectorGUI();

        if (scr.usedButton == null && !scr.alwaysActive)
        {
            EditorGUILayout.HelpBox("No button is assigned! Assign a button in the scene or enable Always Active.", MessageType.Error);
        }
        else if (!scr.usedButton.GetComponent<ButtonScript>() && !scr.alwaysActive)
        {
            EditorGUILayout.HelpBox("The 'Used Button' does not have a ButtonScript.cs component attached! The elevator/door will not function.",MessageType.Error);
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Set Platform Positions",EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Sets the target move position for the platform. Position on scene start will be its starting position.",EditorStyles.miniLabel);

        if (!Application.isPlaying)
        {
            startPos.vector2Value = scr.rb.position;


            if (GUILayout.Button("Set Target Position"))
            {
                endPos.vector2Value = scr.rb.position;
            }
        }



        EditorGUILayout.LabelField("Start Position: " + scr.PlatStart);
        EditorGUILayout.LabelField("Target Position: " + scr.PlatEnd);

        serializedObject.ApplyModifiedProperties();

    }
}
