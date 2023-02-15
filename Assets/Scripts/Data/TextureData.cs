using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    [Range(0, 1)]
    public float textureStrength;
    public Level[] levels;

    float previousMinHeight;
    float previousMaxHeight;
    int textureSize = 512;
    TextureFormat textureFormat = TextureFormat.RGB565;

    public void ApplyToMaterial (Material material) {
        material.SetFloat("textureStrength", textureStrength);
        material.SetInt("countColorLevels", levels.Length);
        material.SetColorArray("colors", levels.Select(x => x.color).ToArray());
        material.SetFloatArray("baseHeights", levels.Select(x => x.baseHeight).ToArray());
        material.SetFloatArray("blends", levels.Select(x => x.blend).ToArray());
        material.SetFloatArray("textureScales", levels.Select(x => x.textureScale).ToArray());
        Texture2DArray textureArray = GenerateTextureArray(levels.Select(x => x.texture).ToArray());
        material.SetTexture("textures", textureArray);
        UpdateHeights(material, previousMinHeight, previousMaxHeight);
    }

    Texture2DArray GenerateTextureArray (Texture2D[] textures) {
        Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);
        for (int i = 0; i < textures.Length; i++) {
           textureArray.SetPixels(textures[i].GetPixels(), i);
        }
        textureArray.Apply();
        return textureArray;
    }

    public void UpdateHeights(Material material, float minHeight, float maxHeight) {
        previousMinHeight = minHeight;
        previousMaxHeight = maxHeight;

        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }

    [System.Serializable]
    public class Level {
        public Texture2D texture;
        public float textureScale;
        public Color color;
        [Range(0, 1)]
        public float baseHeight;
        [Range(0, 1)]
        public float blend;
    }
}

