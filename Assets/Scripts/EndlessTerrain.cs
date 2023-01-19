using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public Transform playerTransform;
    public float playerViewDistance = 50;
    public float chunckSize = 5;

    Dictionary<Vector2Int, TerrainChunk> terrainDict = new Dictionary<Vector2Int, TerrainChunk>();
    List<Vector2Int> coordsList = new List<Vector2Int> ();

    void Update() {
        DetectChuncksInSight();
    }

    Vector2Int PositionToCoords(Vector3 position) {
        int chunkCoordX = Mathf.FloorToInt((position.x - chunckSize/2) / chunckSize) + 1;
        int chunkCoordY = Mathf.FloorToInt((position.z - chunckSize/2) / chunckSize) + 1;
        return new Vector2Int(chunkCoordX, chunkCoordY);
    }

    Vector3 CoordToPosition(Vector2Int coords) {
        float positionX = coords.x * chunckSize;
        float positionY = coords.y * chunckSize;
        return new Vector3(positionX, 0, positionY);
    }

    void DetectChuncksInSight() {

        Vector2Int playerChunkCoords = PositionToCoords(playerTransform.position);

        for (int i = 0; i < coordsList.Count; i++) {
            if (terrainDict.ContainsKey(coordsList[i])) {
                terrainDict[coordsList[i]].SetVisible(false);
            }
        }

        int visibleChunks = Mathf.FloorToInt(playerViewDistance / chunckSize);
        for(int i=-visibleChunks+playerChunkCoords.x; i<=visibleChunks+playerChunkCoords.x; i++){
            for(int j=-visibleChunks+playerChunkCoords.y; j<=visibleChunks+playerChunkCoords.y; j++){
                Vector2Int chunkCoords = new Vector2Int(i, j);
                Vector3 chunkPosition = CoordToPosition(chunkCoords);
                if (terrainDict.ContainsKey(chunkCoords)) {
                    terrainDict[chunkCoords].UpdateTerrainChunk(playerTransform.position, playerViewDistance);
                } else {
                    terrainDict.Add(chunkCoords, new TerrainChunk(chunkCoords, chunkPosition, new Vector3(chunckSize/10, 0, chunckSize/10), transform));
                    terrainDict[chunkCoords].UpdateTerrainChunk(playerTransform.position, playerViewDistance);
                }
                coordsList.Add(chunkCoords);
            }
        }
    }
}

public class TerrainChunk {
    public Vector2Int chunkCoords;
    public Vector3 centerPosition;
    public GameObject plane;
    Bounds bounds;

    public TerrainChunk (Vector2Int coords, Vector3 position, Vector3 scale, Transform parent) {
        chunkCoords = coords;
        centerPosition = position;
        bounds = new Bounds(position, scale);
        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position = centerPosition;
        plane.transform.localScale = scale;
        plane.transform.parent = parent;
        plane.SetActive(false);
    }

    public void UpdateTerrainChunk(Vector3 point, float viewDistance) {
        bool visible = Mathf.Sqrt(bounds.SqrDistance(point)) <= viewDistance;
        SetVisible(visible);
    }
    public void SetVisible(bool visible) {
        plane.SetActive(visible);
    }
}