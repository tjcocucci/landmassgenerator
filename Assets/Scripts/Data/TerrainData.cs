using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdatableData
{
    public float mapScale;
    public bool useFlatShading;
    public bool falloffEnabled;
    public float heightMultiplier;
    public AnimationCurve animationCurve;

    public float minHeight {
        get {
            return animationCurve.Evaluate(0) * heightMultiplier;
        }
    }

    public float maxHeight {
        get {
            return animationCurve.Evaluate(1) * heightMultiplier;
        }
    }

}
