using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{

    [SerializeField] int width;
    [SerializeField] public int length;
    [SerializeField] public int height;
    [SerializeField] public float scale;
    [SerializeField] public int cubeSize;
    //[SerializeField] public float renderDistance = 50f;
    [SerializeField] public GameObject voxelPrefab;
    //[SerializeField] public Camera mainCamera;
    [SerializeField] int chunkSize;



    private Queue<GameObject> voxelPool = new Queue<GameObject>();
    private Queue<Vector2Int> chunkQueue = new Queue<Vector2Int>();
    private Dictionary<Vector2Int, Chunk> chunkVoxels = new Dictionary<Vector2Int, Chunk>();
    private int chunkCountX, chunkCountZ;

    private bool isGenerating = false;
    private int seed;

    private enum Biome
    {
        Mountain,
        Hills,
        Plains
    }

    void Start()
    {
        seed = Random.Range(1, 100000);
        Random.InitState(seed);
        chunkCountX = Mathf.CeilToInt((float)width / chunkSize);
        chunkCountZ = Mathf.CeilToInt((float)length / chunkSize);
        GenerateTerrain();
    }

    private void Update()
    {
        if (chunkQueue.Count > 0 && !isGenerating)
        {
            Vector2Int chunkCoords = chunkQueue.Dequeue();
            isGenerating = true;
            GenerateChunk(chunkCoords.x, chunkCoords.y);
        }
    }

    float GeneratePerlinNoise(float x, float z, int octaves, float persistence, float frequency, float lacunarity, float amplitude, int chunkX, int chunkZ)
    {
        float total;

        float offsetX = seed * 0.3f;
        float offsetZ = seed * 0.3f;

        float plainsHeight = 0;
        float currentFrequency = frequency;
        float currentAmplitude = amplitude;

        for (int i = 0; i < octaves; i++)
        {
            plainsHeight += Mathf.PerlinNoise((x + chunkX * chunkSize + offsetX) * currentFrequency,
                                              (z + chunkZ * chunkSize + offsetZ) * currentFrequency) * currentAmplitude;
            currentFrequency *= lacunarity;
            currentAmplitude *= persistence;
        }

        total = plainsHeight;

        return total;
    }

    void GenerateTerrain()
    {
        for (int sum = 0; sum < chunkCountX + chunkCountZ - 1; sum++)
        {
            for (int x = 0; x <= sum; x++)
            {
                int z = sum - x;

                if (x < chunkCountX && z < chunkCountZ)
                {
                    chunkQueue.Enqueue(new Vector2Int(x, z));
                }
            }
        }
    }

    void GenerateChunk(int chunkX, int chunkZ)
    {
        Vector3 chunkOrigin = new Vector3(chunkX * chunkSize, 0, chunkZ * chunkSize);
        Chunk chunk = new Chunk
        {
            VoxelData = new byte[chunkSize, height, chunkSize],
            ChunkObject = new GameObject($"Chunk_{chunkX}_{chunkZ}")
        };

        float[,] noiseMap = new float[chunkSize, chunkSize];
        float[,] hillsMaskMap = new float[chunkSize, chunkSize];
        float[,] mountainMaskMap = new float[chunkSize, chunkSize];
        //int waterLevel = 70;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                // Compute plains noise
                noiseMap[x, z] = GeneratePerlinNoise(x, z, 4, 0.3f, 0.001f, 0.001f, 0.25f, chunkX, chunkZ);

                // Compute hills mask
                hillsMaskMap[x, z] = Mathf.PerlinNoise((x + chunkX * chunkSize + (seed * 0.3f)) * 0.0023f,
                                                       (z + chunkZ * chunkSize + (seed * 0.3f)) * 0.0023f);
                hillsMaskMap[x, z] = Mathf.Clamp01((hillsMaskMap[x, z] - 0.45f) / 0.3f);

                mountainMaskMap[x, z] = Mathf.PerlinNoise((x + chunkX * chunkSize + (seed * 0.5f)) * 0.002f,
                                                      (z + chunkZ * chunkSize + (seed * 0.5f)) * 0.002f);
                mountainMaskMap[x, z] = Mathf.Clamp01((mountainMaskMap[x, z] - 0.65f) / 0.2f);
            }
        }

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                float plainsNoise = noiseMap[x, z];
                float hillsNoise = GeneratePerlinNoise(x, z, 6, 0.7f, 0.0024f, 1.2f, 0.1f, chunkX, chunkZ);
                hillsNoise = Mathf.Pow(hillsNoise, 1);
                float mountainNoise = GeneratePerlinNoise(x, z, 8, 0.92f, 0.0025f, 1.3f, 0.25f, chunkX, chunkZ);
                mountainNoise = Mathf.Pow(mountainNoise, 2.5f);
                float blendedNoise = Mathf.Lerp(plainsNoise, plainsNoise + hillsNoise, hillsMaskMap[x, z]);
                blendedNoise = Mathf.Lerp(blendedNoise, blendedNoise + mountainNoise, mountainMaskMap[x, z]);

                float rawHeight = blendedNoise * height;
                rawHeight = Mathf.Clamp(rawHeight, 0, height);
                float terrainHeight = Mathf.Clamp(Mathf.FloorToInt(rawHeight / cubeSize), 0, height - 1);

                for (int y = 0; y < height; y++)
                {
                    if (y <= terrainHeight && y < height)
                    {
                        // Solid terrain
                        chunk.VoxelData[x, y, z] = 1;
                    }
                    //else if (y <= waterLevel)
                    //{
                        //chunk.VoxelData[x, y, z] = 2;
                    //}
                    else
                    {
                        // Air or empty space above the water level
                        chunk.VoxelData[x, y, z] = 0;
                    }  
                }
            }
        }

        chunk.ChunkObject.transform.position = chunkOrigin;
        chunk.ChunkObject.AddComponent<MeshFilter>();
        chunk.ChunkObject.AddComponent<MeshRenderer>().material = voxelPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        chunk.ChunkObject.AddComponent<MeshCollider>();

        chunkVoxels[new Vector2Int(chunkX, chunkZ)] = chunk;

        GenerateGreedyMesh(chunk);
        isGenerating = false;
    }
    void GenerateGreedyMesh(Chunk chunk)
    {
        byte[,,] voxelData = chunk.VoxelData;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Color> colors = new List<Color>();

        bool[,,] visited = new bool[chunkSize, height, chunkSize];

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (voxelData[x, y, z] == 1 && !visited[x, y, z])
                    {
                        int width = 1, depth = 1, maxHeight = 0;

                        while (y + maxHeight < height && voxelData[x, y + maxHeight, z] == 1 && !visited[x, y + maxHeight, z])
                        {
                            bool isValidHeight = true;
                            for (int dx = x; dx < x + width; dx++)
                            {
                                for (int dz = z; dz < z + depth; dz++)
                                {
                                    if (voxelData[dx, y + maxHeight, dz] != 1 || visited[dx, y + maxHeight, dz])
                                    {
                                        isValidHeight = false;
                                        break;
                                    }
                                }
                                if (!isValidHeight) break;
                            }
                            if (isValidHeight) maxHeight++;
                            else break;
                        }

                        while (x + width < chunkSize && voxelData[x + width, y, z] == 1 && !visited[x + width, y, z])
                        {
                            bool isValidWidth = true;
                            for (int dy = y; dy < y + maxHeight; dy++)
                            {
                                if (voxelData[x + width, dy, z] != 1 || visited[x + width, dy, z])
                                {
                                    isValidWidth = false;
                                    break;
                                }
                            }
                            if (isValidWidth) width++;
                            else break;
                        }

                        while (z + depth < chunkSize && voxelData[x, y, z + depth] == 1 && !visited[x, y, z + depth])
                        {
                            bool isValidDepth = true;
                            for (int dx = x; dx < x + width; dx++)
                            {
                                for (int dy = y; dy < y + maxHeight; dy++)
                                {
                                    if (voxelData[dx, dy, z + depth] != 1 || visited[dx, dy, z + depth])
                                    {
                                        isValidDepth = false;
                                        break;
                                    }
                                }
                                if (!isValidDepth) break;
                            }
                            if (isValidDepth) depth++;
                            else break;
                        }

                        GenerateFaces(x, y, z, width, depth, maxHeight, vertices, triangles, colors, voxelData);

                        for (int i = x; i < x + width; i++)
                        {
                            for (int j = y; j < y + maxHeight; j++)
                            {
                                for (int k = z; k < z + depth; k++)
                                {
                                    visited[i, j, k] = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        chunk.Mesh = mesh;
        chunk.ChunkObject.GetComponent<MeshFilter>().mesh = mesh;
        chunk.ChunkObject.GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    void GenerateFaces(int x, int y, int z, int width, int depth, int maxHeight, List<Vector3> vertices, List<int> triangles, List<Color> colors, byte[,,] voxelData)
    {
        Color green = new Color(0.1f, 0.3f, 0.1f, 1.0f);
        Color vertexColor = green;

        Color GetColorForHeight(int height)
        {
            if (height < 200) return green;
            if (height < 400) return Color.gray;
            return Color.white;
        }

        // **Bottom Face**: (Check if the voxel below is out of bounds or empty)
        if (y == 0 || voxelData[x, y, z] == 0)
        {
            Vector3 bottomBottomLeft = new Vector3(x, y, z);
            Vector3 bottomBottomRight = new Vector3(x + width, y, z);
            Vector3 bottomTopLeft = new Vector3(x, y, z + depth);
            Vector3 bottomTopRight = new Vector3(x + width, y, z + depth);

            int bottomStartIndex = vertices.Count;
            vertices.Add(bottomBottomLeft);
            vertices.Add(bottomBottomRight);
            vertices.Add(bottomTopLeft);
            vertices.Add(bottomTopRight);

            for (int i = 0; i < 4; i++)
            {
                colors.Add(GetColorForHeight(y));
            }

            // Add triangles for bottom face
            triangles.Add(bottomStartIndex);     // bottom-left
            triangles.Add(bottomStartIndex + 2); // top-left
            triangles.Add(bottomStartIndex + 1); // bottom-right
            triangles.Add(bottomStartIndex + 1); // bottom-right
            triangles.Add(bottomStartIndex + 2); // top-left
            triangles.Add(bottomStartIndex + 3); // top-right
        }

        // **Top Face**
        if (voxelData[x, y + maxHeight, z] == 0 || voxelData[x, y + maxHeight, z] == 1)
        {
            Vector3 topBottomLeft = new Vector3(x, y + maxHeight, z);
            Vector3 topBottomRight = new Vector3(x + width, y + maxHeight, z);
            Vector3 topTopLeft = new Vector3(x, y + maxHeight, z + depth);
            Vector3 topTopRight = new Vector3(x + width, y + maxHeight, z + depth);

            int topStartIndex = vertices.Count;
            vertices.Add(topBottomLeft);
            vertices.Add(topBottomRight);
            vertices.Add(topTopLeft);
            vertices.Add(topTopRight);

            for (int i = 0; i < 4; i++)
            {
                colors.Add(GetColorForHeight(y + maxHeight));
            }

            // Add triangles for top face
            triangles.Add(topStartIndex);     // bottom-left
            triangles.Add(topStartIndex + 2); // top-left
            triangles.Add(topStartIndex + 1); // bottom-right
            triangles.Add(topStartIndex + 1); // bottom-right
            triangles.Add(topStartIndex + 2); // top-left
            triangles.Add(topStartIndex + 3); // top-right
        }

        // Left Face (Check if the voxel on the left is empty or out of bounds)
        if (x == 0 || voxelData[x - 1, y, z] == 0 || voxelData[0, y, z] == 1)
        {
            Vector3 leftBottomLeft = new Vector3(x, y, z);
            Vector3 leftBottomRight = new Vector3(x, y, z + depth);
            Vector3 leftTopLeft = new Vector3(x, y + maxHeight, z);
            Vector3 leftTopRight = new Vector3(x, y + maxHeight, z + depth);

            int leftStartIndex = vertices.Count;
            vertices.Add(leftBottomLeft);
            vertices.Add(leftBottomRight);
            vertices.Add(leftTopLeft);
            vertices.Add(leftTopRight);

            colors.Add(GetColorForHeight(y));
            colors.Add(GetColorForHeight(y));
            colors.Add(GetColorForHeight(y + maxHeight));
            colors.Add(GetColorForHeight(y + maxHeight));

            // Add triangles for left face
            triangles.Add(leftStartIndex);     // bottom-left
            triangles.Add(leftStartIndex + 2); // top-left
            triangles.Add(leftStartIndex + 1); // bottom-right
            triangles.Add(leftStartIndex + 1); // bottom-right
            triangles.Add(leftStartIndex + 2); // top-left
            triangles.Add(leftStartIndex + 3); // top-right
        }

        //Right face (Check if the voxel on the right is empty or out of bounds)
        if (x + width == chunkSize || voxelData[x + width, y, z] == 0 || voxelData[x + width, y, z] == 1)
        {
            Vector3 rightBottomLeft = new Vector3(x + width, y, z);
            Vector3 rightBottomRight = new Vector3(x + width, y, z + depth);
            Vector3 rightTopLeft = new Vector3(x + width, y + maxHeight, z);
            Vector3 rightTopRight = new Vector3(x + width, y + maxHeight, z + depth);

            int rightStartIndex = vertices.Count;
            vertices.Add(rightBottomLeft);
            vertices.Add(rightBottomRight);
            vertices.Add(rightTopLeft);
            vertices.Add(rightTopRight);

            colors.Add(GetColorForHeight(y));
            colors.Add(GetColorForHeight(y));
            colors.Add(GetColorForHeight(y + maxHeight));
            colors.Add(GetColorForHeight(y + maxHeight));

            // Add triangles for right face
            triangles.Add(rightStartIndex);     // bottom-left
            triangles.Add(rightStartIndex + 2); // top-left
            triangles.Add(rightStartIndex + 1); // bottom-right
            triangles.Add(rightStartIndex + 1); // bottom-right
            triangles.Add(rightStartIndex + 2); // top-left
            triangles.Add(rightStartIndex + 3); // top-right
        }

        // Front Face (Check if the voxel in front is empty or out of bounds)
        if (z == 0 || voxelData[x, y, z - 1] == 0 || voxelData[x, y, z - 1] == 1)
        {
            Vector3 frontBottomLeft = new Vector3(x, y, z);
            Vector3 frontBottomRight = new Vector3(x + width, y, z);
            Vector3 frontTopLeft = new Vector3(x, y + maxHeight, z);
            Vector3 frontTopRight = new Vector3(x + width, y + maxHeight, z);

            int frontStartIndex = vertices.Count;
            vertices.Add(frontBottomLeft);
            vertices.Add(frontBottomRight);
            vertices.Add(frontTopLeft);
            vertices.Add(frontTopRight);

            colors.Add(GetColorForHeight(y));
            colors.Add(GetColorForHeight(y));
            colors.Add(GetColorForHeight(y + maxHeight));
            colors.Add(GetColorForHeight(y + maxHeight));

            // Add triangles for front face
            triangles.Add(frontStartIndex);     // bottom-left
            triangles.Add(frontStartIndex + 2); // top-left
            triangles.Add(frontStartIndex + 1); // bottom-right
            triangles.Add(frontStartIndex + 1); // bottom-right
            triangles.Add(frontStartIndex + 2); // top-left
            triangles.Add(frontStartIndex + 3); // top-right
        }

        // Back Face (Check if the voxel in the back is empty or out of bounds)
        if (z + depth == chunkSize || voxelData[x, y, z + depth] == 0 || voxelData[x, y, 0] == 1 || voxelData[x, y, z + depth] == 1)
        {
            Vector3 backBottomLeft = new Vector3(x, y, z + depth);
            Vector3 backBottomRight = new Vector3(x + width, y, z + depth);
            Vector3 backTopLeft = new Vector3(x, y + maxHeight, z + depth);
            Vector3 backTopRight = new Vector3(x + width, y + maxHeight, z + depth);

            int backStartIndex = vertices.Count;
            vertices.Add(backBottomLeft);
            vertices.Add(backBottomRight);
            vertices.Add(backTopLeft);
            vertices.Add(backTopRight);

            colors.Add(GetColorForHeight(y));
            colors.Add(GetColorForHeight(y));
            colors.Add(GetColorForHeight(y + maxHeight));
            colors.Add(GetColorForHeight(y + maxHeight));

            // Add triangles for back face
            triangles.Add(backStartIndex);     // bottom-left
            triangles.Add(backStartIndex + 2); // top-left
            triangles.Add(backStartIndex + 1); // bottom-right
            triangles.Add(backStartIndex + 1); // bottom-right
            triangles.Add(backStartIndex + 2); // top-left
            triangles.Add(backStartIndex + 3); // top-right
        }
    }
}


class Voxel
{
    public Vector3 Position { get; private set; }
    public float Height { get; private set; }

    public Voxel(Vector3 position, float height)
    {
        Position = position;
        Height = height;
    }
}

class Chunk
{
    public byte[,,] VoxelData;
    public GameObject ChunkObject;
    public Mesh Mesh;
}