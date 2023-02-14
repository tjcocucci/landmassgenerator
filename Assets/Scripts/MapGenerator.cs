using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class MapGenerator: MonoBehaviour
{

    public enum DrawMode {Noise, ColorMap, Mesh, Falloff};
    public DrawMode drawMode;

    [Range(0, 6)]
    public int editorPreviewLevelOfDetail;

    public NoiseData noiseData;
    public TerrainData terrainData;
    public TextureData textureData;

    public  Material material;
    public bool autoUpdate;
    public static MapGenerator instance;

    public MapDisplay mapDisplay;
    Queue<ThreadInfo<MapData>> mapDataQueue = new Queue<ThreadInfo<MapData>>();
    Queue<ThreadInfo<MeshData>> meshDataQueue = new Queue<ThreadInfo<MeshData>>();

    public int chunkSize {
        get {
                return terrainData.useFlatShading ? 95 : 239;
            }
    }

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

    void OnTextureValuesUpdated () {
        if (!Application.isPlaying) {
            GenerateMap();
        }
        textureData.ApplyToMaterial(material);
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
        if (textureData != null) {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
    }

    public void GenerateMap () {
        MapData mapData = GenerateMapData(Vector2.zero);
        DisplayMap(mapData.noiseMap);
    }
 
    public MapData GenerateMapData (Vector2 center) {
        float [,] noiseMap;
        float [,] falloffMap;
        falloffMap = FalloffMap.GenerateFalloffMap(chunkSize + 2);
        noiseMap = Noise.GenerateNoiseMap(chunkSize + 2, chunkSize + 2, terrainData.mapScale, noiseData.seed, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offsets + center, noiseData.normalizeMode);
        if (drawMode == DrawMode.Falloff) {
            return new MapData(falloffMap);
        } else {
            if (terrainData.falloffEnabled) {
                noiseMap = FalloffMap.MaskMapWithFalloffMap(noiseMap, falloffMap); 
            }
            textureData.UpdateHeights(material, terrainData.minHeight, terrainData.maxHeight);
            return new MapData(noiseMap);
        }
    }

    public void DisplayMap(float[,] noiseMap) {
        mapDisplay = FindObjectOfType<MapDisplay>();
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        
        // Texture2D texture = TextureGenerator.TextureFromColorMap (width-2, height-2);

        if (drawMode == DrawMode.Noise || drawMode == DrawMode.ColorMap || drawMode == DrawMode.Falloff ) {
            // mapDisplay.SetTexture(texture);
        } else if (drawMode == DrawMode.Mesh) {
            MeshData meshData = MeshGenerator.GenerateMeshData(noiseMap, terrainData.heightMultiplier, terrainData.animationCurve, editorPreviewLevelOfDetail, terrainData.useFlatShading);
            Mesh mesh = meshData.CreateMesh();
            mapDisplay.SetMesh(mesh);
            textureData.ApplyToMaterial(material);
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

    public MapData (float[,] noiseMap) {
        this.noiseMap = noiseMap;
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
