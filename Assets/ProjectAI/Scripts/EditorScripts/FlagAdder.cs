using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FlagMono))]
public class FlagAdder : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FlagMono mono = (FlagMono)target;

        if(GUILayout.Button("Add flags"))
        {
            mono.stuff();
        }
    }
}
