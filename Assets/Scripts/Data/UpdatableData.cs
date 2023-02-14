using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatableData : ScriptableObject
{
    public event System.Action OnValuesUpdated;
    public bool autoUpdate;

    protected virtual void _OnValidate () {
        UnityEditor.EditorApplication.update -= _OnValidate;
        NotifyValuesUpdated();
    }

    protected virtual void OnValidate () {
        if (autoUpdate) {
            UnityEditor.EditorApplication.update += _OnValidate;
        }
    }

    public void NotifyValuesUpdated () {
        if (OnValuesUpdated != null) {
            OnValuesUpdated();
        }
    }

}
