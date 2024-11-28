using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private static GridManager instance;
    public static GridManager Instance { get { return instance; } }
    //public static GridManager Instance { get; private set; }

    [SerializeField] private int width, height;
    [SerializeField] private Tile floorPrefab;
    [SerializeField] private Tile doorPrefab;
    [SerializeField] private Tile stationPrefab;
    [SerializeField] private GameObject pilCrew;
    [SerializeField] private GameObject engCrew;
    [SerializeField] private GameObject docCrew;
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private Sprite[] floorSprites;
    [SerializeField] private Sprite breachSprite;

    [SerializeField] private List<Vector2Int> nullTiles;
    [SerializeField] private List<Vector2Int> hDoorTiles;
    [SerializeField] private List<Vector2Int> vDoorTiles;
    [SerializeField] private List<Vector2Int> floorTiles;

    public List<Vector2Int> uStationTiles;
    public List<Vector2Int> dStationTiles;
    public List<Vector2Int> lStationTiles;
    public List<Vector2Int> rStationTiles;

    public List<BaseCrew> crew = new List<BaseCrew>();
    public Dictionary<Vector2, Tile> tiles;

    public float totalPressure = 0f; 
    public float maxPressure = 100f;

    private Transform pilSpawn;
    private Transform engSpawn;
    private Transform docSpawn;
    private Transform enemySpawn;

    private List<Vector2Int> breachTiles = new List<Vector2Int>();

    void Start()
    {
        instance = this;
        GenerateGrid();

        pilSpawn = tiles[new Vector2(7, 2)].transform;
        Instantiate(pilCrew, pilSpawn);
        engSpawn = tiles[new Vector2(2, 6)].transform;
        Instantiate(engCrew, engSpawn);
        docSpawn = tiles[new Vector2(11, 7)].transform;
        Instantiate(docCrew, docSpawn);

        StartCoroutine(BreachRoutine());

        UpdatePressure();
    }

    IEnumerator BreachRoutine()
    {
        yield return new WaitForSeconds(5f);

        while (true)
        {
            GenerateBreach();
            yield return new WaitForSeconds(5f);
        }
    }

    void GenerateGrid()
    {
        Vector3 gridOffset = new Vector3(30, 10, 0); // Add 30 to x and 10 to y

        tiles = new Dictionary<Vector2, Tile>();
        floorTiles = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tileLocation = new Vector2Int(x, y);
                Tile spawnedTile;

                if (nullTiles.Contains(tileLocation))
                {
                    spawnedTile = null;
                }
                else if (vDoorTiles.Contains(tileLocation))
                {
                    spawnedTile = Instantiate(doorPrefab, gridOffset + new Vector3(x, y, 0), Quaternion.Euler(0f, 0f, 90f));
                }
                else if (hDoorTiles.Contains(tileLocation))
                {
                    spawnedTile = Instantiate(doorPrefab, gridOffset + new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (uStationTiles.Contains(tileLocation))
                {
                    spawnedTile = Instantiate(stationPrefab, gridOffset + new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (dStationTiles.Contains(tileLocation))
                {
                    spawnedTile = Instantiate(stationPrefab, gridOffset + new Vector3(x, y, 0), Quaternion.Euler(0f, 0f, 180f));
                }
                else if (lStationTiles.Contains(tileLocation))
                {
                    spawnedTile = Instantiate(stationPrefab, gridOffset + new Vector3(x, y, 0), Quaternion.Euler(0f, 0f, 90f));
                }
                else if (rStationTiles.Contains(tileLocation))
                {
                    spawnedTile = Instantiate(stationPrefab, gridOffset + new Vector3(x, y, 0), Quaternion.Euler(0f, 0f, 270f));
                }
                else
                {
                    Sprite randomSprite = floorSprites[Random.Range(0, floorSprites.Length)];
                    spawnedTile = Instantiate(floorPrefab, gridOffset + new Vector3(x, y, 0), Quaternion.identity);
                    spawnedTile.GetComponent<SpriteRenderer>().sprite = randomSprite;
                    floorTiles.Add(tileLocation);
                }

                if (spawnedTile != null)
                {
                    spawnedTile.name = $"Tile {x} {y}";
                    tiles[new Vector2(x, y)] = spawnedTile;
                    spawnedTile.gridLocation = tileLocation;
                }
            }
        }
    }


    public void Patched(Tile tile)
    {
        tile.GetComponent<SpriteRenderer>().sprite = floorSprites[Random.Range(0, floorSprites.Length)];
        tile.tag = "Floor";
        UpdatePressure();
    }

    void GenerateBreach()
    {
        if (floorTiles.Count > 0)
        {
            Vector2Int breachLocation = floorTiles[Random.Range(0, floorTiles.Count)];

            if (tiles.TryGetValue(breachLocation, out Tile breachTile))
            {
                breachTile.GetComponent<SpriteRenderer>().sprite = breachSprite;
                breachTile.tag = "Breach";
                breachTiles.Add(breachLocation);  
                StartCoroutine(IncreasePressure(breachLocation));

                UpdatePressure();
            }
        }
    }

    IEnumerator IncreasePressure(Vector2Int breachLocation)
    {
        while (tiles.TryGetValue(breachLocation, out Tile breachTile) && breachTile.tag == "Breach")
        {
            PressureSpread(breachLocation);
            UpdatePressure();
            yield return null;  
        }
    }


    void PressureSpread(Vector2Int breachLocation)
    {
        Queue<Vector2Int> tilesToCheck = new Queue<Vector2Int>();

        List<Vector2Int> affectedTiles = new List<Vector2Int>();
        affectedTiles.Add(breachLocation);
        tilesToCheck.Enqueue(breachLocation);

        while (tilesToCheck.Count > 0)
        {
            Vector2Int currentTile = tilesToCheck.Dequeue();

            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (var direction in directions)
            {
                Vector2Int neighborLocation = currentTile + direction;

                if (tiles.TryGetValue(neighborLocation, out Tile neighborTile) && neighborTile.open)
                {
                    if (!affectedTiles.Contains(neighborLocation))
                    {
                        affectedTiles.Add(neighborLocation);
                        tilesToCheck.Enqueue(neighborLocation);
                    }
                }
            }
        }

        foreach(Vector2Int tileLocation in affectedTiles)
        {
            if (tiles.TryGetValue(tileLocation, out Tile tile))
            {
                tile.IncreasePressure(Time.deltaTime / affectedTiles.Count);
            }
        }
    }

    public void UpdatePressure()
    {
        totalPressure = 0f;

        foreach (var tileEntry in tiles)
        {
            Tile tile = tileEntry.Value;

            if (tile.tag == "Breach" || tile.tag == "Floor")
            {
                totalPressure += tile.currentPressure;
            }
        }
        float maxPressureHere = 1000;
        totalPressure = Mathf.Clamp(totalPressure, 0, maxPressureHere);
        //Debug.Log($"Updated Total Pressure: {totalPressure}");
        PressureBarController.Instance?.UpdateBar(totalPressure / maxPressureHere);

        //Debug.Log($"Updated Total Pressure: {totalPressure}/{maxPressure}");
    }





}
