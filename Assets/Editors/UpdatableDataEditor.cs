using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UpdatableData), true)]
public class UpdatableDataEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        UpdatableData data = (UpdatableData) target;
        if (data.autoUpdate) {
            data.NotifyValuesUpdated();
        }
        
        if(GUILayout.Button("Generate Map")) {
            data.NotifyValuesUpdated();
        }
    }
}
