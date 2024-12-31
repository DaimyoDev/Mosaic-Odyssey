using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{

    [SerializeField] int width = 30;  // Width of the terrain
    [SerializeField] public int length = 30;  // Length of the terrain
    [SerializeField] public int height = 10;  // Maximum height of the terrain
    [SerializeField] public float scale = 20f;  // Scale for Perlin noise
    [SerializeField] public int cubeSize = 1; //Size of each cube
    [SerializeField] public float renderDistance = 50f;
    [SerializeField] public GameObject voxelPrefab;
    [SerializeField] public Camera mainCamera;
    [SerializeField] int chunkSize = 10;



    private Queue<GameObject> voxelPool = new Queue<GameObject>();
    private Queue<Vector2Int> chunkQueue = new Queue<Vector2Int>();
    private Dictionary<Vector2Int, List<Voxel>> chunkVoxels = new Dictionary<Vector2Int, List<Voxel>>();
    private int chunkCountX, chunkCountZ;

    private bool isGenerating = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chunkCountX = Mathf.CeilToInt((float)width / chunkSize);
        chunkCountZ = Mathf.CeilToInt((float)length / chunkSize);
        GenerateTerrain();
    }

    private void Update()
    {
        if (chunkQueue.Count > 0 && !isGenerating)
        {
            // Start generating the next chunk in the queue
            Vector2Int chunkCoords = chunkQueue.Dequeue();
            isGenerating = true;
            GenerateChunk(chunkCoords.x, chunkCoords.y);
        }
    }

    void GenerateTerrain()
    {
        for (int x = 0; x < chunkCountX; x++)
        {
            for (int z = 0; z < chunkCountZ; z++)
            {
                chunkQueue.Enqueue(new Vector2Int(x, z));
            }
        }
    }

    void GenerateChunk(int chunkX, int chunkZ)
    {
        Vector3 chunkOrigin = new Vector3(chunkX * chunkSize / 2, 0, chunkZ * chunkSize / 2);
        List<CombineInstance> combineInstances = new List<CombineInstance>();
        List<Voxel> voxels = new List<Voxel>();

        Mesh voxelMesh = voxelPrefab.GetComponent<MeshFilter>()?.sharedMesh;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                float rawHeight = Mathf.PerlinNoise((chunkX * chunkSize + x) * scale / width, (chunkZ * chunkSize + z) * scale / length)* height;
                float quantizedHeight = Mathf.Floor(rawHeight / cubeSize) * cubeSize;

                Vector3 position = new Vector3(chunkOrigin.x + x * cubeSize, quantizedHeight, chunkOrigin.z + z * cubeSize);

                Voxel voxel = new Voxel(position, quantizedHeight);

                voxels.Add(voxel);

                CombineInstance combineInstance = new CombineInstance
                {
                    mesh = voxelMesh,
                    transform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one)
                };
                combineInstances.Add(combineInstance);
            }
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        GameObject chunkObject = new GameObject("Chunk_" + chunkX + "_" + chunkZ);

        MeshFilter meshFilter = chunkObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = combinedMesh;
        meshRenderer.material = voxelPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        chunkObject.transform.position = chunkOrigin;

        chunkVoxels[new Vector2Int(chunkX, chunkZ)] = voxels;
        isGenerating = false;
    }
}

public class Voxel
{
    public Vector3 Position { get; private set; }
    public float Height { get; private set; }

    public Voxel(Vector3 position, float height)
    {
        Position = position;
        Height = height;
    }
}