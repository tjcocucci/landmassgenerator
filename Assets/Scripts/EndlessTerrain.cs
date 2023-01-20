using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public Transform playerTransform;
    public Material mapMaterial;
    public float playerViewDistance = 700;
    public float chunckSize = 241;
    static MapGenerator mapGenerator;

    Dictionary<Vector2Int, TerrainChunk> terrainDict = new Dictionary<Vector2Int, TerrainChunk>();
    List<Vector2Int> coordsList = new List<Vector2Int> ();

    void Start() {
        mapGenerator = FindObjectOfType<MapGenerator>();
    }

    void Update() {
        DetectChuncksInSight();
    }

    Vector2Int PositionToCoords(Vector3 position) {
        int chunkCoordX = Mathf.FloorToInt((position.x - chunckSize/2) / chunckSize) + 1;
        int chunkCoordY = Mathf.FloorToInt((position.z - chunckSize/2) / chunckSize) + 1;
        return new Vector2Int(chunkCoordX, chunkCoordY);
    }

    Vector3 CoordToPosition(Vector2Int coords) {
        float positionX = coords.x * (chunckSize-1);
        float positionY = coords.y * (chunckSize-1);
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
                    terrainDict.Add(chunkCoords, new TerrainChunk(chunkCoords, chunkPosition, new Vector3(1, -1, 1), transform, mapMaterial));
                    terrainDict[chunkCoords].UpdateTerrainChunk(playerTransform.position, playerViewDistance);
                }
                coordsList.Add(chunkCoords);
            }
        }
    }

    public class TerrainChunk {
        public Vector2Int chunkCoords;
        public Vector3 centerPosition;
        public GameObject mesh;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        Bounds bounds;

        public TerrainChunk (Vector2Int coords, Vector3 position, Vector3 scale, Transform parent, Material mapMaterial) {
            chunkCoords = coords;
            centerPosition = position;
            bounds = new Bounds(position, scale);
            
            mesh = new GameObject("Terrain Mesh");
            meshFilter = mesh.AddComponent<MeshFilter> ();
            meshRenderer = mesh.AddComponent<MeshRenderer> ();

            meshRenderer.sharedMaterial = mapMaterial;
            meshFilter.transform.localScale = scale;
            mesh.transform.position = centerPosition;
            mesh.transform.parent = parent;
            mesh.SetActive(false);

            mapGenerator.RequestMapData(OnMapDataRecieved);
        }

        void OnMapDataRecieved(MapData mapData) {
            mapGenerator.RequestMeshData(mapData, OnMeshDataRecieved);
        }

        void OnMeshDataRecieved(MeshData meshData) {
            meshFilter.sharedMesh = meshData.CreateMesh();
        }


        public void UpdateTerrainChunk(Vector3 point, float viewDistance) {
            bool visible = Mathf.Sqrt(bounds.SqrDistance(point)) <= viewDistance;
            SetVisible(visible);
        }
        public void SetVisible(bool visible) {
            mesh.SetActive(visible);
        }
    }

}