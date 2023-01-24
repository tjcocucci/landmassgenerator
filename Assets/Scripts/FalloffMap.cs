using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffMap
{
    public static float[,] GenerateFalloffMap(int size) {
        float[,] map = new float[size, size];

        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                float a = Mathf.Min((float) (i % size), (float) ((size - i) % size));
                float b = Mathf.Min((float) (j % size), (float) ((size - j) % size));
                float value = Mathf.Min(a, b);
                map[i, j] = value;
                if (map[i, j] > maxHeight) {
                    maxHeight = map[i, j];
                }
                if (map[i, j] < minHeight) {
                    minHeight = map[i, j];
                }
            }
        }
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                map[i, j] = Sigmoid(Mathf.InverseLerp(minHeight, maxHeight, map[i, j]));
            }
        }
    return map;
    }

    public static float[,] MaskMapWithFalloffMap(float[,] noiseMap, float[,] falloffMap) {
        int size = noiseMap.GetLength(0);
        float[,] map = new float[size, size];
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                float value = noiseMap[i, j] * falloffMap[i, j];
                map[i, j] = value;
            }
        }
    return map;
    }

    static float Sigmoid (float x) {
        float a = 4;
        float b = 0.35f;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(x * b - b, a));
    }

}
