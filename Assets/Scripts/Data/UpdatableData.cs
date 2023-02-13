using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatableData : ScriptableObject
{
    public event System.Action OnValuesUpdated;
    public bool autoUpdate;

    protected virtual void _OnValidate () {
        if (autoUpdate) {
            NotifyValuesUpdated();
        }
    }

    protected virtual void OnValidate () {
        UnityEditor.EditorApplication.delayCall += _OnValidate;
    }

    public void NotifyValuesUpdated () {
        if (OnValuesUpdated != null) {
            OnValuesUpdated();
        }
    }

}
