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
}
