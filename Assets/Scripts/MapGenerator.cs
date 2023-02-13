using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class MapGenerator: MonoBehaviour
{

    public enum DrawMode {Noise, ColorMap, Mesh, Falloff};
    public DrawMode drawMode;
    // public Noise.NormalizeMode normalizeMode;

    // public bool useFlatShading;

    [Range(0, 6)]
    public int editorPreviewLevelOfDetail;
    // public float mapScale;
    // [Min(1)]
    // public int octaves;
    // [Min(0)]
    // public float lacunarity;
    // [Min(0)]
    // public float persistance;
    // public bool falloffEnabled;

    // public float heightMultiplier;
    // public AnimationCurve animationCurve;

    public ColorLevel[] colorLevels;

    // public int seed;
    // public Vector2 offsets;

    public NoiseData noiseData;
    public TerrainData terrainData;

    public bool autoUpdate;
    static MapGenerator instance;

    public MapDisplay mapDisplay;
    Queue<ThreadInfo<MapData>> mapDataQueue = new Queue<ThreadInfo<MapData>>();
    Queue<ThreadInfo<MeshData>> meshDataQueue = new Queue<ThreadInfo<MeshData>>();

    public int chunkSize = 239;
    // public static int chunkSize {
    //     get {
    //             if (instance == null) {
    //                 instance = FindObjectOfType<MapGenerator>();
    //             }
    //             return instance.terrainData.useFlatShading ? 95 : 239;
    //         }
    // }

    public void Update() {
        while (mapDataQueue.Count > 0) {
            for (int i = 0; i < mapDataQueue.Count; i++) {
                ThreadInfo<MapData> threadInfo = mapDataQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        while (meshDataQueue.Count > 0) {
            for (int i = 0; i < meshDataQueue.Count; i++) {
                ThreadInfo<MeshData> threadInfo = meshDataQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    void OnValuesUpdated () {
        if (!Application.isPlaying) {
            GenerateMap();
        }
    }

    void OnValidate () {
        if (terrainData != null) {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        }
        if (noiseData != null) {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }
    }

    public void GenerateMap () {
        MapData mapData = GenerateMapData(Vector2.zero);
        DisplayMap(mapData.noiseMap, mapData.colorMap);
    }
 
    public MapData GenerateMapData (Vector2 center) {
        float [,] noiseMap;
        float [,] falloffMap;
        Color[] colorMap;
        falloffMap = FalloffMap.GenerateFalloffMap(chunkSize);
        noiseMap = Noise.GenerateNoiseMap(chunkSize + 2, chunkSize + 2, terrainData.mapScale, noiseData.seed, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offsets + center, noiseData.normalizeMode);
        if (drawMode == DrawMode.Falloff) {
            colorMap = ColorMap.GenerateColorMap(falloffMap, colorLevels, drawMode, chunkSize, chunkSize);
            return new MapData(falloffMap, colorMap);
        } else {
            if (terrainData.falloffEnabled) {
                noiseMap = FalloffMap.MaskMapWithFalloffMap(noiseMap, falloffMap); 
            }
            colorMap = ColorMap.GenerateColorMap(noiseMap, colorLevels, drawMode, chunkSize, chunkSize);
            return new MapData(noiseMap, colorMap);
        }
    }

    public void DisplayMap(float[,] noiseMap, Color[] colorMap) {
        mapDisplay = FindObjectOfType<MapDisplay>();
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        
        Texture2D texture = TextureGenerator.TextureFromColorMap (colorMap, width-2, height-2);

        if (drawMode == DrawMode.Noise || drawMode == DrawMode.ColorMap || drawMode == DrawMode.Falloff ) {
            mapDisplay.SetTexture(texture);
        } else if (drawMode == DrawMode.Mesh) {
            MeshData meshData = MeshGenerator.GenerateMeshData(noiseMap, terrainData.heightMultiplier, terrainData.animationCurve, editorPreviewLevelOfDetail, terrainData.useFlatShading);
            Mesh mesh = meshData.CreateMesh();
            mapDisplay.SetMesh(texture, mesh);
        }
    }

    public void RequestMapData(Vector2 center, Action<MapData> callback) {
        ThreadStart threadStart = delegate {
            ThreadMapData(center, callback);
        };
        new Thread(threadStart).Start();
    }

    public void ThreadMapData (Vector2 center, Action<MapData> callback) {
        MapData threadMapData = GenerateMapData(center);
        lock (mapDataQueue) {
            mapDataQueue.Enqueue(new ThreadInfo<MapData>(callback, threadMapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)     {
        ThreadStart threadStart = delegate {
            ThreadMeshData(mapData, lod, callback);
        };
        new Thread(threadStart).Start();
    }

    public void ThreadMeshData (MapData mapData, int lod, Action<MeshData> callback) {
        MeshData threadMeshData = MeshGenerator.GenerateMeshData(mapData.noiseMap, terrainData.heightMultiplier, terrainData.animationCurve, lod, terrainData.useFlatShading);
        lock (meshDataQueue) {
            meshDataQueue.Enqueue(new ThreadInfo<MeshData>(callback, threadMeshData));
        }
    }

}

public struct MapData {
    public readonly float[,] noiseMap;
    public readonly Color[] colorMap;

    public MapData (float[,] noiseMap, Color[] colorMap) {
        this.noiseMap = noiseMap;
        this.colorMap = colorMap;
    }

}

public struct ThreadInfo<T> {
    public readonly Action<T> callback;
    public readonly T parameter;

    public ThreadInfo (Action<T> callback, T parameter) {
        this.callback = callback;
        this.parameter = parameter;
    }

}

[System.Serializable]
public class ColorLevel {
    public Color color;
    [Range(0, 1)]
    public float threshold;
}
