using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

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

    public MapDisplay mapDisplay;
    Queue<ThreadInfo<MapData>> mapDataQueue = new Queue<ThreadInfo<MapData>>();
    Queue<ThreadInfo<MeshData>> meshDataQueue = new Queue<ThreadInfo<MeshData>>();

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

    public void GenerateMap () {
        MapData mapData = GenerateMapData();
        DisplayMap(mapData.noiseMap, mapData.colorMap);
    }
 
    public MapData GenerateMapData () {
        float [,] noiseMap = Noise.GenerateNoiseMap(chunkSize, chunkSize, mapScale, seed, octaves, persistance, lacunarity, offsets);
        Color[] colorMap = ColorMap.GenerateColorMap(noiseMap, colorLevels, drawMode);
        return new MapData(noiseMap, colorMap);
    }

    public void DisplayMap(float[,] noiseMap, Color[] colorMap) {
        mapDisplay = FindObjectOfType<MapDisplay>();
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture texture = TextureGenerator.TextureFromColorMap (colorMap, width, height);

        if (drawMode == DrawMode.Noise || drawMode == DrawMode.ColorMap) {
            mapDisplay.SetTexture(width, height, texture);
        } else if (drawMode == DrawMode.Mesh) {
            MeshData meshData = MeshGenerator.GenerateMeshData(noiseMap, heightMultiplier, animationCurve, levelOfDetail);
            Mesh mesh = meshData.CreateMesh();
            mapDisplay.SetMesh(width, height, texture, mesh);
        }
    }

    public void RequestMapData(Action<MapData> callback) {
        ThreadStart threadStart = delegate {
            ThreadMapData(callback);
        };
        new Thread(threadStart).Start();
    }

    public void ThreadMapData (Action<MapData> callback) {
        MapData threadMapData = GenerateMapData();
        lock (mapDataQueue) {
            mapDataQueue.Enqueue(new ThreadInfo<MapData>(callback, threadMapData));
        }
    }

    public void RequestMeshData(MapData mapData, Action<MeshData> callback)     {
        ThreadStart threadStart = delegate {
            ThreadMeshData(mapData, callback);
        };
        new Thread(threadStart).Start();
    }

    public void ThreadMeshData (MapData mapData, Action<MeshData> callback) {
        MeshData threadMeshData = MeshGenerator.GenerateMeshData(mapData.noiseMap, heightMultiplier, animationCurve, levelOfDetail);
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
