using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer renderTexture;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DisplayNoiseMap(float[,] noiseMap) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Color[] colorMap = TextureGenerator.GenerateNoiseColorMap(noiseMap);
        Texture texture = TextureGenerator.TextureFromColorMap (colorMap, width, height);
        renderTexture.sharedMaterial.SetTexture("_BaseMap", texture);
        renderTexture.transform.localScale = new Vector3(width, 1, height);
    }

    public void DisplayColorMap(float[,] noiseMap, ColorLevel[] colorLevels) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Color[] colorMap = TextureGenerator.GenerateLeveledColorMap(noiseMap, colorLevels);
        Texture texture = TextureGenerator.TextureFromColorMap (colorMap, width, height);
        renderTexture.sharedMaterial.SetTexture("_BaseMap", texture);
        renderTexture.transform.localScale = new Vector3(width, 1, height);
    }

    public void DisplayMeshColorMap(float[,] noiseMap, ColorLevel[] colorLevels, float heightMultiplier, AnimationCurve animationCurve, int levelOfDetail) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Color[] colorMap = TextureGenerator.GenerateLeveledColorMap(noiseMap, colorLevels);
        Texture texture = TextureGenerator.TextureFromColorMap (colorMap, width, height);
        MeshData meshData = MeshGenerator.GenerateMeshData(noiseMap, heightMultiplier, animationCurve, levelOfDetail);
        Mesh mesh = MeshGenerator.MeshDataToMesh(meshData);

        meshRenderer.sharedMaterial.SetTexture("_BaseMap", texture);
        meshFilter.sharedMesh = mesh;
        meshFilter.transform.localScale = new Vector3(width, -1, height);
    }

}
