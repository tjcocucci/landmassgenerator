using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateMeshData(float[,] heightMap, float heightMultiplier, AnimationCurve animationCurve, int levelOfDetail) {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float offsetX = (width - 1) / -2f;
        float offsetZ = (height - 1) / 2f;

        int verticesPerLineX;
        int verticesPerLineZ;

        int meshIncrement = 1;
        if (levelOfDetail > 0 && (width - 1)%levelOfDetail == 0 && (height - 1)%levelOfDetail == 0) {
            meshIncrement = levelOfDetail * 2;
        }
        verticesPerLineX = (width - 1) / meshIncrement + 1;
        verticesPerLineZ = (height - 1) / meshIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLineX, verticesPerLineZ);

        int triangleIndex = 0;
        int vertexIndex = 0;
        for (int i = 0; i < height; i+=meshIncrement) {
            for (int j = 0; j < width; j+=meshIncrement) {
                meshData.vertices[vertexIndex] = new Vector3(i + offsetX, animationCurve.Evaluate(heightMap[i, j]) * -heightMultiplier, offsetZ - j);
                meshData.uvs[vertexIndex] = new Vector2(i/(float)width, j/(float)height);
                
                if (i < height - 1 && j < width - 1) {
                    meshData.triangles[triangleIndex] = vertexIndex;
                    meshData.triangles[triangleIndex + 1] = vertexIndex + verticesPerLineX + 1;
                    meshData.triangles[triangleIndex + 2] = vertexIndex + verticesPerLineX;
                    meshData.triangles[triangleIndex + 3] = vertexIndex + verticesPerLineX + 1;
                    meshData.triangles[triangleIndex + 4] = vertexIndex ;
                    meshData.triangles[triangleIndex + 5] = vertexIndex + 1;
                    triangleIndex += 6;
                }
                vertexIndex++;
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
