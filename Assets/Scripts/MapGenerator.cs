using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator: MonoBehaviour
{
    [Min(1)]
    public int mapWidth;
    [Min(1)]
    public int mapHeight;
    public float mapScale;
    [Min(1)]
    public int octaves;
    [Min(0)]
    public float lacunarity;
    [Min(0)]
    public float persistance;

    public int seed;
    public Vector2 offsets;

    public bool autoUpdate;

    public void GenerateMap () {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, mapScale, seed, octaves, persistance, lacunarity, offsets);
        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        mapDisplay.DisplayMap(noiseMap);
    }

}
