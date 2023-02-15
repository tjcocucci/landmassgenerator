using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatableData : ScriptableObject
{
    public event System.Action OnValuesUpdated;
    public bool autoUpdate;

    protected virtual void OnValidate () {
        if (autoUpdate) {
            UnityEditor.EditorApplication.update += NotifyValuesUpdated;
        }
    }

    public void NotifyValuesUpdated () {
        UnityEditor.EditorApplication.update -= NotifyValuesUpdated;
        if (OnValuesUpdated != null) {
            OnValuesUpdated();
        }
    }

}
