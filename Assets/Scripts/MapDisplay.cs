using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer renderTexture;

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

}
