using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveGenerator : MonoBehaviour
{
    [SerializeField] private int _width = 10;
    [SerializeField] private int _height = 10;
    [SerializeField] private int _bordersSize;
    [SerializeField] private int _offsetX;
    [SerializeField] private int _offsetY;
    [SerializeField] private float _noiseValue;
    [SerializeField, Range(0, 1)] private float _wallsThickness = 0.5f;

    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _wallsTilemap;
    [SerializeField] private Tile[] _groundTiles;
    [SerializeField] private TileBase _wallsTile;
    [SerializeField] private TileBase _waterTile;

    private int[,] map;
    float noiseValue;
    void Start()
    {
        _offsetX = Random.Range(0, 9999);
        _offsetY = Random.Range(0, 9999);

        map = new int[_width, _height];
        System.Random rand = new System.Random();

        _wallsTilemap.ClearAllTiles();

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                GenerateGround(x, y);
                GenerateWalls(x, y);
                RenderWalls(x, y);
            }
        }

        for(int x = -_bordersSize; x < _width + _bordersSize; x++)
        {
            for(int y = -_bordersSize; y < _height + _bordersSize; y++)
            {
                if(x < 0 || y < 0 || x >= _width || y >= _height)
                _wallsTilemap.SetTile(new Vector3Int(x, y, 0), _wallsTile);
            }
        }
    }

    private void RenderWalls(int x, int y)
    {
        if (map[x, y] == 1)
            _wallsTilemap.SetTile(new Vector3Int(x, y, 0), _wallsTile);
        if(noiseValue > 0 && noiseValue < 0.2f)
            _wallsTilemap.SetTile(new Vector3Int(x, y, 0), _waterTile);
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
}
