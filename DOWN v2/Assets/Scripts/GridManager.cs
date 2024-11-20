using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{
   private static GridManager instance;
   public static GridManager Instance { get { return instance; } }


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


   private Transform pilSpawn;
   private Transform engSpawn;
   private Transform docSpawn;


   private Transform enemySpawn;


   public bool moveable;


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
   }


   IEnumerator BreachRoutine()
   {
       while (true)
       {
           GenerateBreach();
           yield return new WaitForSeconds(5f);
       }
   }


   IEnumerator SpawnEnemies(float interval)
   {
       while (true)
       {
           yield return new WaitForSeconds(interval);


           List<Tile> unoccupiedTiles = new List<Tile>();
           foreach (var tile in tiles.Values)
           {
               if (!tile.isOccupied)
               {
                   unoccupiedTiles.Add(tile);
               }
           }


           if (unoccupiedTiles.Count > 0)
           {
               Tile randomTile = unoccupiedTiles[Random.Range(0, unoccupiedTiles.Count)];
               enemySpawn = tiles[new Vector2(randomTile.gridLocation.x, randomTile.gridLocation.y)].transform;
               Instantiate(enemyPrefab, enemySpawn);
           }
       }
   }


   void GenerateGrid()
   {
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
                   spawnedTile = Instantiate(doorPrefab, new Vector3(x, y), Quaternion.Euler(0f, 0f, 90f));
               }
               else if (hDoorTiles.Contains(tileLocation))
               {
                   spawnedTile = Instantiate(doorPrefab, new Vector3(x, y), Quaternion.identity);
               }
               else if (uStationTiles.Contains(tileLocation))
               {
                   spawnedTile = Instantiate(stationPrefab, new Vector3(x, y), Quaternion.identity);
               }
               else if (dStationTiles.Contains(tileLocation))
               {
                   spawnedTile = Instantiate(stationPrefab, new Vector3(x, y), Quaternion.Euler(0f, 0f, 180f));
               }
               else if (lStationTiles.Contains(tileLocation))
               {
                   spawnedTile = Instantiate(stationPrefab, new Vector3(x, y), Quaternion.Euler(0f, 0f, 90f));
               }
               else if (rStationTiles.Contains(tileLocation))
               {
                   spawnedTile = Instantiate(stationPrefab, new Vector3(x, y), Quaternion.Euler(0f, 0f, 270f));
               }
               else
               {
                   Sprite randomSprite = floorSprites[Random.Range(0, floorSprites.Length)];
                   spawnedTile = Instantiate(floorPrefab, new Vector3(x, y), Quaternion.identity);
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
       //cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
   }

   public void Patched(Tile tile)
    {
        tile.GetComponent<SpriteRenderer>().sprite = floorSprites[Random.Range(0, floorSprites.Length)];
        tile.tag = "Floor";
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
           }
       }
   }
}



