using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateMeshData(float[,] heightMap, float heightMultiplier, AnimationCurve _animationCurve, int levelOfDetail) {
        AnimationCurve animationCurve = new AnimationCurve(_animationCurve.keys);
        int borderedSize = heightMap.GetLength(0);
        int meshIncrement = 1;
        if (levelOfDetail > 0 && (borderedSize - 1)%levelOfDetail == 0) {
            meshIncrement = levelOfDetail * 2;
        }
        int meshSize = borderedSize - 2 * meshIncrement;
        int meshSizeUnsimplified = borderedSize - 2;
        float offsetX = (meshSizeUnsimplified - 1) / -2f;
        float offsetZ = (meshSizeUnsimplified - 1) / 2f;

        int verticesPerLine;
        verticesPerLine = (meshSize - 1) / meshIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine);

        int borderIndex = -1;
        int meshIndex = 0;
        int[,] vertexIndices = new int[borderedSize, borderedSize];
        for (int i = 0; i < borderedSize; i+=meshIncrement) {
            for (int j = 0; j < borderedSize; j+=meshIncrement) {
                bool isBorder = i == 0 || i == (borderedSize-1) || j == 0 || j == (borderedSize-1);
                if (isBorder) {
                    vertexIndices[i, j] = borderIndex;
                    borderIndex--;
                } else {
                        vertexIndices[i, j] = meshIndex;
                        meshIndex++;
                }
            }
        }

        for (int i = 0; i < borderedSize; i+=meshIncrement) {
            for (int j = 0; j < borderedSize; j+=meshIncrement) {

                Vector2 percent = new Vector2((i - meshIncrement)/(float)meshSize, (j - meshIncrement)/(float)meshSize);
                float height = animationCurve.Evaluate(heightMap[i, j]) * -heightMultiplier;
                Vector3 vertexPosition = new Vector3(meshSizeUnsimplified * percent.x + offsetX, height, offsetZ - meshSizeUnsimplified * percent.y);
                meshData.AddVertex(vertexPosition, percent, vertexIndices[i, j]);

                if (i < borderedSize - 1 && j < borderedSize - 1) {
                    int a = vertexIndices[i, j];
                    int b = vertexIndices[i, j + meshIncrement];
                    int c = vertexIndices[i + meshIncrement, j];
                    int d = vertexIndices[i + meshIncrement, j + meshIncrement];
                    meshData.AddTriangle(a, d, c);
                    meshData.AddTriangle(d, a, b);
                }
            }
        }
        return meshData;
    }

}

public class MeshData {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector3[] borderVertices;
    public int[] borderTriangles;
    public Vector2[] uvs;

    int triangleIndex;
    int borderTriangleIndex;

    public MeshData(int size) {
        vertices = new Vector3[size * size];
        triangles = new int[(size-1) * (size-1) * 6];
        borderVertices = new Vector3[(size + 1) * 4];
        borderTriangles = new int[size * 24];
        uvs = new Vector2[size * size];
        triangleIndex = 0;
        borderTriangleIndex = 0;
    }

    public void AddTriangle(int a, int b, int c) {
        if (a < 0 || b < 0 || c < 0) {
            borderTriangles[borderTriangleIndex] = a;
            borderTriangles[borderTriangleIndex + 1] = b;
            borderTriangles[borderTriangleIndex + 2] = c;
            borderTriangleIndex += 3;
        } else {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }
    }

    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex) {
        if (vertexIndex < 0) {
            borderVertices[-vertexIndex-1] = vertexPosition;
        } else {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = uv;
        }
    }

    Vector3[] CalculateNomals () {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int trianglesCount = triangles.Length / 3;
        for (int i = 0; i < trianglesCount; i++)  {
            int vertexIndexA = triangles[i * 3];
            int vertexIndexB = triangles[i * 3 + 1];
            int vertexIndexC = triangles[i * 3 + 2];

            Vector3 normal = CalculateNormalFromVertexIndices(vertexIndexA, vertexIndexB, vertexIndexC);

            vertexNormals[vertexIndexA] += normal;
            vertexNormals[vertexIndexB] += normal;
            vertexNormals[vertexIndexC] += normal;
        }

        int borderTrianglesCount = borderTriangles.Length / 3;
        for (int i = 0; i < borderTrianglesCount; i++)  {
            int vertexIndexA = borderTriangles[i * 3];
            int vertexIndexB = borderTriangles[i * 3 + 1];
            int vertexIndexC = borderTriangles[i * 3 + 2];

            Vector3 normal = CalculateNormalFromVertexIndices(vertexIndexA, vertexIndexB, vertexIndexC);

            if (vertexIndexA >= 0) {
                vertexNormals[vertexIndexA] += normal;
            }
            if (vertexIndexB >= 0) {
                vertexNormals[vertexIndexB] += normal;
            }
            if (vertexIndexC >= 0) {
                vertexNormals[vertexIndexC] += normal;
            }
        }
        for (int i = 0; i < vertices.Length; i++)  {
            vertexNormals[i] = vertexNormals[i].normalized;
        }
        return vertexNormals;
    }
    

    Vector3 CalculateNormalFromVertexIndices (int a, int b, int c) {
        Vector3 vertexA = a < 0 ? borderVertices[-a-1] : vertices[a];
        Vector3 vertexB = b < 0 ? borderVertices[-b-1] : vertices[b];
        Vector3 vertexC = c < 0 ? borderVertices[-c-1] : vertices[c];

        Vector3 sideAB = vertexB - vertexA;
        Vector3 sideAC = vertexC - vertexA;

        return Vector3.Cross(sideAB, sideAC);
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = CalculateNomals();
        // mesh.RecalculateNormals();
        return mesh;
    }

}
