using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : UpdatableData
{
    public Noise.NormalizeMode normalizeMode;
    [Min(1)]
    public int octaves;
    [Min(0)]
    public float lacunarity;
    [Min(0)]
    public float persistance;
    public int seed;
    public Vector2 offsets;

}
