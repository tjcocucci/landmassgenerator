using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{

    public Level[] levels;

    float previousMinHeight;
    float previousMaxHeight;

    public void ApplyToMaterial (Material material) {
        material.SetInt("countColorLevels", levels.Length);
        material.SetColorArray("colors", levels.Select(x => x.color).ToArray());
        material.SetFloatArray("baseHeights", levels.Select(x => x.baseHeight).ToArray());
        UpdateHeights(material, previousMinHeight, previousMaxHeight);
    }

    public void UpdateHeights(Material material, float minHeight, float maxHeight) {
        previousMinHeight = minHeight;
        previousMaxHeight = maxHeight;

        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }

    [System.Serializable]
    public class Level {
        public Color color;
        [Range(0, 1)]
        public float baseHeight;
    }
}

