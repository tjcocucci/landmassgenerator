using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{

    public static Texture TextureFromColorMap (Color[] colorMap, int width, int height) {
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(colorMap);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        // renderTexture.sharedMaterial.SetTexture("_BaseMap", texture);
        // renderTexture.transform.localScale = new Vector3(width, 1, height);
        return texture;
    }

    public static Color[] GenerateNoiseColorMap(float[,] noiseMap) {
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

    public static Color[] GenerateLeveledColorMap(float[,] noiseMap, ColorLevel[] colorLevels) {
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
