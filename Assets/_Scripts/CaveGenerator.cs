using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
using JetBrains.Annotations;

public class CaveGenerator : MonoBehaviour
{
    [SerializeField] private int _width = 10;
    [SerializeField] private int _height = 10;
    [SerializeField] private int _bordersSize;
    [SerializeField] private int _offsetX;
    [SerializeField] private int _offsetY;
    [SerializeField] private float _noiseValue;
    [SerializeField, Range(0, 1)] private float _wallsThickness = 0.5f;
    [SerializeField, Range(0, 1)] private float _waterLevel = 0.2f;

    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _wallsTilemap;
    [SerializeField] private Tilemap _waterTilemap;
    [SerializeField] private Tile[] _groundTiles;
    [SerializeField] private TileBase _wallsTile;
    [SerializeField] private TileBase _waterTile;

    [SerializeField] private SpriteRenderer _fakeLights;

    private int[,] map;
    private int waterTilesAmount = 0;
    private float noiseValue;

    [SerializeField] private GameObject waterLightPrefab;
    [SerializeField] private GameObject waterParticlesPrefab;
    [SerializeField] private GameObject housePrefab;

    private bool[,] visited;

    void Start()
    {
        _offsetX = Random.Range(0, 9999);
        _offsetY = Random.Range(0, 9999);

        map = new int[_width, _height];
        visited = new bool[_width, _height];

        _wallsTilemap.ClearAllTiles();
        _waterTilemap.ClearAllTiles();
        _groundTilemap.ClearAllTiles();

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                GenerateGround(x, y);
                GenerateWalls(x, y);
                RenderMap(x, y);
            }
        }

        CreateBorders();
        SpawnWaterLights();
        PlaceBuildings();
    }

    void SpawnWaterLights()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_waterTilemap.GetTile(new Vector3Int(x, y, 0)) != null && !visited[x, y])
                {
                    List<Vector2> lakeTiles = new List<Vector2>();
                    FloodFillWater(x, y, lakeTiles);

                    Vector2 center = Vector2.zero;
                    foreach (var p in lakeTiles)
                        center += p;

                    center /= lakeTiles.Count;

                    GameObject light = Instantiate(waterLightPrefab, center, Quaternion.identity);

                    var l = light.GetComponent<Light2D>();

                    float lakeSize = lakeTiles.Count;

                    float radius = Mathf.Sqrt(lakeSize);

                    l.pointLightOuterRadius = radius;


                }
            }
        }
    }

    void FloodFillWater(int startX, int startY, List<Vector2> lake)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        visited[startX, startY] = true;

        while (queue.Count > 0)
        {
            Vector2Int p = queue.Dequeue();
            lake.Add(new Vector2(p.x + 0.5f, p.y + 0.5f));

            foreach (var dir in new Vector2Int[] {
                new Vector2Int(1,0), new Vector2Int(-1,0),
                new Vector2Int(0,1), new Vector2Int(0,-1)
            })
            {
                int nx = p.x + dir.x;
                int ny = p.y + dir.y;

                if (nx >= 0 && ny >= 0 && nx < _width && ny < _height)
                {
                    if (!visited[nx, ny] && _waterTilemap.GetTile(new Vector3Int(nx, ny, 0)) != null)
                    {
                        visited[nx, ny] = true;
                        queue.Enqueue(new Vector2Int(nx, ny));
                    }
                }
            }
        }
    }

    private void CreateBorders()
    {
        for (int x = -_bordersSize; x < _width + _bordersSize; x++)
        {
            for (int y = -_bordersSize; y < _height + _bordersSize; y++)
            {
                if (x < 0 || y < 0 || x >= _width || y >= _height)
                    _wallsTilemap.SetTile(new Vector3Int(x, y, 0), _wallsTile);
            }
        }
    }

    private void RenderMap(int x, int y)
    {
        if (map[x, y] == 1)
            _wallsTilemap.SetTile(new Vector3Int(x, y, 0), _wallsTile);
        GenerateWater(x, y);
    }

    private void GenerateWater(int x, int y)
    {
        if (noiseValue > 0 && noiseValue < _waterLevel)
        {
            _waterTilemap.SetTile(new Vector3Int(x, y, 0), _waterTile);
            waterTilesAmount++;

            if (waterTilesAmount % 15 == 0)
            { 
                GameObject.Instantiate(waterParticlesPrefab, new Vector3(x, y, 0), Quaternion.identity);
            }
        }

    }

    private void GenerateWalls(int x, int y)
    {
        float xCoord = (float)x / _width * _noiseValue + _offsetX;
        float yCoord = (float)y / _height * _noiseValue + _offsetY;

        noiseValue = Mathf.PerlinNoise(xCoord, yCoord);

        map[x, y] = (noiseValue > _wallsThickness) ? 1 : 0;
    }

    private void GenerateGround(int x, int y)
    {
        _groundTilemap.SetTile(new Vector3Int(x, y, 0), _groundTiles[Random.Range(0, _groundTiles.Length)]);
    }

    private void PlaceBuildings()
    {
        int buildingsAmount = 3;
        Vector3Int randPos = new Vector3Int(0, 0, 0);
        bool isAreaFree = false;
        for(int i = 0; i < buildingsAmount; i++)
        {
            int limit = 10;
            while (isAreaFree != true)
            {
                randPos = new Vector3Int(Random.Range(15,_width-15), Random.Range(12, _height - 12));
                isAreaFree = AreaIsFree(randPos, 15, 12);
                limit--;
                if (limit <= 0) break;
            }

            isAreaFree = false;

            ClearArea(randPos, 15, 12);
            GameObject.Instantiate(housePrefab, randPos, Quaternion.identity);
        }
        
    }

    bool AreaIsFree(Vector3Int pos, int width, int height)
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                Vector3Int check = new Vector3Int(pos.x + x, pos.y + y, 0);

                if (_waterTilemap.GetTile(check) != null) return false;
            }
        return true;
    }
    void ClearArea(Vector3Int pos, int width, int height)
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                Vector3Int check = new Vector3Int(pos.x + x, pos.y + y, 0);

                if (_wallsTilemap.GetTile(check) != null)
                {
                    _wallsTilemap.SetTile(check, null);
                }
            }
    }
}
