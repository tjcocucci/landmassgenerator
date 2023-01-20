using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer renderTexture;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void SetTexture(int width, int height, Texture texture) {
        renderTexture.sharedMaterial.SetTexture("_BaseMap", texture);
        renderTexture.transform.localScale = new Vector3(width, 1, height);
    }

    public void SetMesh(int width, int height, Texture texture, Mesh mesh) {
        meshRenderer.sharedMaterial.SetTexture("_BaseMap", texture);
        meshFilter.sharedMesh = mesh;
        meshFilter.transform.localScale = new Vector3(width, -1, height);
    }

    public Color[] GenerateNoiseColorMap(float[,] noiseMap) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Color[] colorMap = new Color[width*height];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                colorMap[j * width + i] = Color.Lerp(Color.white, Color.black, noiseMap[i, j]);
            }
        }
        return colorMap;
    }

    public Color[] GenerateColorMap (float[,] noiseMap, ColorLevel[] colorLevels, MapGenerator.DrawMode drawMode) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Color[] colorMap = null;
        if (drawMode == MapGenerator.DrawMode.Noise) {
            colorMap = GenerateNoiseColorMap(noiseMap);
        } else if (drawMode == MapGenerator.DrawMode.ColorMap) {
            colorMap = GenerateLeveledColorMap(noiseMap, colorLevels);
        } else if (drawMode == MapGenerator.DrawMode.Mesh) {
            colorMap = GenerateLeveledColorMap(noiseMap, colorLevels);
        }
        return colorMap;
    }

    public Color[] GenerateLeveledColorMap(float[,] noiseMap, ColorLevel[] colorLevels) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Color[] colorMap = new Color[width*height];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                for (int k = 0; k < colorLevels.Length; k++) {
                    if(noiseMap[i, j] >= colorLevels[k].threshold) {
                        colorMap[j * width + i] = colorLevels[k].color;
                    }
                }
            }
        }
        return colorMap;
    }

}
