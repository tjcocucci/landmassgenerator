using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
 
    public Transform playerTransform;
    public Material mapMaterial;
    public LODInfo[] detailLevels;
    public float viewerDistanceToUpgradeChunks = 25f;
    float sqrViewerDistanceToUpgradeChunks;
    int chunkSize;
    static MapGenerator mapGenerator;
    static float playerViewDistance;
    Vector2 previousPosition;
    public static Vector2 viewerPosition;

    Dictionary<Vector2Int, TerrainChunk> terrainDict = new Dictionary<Vector2Int, TerrainChunk>();
    List<Vector2Int> coordsList = new List<Vector2Int> ();

    void Awake() {
        mapGenerator = FindObjectOfType<MapGenerator>();
    }

    void Start() {
        chunkSize = MapGenerator.chunkSize - 1;
        previousPosition = new Vector2(playerTransform.position.x, playerTransform.position.z);
        sqrViewerDistanceToUpgradeChunks = Mathf.Pow(viewerDistanceToUpgradeChunks, 2);
        playerViewDistance = detailLevels[detailLevels.Length - 1].distanceThershold;
        DetectChuncksInSight();
    }

    void Update() {
        viewerPosition = new Vector2(playerTransform.position.x, playerTransform.position.z);
        if (Vector2.SqrMagnitude(viewerPosition - previousPosition) > sqrViewerDistanceToUpgradeChunks) {
            previousPosition = viewerPosition;
            DetectChuncksInSight();
        }
    }

    Vector2Int PositionToCoords(Vector3 position) {
        int chunkCoordX = Mathf.FloorToInt((position.x - chunkSize/2) / chunkSize) + 1;
        int chunkCoordY = Mathf.FloorToInt((position.z - chunkSize/2) / chunkSize) + 1;
        // int chunkCoordX = Mathf.RoundToInt(position.x / chunkSize);
        // int chunkCoordY = Mathf.RoundToInt(position.z / chunkSize);
        return new Vector2Int(chunkCoordX, chunkCoordY);
    }

    Vector3 CoordToPosition(Vector2Int coords) {
        float positionX = coords.x * (chunkSize);
        float positionY = coords.y * (chunkSize);
        return new Vector3(positionX, 0, positionY);
    }

    public void DetectChuncksInSight() {

        Vector2Int playerChunkCoords = PositionToCoords(playerTransform.position);

        for (int i = 0; i < coordsList.Count; i++) {
            if (terrainDict.ContainsKey(coordsList[i])) {
                terrainDict[coordsList[i]].SetVisible(false);
            }
        }

        int visibleChunks = Mathf.FloorToInt(playerViewDistance / chunkSize);
        for(int i=-visibleChunks+playerChunkCoords.x; i<=visibleChunks+playerChunkCoords.x; i++){
            for(int j=-visibleChunks+playerChunkCoords.y; j<=visibleChunks+playerChunkCoords.y; j++){
                Vector2Int chunkCoords = new Vector2Int(i, j);
                Vector3 chunkPosition = CoordToPosition(chunkCoords);
                if (terrainDict.ContainsKey(chunkCoords)) {
                    terrainDict[chunkCoords].UpdateTerrainChunk();
                } else {
                    terrainDict.Add(chunkCoords, new TerrainChunk(chunkCoords, chunkSize, detailLevels, transform, mapMaterial));
                    terrainDict[chunkCoords].UpdateTerrainChunk();
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
        public MeshCollider meshCollider;
        LODMesh[] lodMeshes;
        Bounds bounds;
        LODInfo[] detailLevels;
        MapData mapData;
        bool mapDataRecieved;
        int previousLODIndex = -1;

        public TerrainChunk (Vector2Int coords, int size, LODInfo[] detailLevels, Transform parent, Material mapMaterial) {
            this.detailLevels = detailLevels;
            chunkCoords = coords;
            centerPosition = new Vector3(coords.x, 0, coords.y) * size;
            bounds = new Bounds(new Vector2(centerPosition.x, centerPosition.z), Vector2.one * size);
            
            mesh = new GameObject("Terrain Mesh");
            meshFilter = mesh.AddComponent<MeshFilter> ();
            meshRenderer = mesh.AddComponent<MeshRenderer> ();
            meshCollider = mesh.AddComponent<MeshCollider> ();
            meshRenderer.sharedMaterial = mapMaterial;

            mesh.transform.position = centerPosition;
            mesh.transform.localScale = new Vector3(1, -1, 1);
            mesh.transform.parent = parent;
            mesh.SetActive(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < lodMeshes.Length; i++) {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
            }

            mapGenerator.RequestMapData(new Vector2(centerPosition.x, centerPosition.z), OnMapDataRecieved);
        }

        void OnMapDataRecieved(MapData mapData) {
            this.mapData = mapData;
            mapDataRecieved = true;
            int width = mapData.noiseMap.GetLength(0);
            int height = mapData.noiseMap.GetLength(1);
            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, width, height);
            meshRenderer.material.SetTexture("_BaseMap", texture);
            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk() {
            if (mapDataRecieved) {
                float distanceFromPoint = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = distanceFromPoint <= playerViewDistance;
                if (visible) {
                    int lodIndex = 0;
                    for (int i = 0; i < detailLevels.Length - 1; i++) {
                        if (distanceFromPoint >= detailLevels[i].distanceThershold) {
                            lodIndex++;
                        } else {
                            break;
                        }
                    }

                    if (previousLODIndex != lodIndex) {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh) {
                            previousLODIndex = lodIndex;
                            meshFilter.sharedMesh = lodMesh.mesh;
                            meshCollider.sharedMesh = lodMesh.mesh;
                        } else if (!lodMesh.hasRequestedData) {
                            lodMesh.RequestMesh(mapData);
                        }
                    }

                }

                SetVisible(visible);
            }
        }

        public void SetVisible(bool visible) {
            mesh.SetActive(visible);
        }
    }

    class LODMesh {
        public Mesh mesh;
        public bool hasRequestedData;
        public bool hasMesh;
        int lod;
        System.Action updateCallback;

        public LODMesh (int lod, System.Action updateCallback) {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        public void OnMeshDataRecieved(MeshData meshData) {
            mesh = meshData.CreateMesh();
            hasMesh = true;
            updateCallback();
        }

        public void RequestMesh(MapData mapData) {
            hasRequestedData = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataRecieved);
        } 
        
    }

    [System.Serializable]
    public struct LODInfo {
        public int lod;
        public float distanceThershold;
    }


}