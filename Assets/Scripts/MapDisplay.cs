using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer renderTexture;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void SetTexture(Texture texture) {
        renderTexture.sharedMaterial.SetTexture("_BaseMap", texture);
        renderTexture.transform.localScale = new Vector3(1, -1, 1);
    }

    public void SetMesh(Texture texture, Mesh mesh) {
        meshRenderer.sharedMaterial.SetTexture("_BaseMap", texture);
        meshFilter.sharedMesh = mesh;
        meshFilter.transform.localScale = new Vector3(1, -1, 1);
        meshRenderer.transform.localScale = new Vector3(1, -1, 1);
    }

}
