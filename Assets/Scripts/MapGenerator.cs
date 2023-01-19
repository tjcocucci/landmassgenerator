using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator: MonoBehaviour
{

    public enum DrawMode {Noise, ColorMap, Mesh};
    public DrawMode drawMode;

    const int chunkSize = 241;

    [Range(0, 6)]
    public int levelOfDetail;
    public float mapScale;
    [Min(1)]
    public int octaves;
    [Min(0)]
    public float lacunarity;
    [Min(0)]
    public float persistance;

    public float heightMultiplier;
    public AnimationCurve animationCurve;

    public ColorLevel[] colorLevels;

    public int seed;
    public Vector2 offsets;

    public bool autoUpdate;

    public void GenerateMap () {
        float[,] noiseMap = Noise.GenerateNoiseMap(chunkSize, chunkSize, mapScale, seed, octaves, persistance, lacunarity, offsets);
        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.Noise) {
            mapDisplay.DisplayNoiseMap(noiseMap);
        } else if (drawMode == DrawMode.ColorMap) {
            mapDisplay.DisplayColorMap(noiseMap, colorLevels);
        } else if (drawMode == DrawMode.Mesh) {
            mapDisplay.DisplayMeshColorMap(noiseMap, colorLevels, heightMultiplier, animationCurve, levelOfDetail);
        }
        
    }

}

[System.Serializable]
public class ColorLevel {
    public Color color;
    [Range(0, 1)]
    public float threshold;
}
