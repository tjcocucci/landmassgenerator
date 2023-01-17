using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer renderTexture;

    public void DisplayMap(float[,] noiseMap) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colorMap = new Color[width*height];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                colorMap[j * width + i] = Color.Lerp(Color.white, Color.black, noiseMap[i, j]);
            }
        }
        texture.SetPixels(colorMap);
        texture.Apply();
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
            }
        }
        
        renderTexture.sharedMaterial.SetTexture("_BaseMap", texture);
        renderTexture.transform.localScale = new Vector3(width, 1, height);
    }
}
