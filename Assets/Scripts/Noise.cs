using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int width, int height, float scale, int seed, int octaves, float persistance, float lacunarity, Vector2 offsets) {
        float[,] noiseMap = new float[width, height];
        System.Random prng = new System.Random(seed);
        Vector2[] ovtaveOffsets = new Vector2[octaves];
        for (int k = 0; k < octaves; k++) {
            ovtaveOffsets[k].x = prng.Next(-100000, 100000) + offsets.x;
            ovtaveOffsets[k].y = prng.Next(-100000, 100000) + offsets.y;
        }

        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                float frequency = 1;
                float amplitude = 1;
                for (int k = 0; k < octaves; k++) {
                    float x = (i - width / 2f) / scale * frequency + ovtaveOffsets[k].x;
                    float y = (j - height / 2f) / scale * frequency + ovtaveOffsets[k].y;
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
            for (int j = 0; j < height; j++) {
                noiseMap[i, j] = Mathf.InverseLerp(minHeight, maxHeight, noiseMap[i, j]);
            }
        }
 
        return noiseMap;
    }
}
