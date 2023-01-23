using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum NormalizeMode {Local, Global};

    public static float[,] GenerateNoiseMap(int width, int height, float scale, int seed, int octaves, float persistance, float lacunarity, Vector2 offsets, NormalizeMode normalizeMode) {
        float[,] noiseMap = new float[width, height];
        System.Random prng = new System.Random(seed);
        Vector2[] ovtaveOffsets = new Vector2[octaves];
        float maxPossibleHeight = 0;
        float frequency = 1;
        float amplitude = 1;

        for (int k = 0; k < octaves; k++) {
            ovtaveOffsets[k].x = prng.Next(-100000, 100000) + offsets.x;
            ovtaveOffsets[k].y = prng.Next(-100000, 100000) - offsets.y;
            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                frequency = 1;
                amplitude = 1;
                for (int k = 0; k < octaves; k++) {
                    float x = (i  + ovtaveOffsets[k].x - width / 2f) / scale * frequency;
                    float y = (j  + ovtaveOffsets[k].y - height / 2f) / scale * frequency;
                    noiseMap[i, j] += (Mathf.PerlinNoise(x, y) * 2 - 1)  * amplitude;
                    frequency *= lacunarity;
                    amplitude *= persistance;
                }
                if (noiseMap[i, j] > maxHeight) {
                    maxHeight = noiseMap[i, j];
                }
                if (noiseMap[i, j] < minHeight) {
                    minHeight = noiseMap[i, j];
                }
            }
        }

        for (int i = 0; i < width; i++) {
            if (normalizeMode == NormalizeMode.Local) {
                for (int j = 0; j < height; j++) {
                    noiseMap[i, j] = Mathf.InverseLerp(minHeight, maxHeight, noiseMap[i, j]);
                }
            } else {
                for (int j = 0; j < height; j++) {
                    float normalizedHeight = (1 + noiseMap[i, j] / maxPossibleHeight) / 2f;
                    normalizedHeight /= 1.1f;
                    noiseMap[i, j] = Mathf.Clamp(normalizedHeight, 0, float.MaxValue);
                }
            }
        }
 
        return noiseMap;
    }
}
