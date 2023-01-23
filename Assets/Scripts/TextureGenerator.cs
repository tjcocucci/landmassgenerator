using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap (Color[] colorMap, int width, int height) {
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(colorMap);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }
}
