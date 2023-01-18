using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateMeshData(float[,] heightMap, float heightMultiplier, AnimationCurve animationCurve) {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float offsetX = (width - 1) / -2f;
        float offsetZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);

        int triangleIndex = 0;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                int index = j * width + i;
                meshData.vertices[index] = new Vector3(i + offsetX, animationCurve.Evaluate(heightMap[i, j]) * heightMultiplier, offsetZ - j);
                
                if (i < width - 1 && j < height - 1) {
                    meshData.triangles[triangleIndex] = index;
                    meshData.triangles[triangleIndex + 1] = index + width + 1;
                    meshData.triangles[triangleIndex + 2] = index + width;
                    meshData.triangles[triangleIndex + 3] = index + width + 1;
                    meshData.triangles[triangleIndex + 4] = index ;
                    meshData.triangles[triangleIndex + 5] = index + 1;
                    triangleIndex += 6;
                }

                meshData.uvs[index] = new Vector2(i/(float)width, j/(float)height);
            }
        }
        return meshData;
    }

    public static Mesh MeshDataToMesh(MeshData meshData) {
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.vertices;
        mesh.triangles = meshData.triangles;
        mesh.uv = meshData.uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}

public class MeshData {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    public MeshData(int width, int height) {
        vertices = new Vector3[width * height];
        triangles = new int[(width-1) * (height-1) * 6];
        uvs = new Vector2[width * height];
    }
}
