using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorMap
{
    public static Color[] GenerateColorMap (float[,] noiseMap, ColorLevel[] colorLevels, MapGenerator.DrawMode drawMode, int width, int height) {
        Color[] colorMap = null;
        if (drawMode == MapGenerator.DrawMode.Noise || drawMode == MapGenerator.DrawMode.Falloff) {
            colorMap = GenerateNoiseColorMap(noiseMap, width, height);
        } else if (drawMode == MapGenerator.DrawMode.ColorMap) {
            colorMap = GenerateLeveledColorMap(noiseMap, colorLevels, width, height);
        } else if (drawMode == MapGenerator.DrawMode.Mesh) {
            colorMap = GenerateLeveledColorMap(noiseMap, colorLevels, width, height);
        }
        return colorMap;
    }

    public static Color[] GenerateNoiseColorMap(float[,] noiseMap, int width, int height) {
        Color[] colorMap = new Color[width*height];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                colorMap[j * width + i] = Color.Lerp(Color.white, Color.black, noiseMap[i, j]);
            }
        }
        return colorMap;
    }

    public static Color[] GenerateLeveledColorMap(float[,] noiseMap, ColorLevel[] colorLevels, int width, int height) {
        Color[] colorMap = new Color[width*height];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                for (int k = 0; k < colorLevels.Length; k++) {
                    if(noiseMap[i, j] >= colorLevels[k].threshold) {
                        colorMap[j * width + i] = colorLevels[k].color;
                    } else {
                        break;
                    }
                }
            }
        }
        return colorMap;
    }

}
