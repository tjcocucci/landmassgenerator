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
        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        float[,] noiseMap = Noise.GenerateNoiseMap(chunkSize, chunkSize, mapScale, seed, octaves, persistance, lacunarity, offsets);
        Color[] colorMap = mapDisplay.GenerateColorMap(noiseMap, colorLevels, drawMode);
        DisplayMap(noiseMap, colorMap);
    }

    public void DisplayMap(float[,] noiseMap, Color[] colorMap) {
        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();

        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture texture = TextureGenerator.TextureFromColorMap (colorMap, width, height);

        if (drawMode == DrawMode.Noise || drawMode == DrawMode.ColorMap) {
            mapDisplay.SetTexture(width, height, texture);
        } else if (drawMode == DrawMode.Mesh) {
            MeshData meshData = MeshGenerator.GenerateMeshData(noiseMap, heightMultiplier, animationCurve, levelOfDetail);
            Mesh mesh = MeshGenerator.MeshDataToMesh(meshData);
            mapDisplay.SetMesh(width, height, texture, mesh);
        }
    }

    public void RequestMapData(System.Action<MapData> callback) {

    }

}

public struct MapData {
    public readonly float[,] noiseMap;
    public readonly Color[] colorMap;
    
}

[System.Serializable]
public class ColorLevel {
    public Color color;
    [Range(0, 1)]
    public float threshold;
}
