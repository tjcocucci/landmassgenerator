using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer renderTexture;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void SetTexture(int width, int height, Texture texture) {
        renderTexture.sharedMaterial.SetTexture("_BaseMap", texture);
        renderTexture.transform.localScale = new Vector3(width, 1, height);
    }

    public void SetMesh(int width, int height, Texture texture, Mesh mesh) {
        meshRenderer.sharedMaterial.SetTexture("_BaseMap", texture);
        meshFilter.sharedMesh = mesh;
        meshFilter.transform.localScale = new Vector3(width, -1, height);
    }

}
