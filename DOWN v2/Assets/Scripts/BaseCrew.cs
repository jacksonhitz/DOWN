using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BaseCrew : MonoBehaviour
{
   public bool selected;
   public Pathfinder pathfinder;
   public List<Tile> path = new List<Tile>();
   public Tile startTile;
   public Tile endTile;
   [SerializeField] private float speed;
   private GridManager gridManager;
   private GameObject highlight;
   private Animator animator;
   public int health;

    public bool fixing;

   [SerializeField] Slider healthBar;
   [SerializeField] Slider progressBar;
   [SerializeField] float fillSpeed = 0.001f;
   float targetProgress = 1f;


   // Start is called before the first frame update
   public void Start()
   {
       gridManager = FindObjectOfType<GridManager>();
       startTile = transform.parent.GetComponent<Tile>();
       startTile.isOccupied = true;
       pathfinder = new Pathfinder();
       gridManager.crew.Add(this);
       animator = GetComponent<Animator>();
       highlight = transform.Find("Highlight").gameObject;


       StationCheck();
   }




   void Update()
   {
       if (path.Count > 0 && selected)
       {
           MoveAlongPath();
           Vector2 currentPosition = transform.position;
           Vector2 targetPosition = path[0].transform.position;
           Vector2 movement = (targetPosition - currentPosition).normalized;
           animator.SetBool("Moving", true);
           animator.SetFloat("Horizontal", movement.x);
           animator.SetFloat("Vertical", movement.y);
       }
       else
       {
           animator.SetBool("Moving", false);
       }


       if (selected)
       {
           highlight.SetActive(true);
       }
       else
       {
           highlight.SetActive(false);
       }


       path = pathfinder.FindPath(startTile, endTile);


       if (startTile == endTile)
       {
           endTile = null;
       }

       //EnemyCheck();


        if (fixing)
        {
            progressBar.gameObject.SetActive(true);
            
            if (progressBar.value < targetProgress)
            {
                progressBar.value += fillSpeed * Time.deltaTime;
            }
            else
            {
                fixing = false; 
                progressBar.value = 0f; 
                progressBar.gameObject.SetActive(false); 
                gridManager.Patched(startTile);
                animator.SetBool("Moving", false);
                animator.Play("CrewIdle");
            }
        }
   }


   public void OnMouseDown()
   {
       foreach (BaseCrew crew in gridManager.crew)
       {
           crew.selected = false;
       }
       selected = true;
       gridManager.moveable = true;
   }


   public void MoveAlongPath()
   {
       transform.position = Vector2.MoveTowards(transform.position, path[0].transform.position, speed * Time.deltaTime);


       if (Vector2.Distance(transform.position, path[0].transform.position) < 0.00001f)
       {
           FinishedMoving(path[0]);
           //path.RemoveAt(0);
       }
   }




   public void FinishedMoving(Tile tile)
   {
       startTile = tile;
       transform.parent = tile.transform;

       StationCheck();
       BreachCheck();
   }


   void BreachCheck()
   {
       if (startTile.CompareTag("Breach"))
       {
           animator.SetTrigger("DStationIdle");
           Debug.Log("On Breach");
           fixing = true;
       }
   }


   void StationCheck()
   {
       Vector2Int crewPosition = startTile.gridLocation;


       Vector2Int[] adjacentTileOffsets =
       {
           new Vector2Int(0, 1),   // Above
           new Vector2Int(0, -1),  // Below
           new Vector2Int(-1, 0),  // Left
           new Vector2Int(1, 0)    // Right
       };


       if (CheckStationTiles(GridManager.Instance.uStationTiles, crewPosition, adjacentTileOffsets))
       {
           animator.SetTrigger("UStationIdle");
       }
       else if (CheckStationTiles(GridManager.Instance.dStationTiles, crewPosition, adjacentTileOffsets))
       {
           animator.SetTrigger("DStationIdle");
       }
       else if (CheckStationTiles(GridManager.Instance.lStationTiles, crewPosition, adjacentTileOffsets))
       {
           animator.SetTrigger("LStationIdle");
       }
       else if (CheckStationTiles(GridManager.Instance.rStationTiles, crewPosition, adjacentTileOffsets))
       {
           animator.SetTrigger("RStationIdle");
       }
   }


   void EnemyCheck()
   {
       Vector2Int crewPosition = startTile.gridLocation;


       Vector2Int[] adjacentTileOffsets =
       {
           new Vector2Int(0, 1),   // Above
           new Vector2Int(0, -1),  // Below
           new Vector2Int(-1, 0),  // Left
           new Vector2Int(1, 0)    // Right
       };


       if (CheckForEnemy(adjacentTileOffsets[0]))
       {
           animator.SetTrigger("UAttack");
       }
       else if (CheckForEnemy(adjacentTileOffsets[1]))
       {
           animator.SetTrigger("DAttack");
       }
       else if (CheckForEnemy(adjacentTileOffsets[2]))
       {
           animator.SetTrigger("LAttack");
       }
       else if (CheckForEnemy(adjacentTileOffsets[3]))
       {
           animator.SetTrigger("RAttack");
       }
   }




   private bool CheckStationTiles(List<Vector2Int> stationTiles, Vector2Int crewPosition, Vector2Int[] adjacentTileOffsets)
   {
       foreach (Vector2Int offset in adjacentTileOffsets)
       {
           Vector2Int adjacentTileLocation = crewPosition + offset;


           if (stationTiles.Contains(adjacentTileLocation))
           {
               return true;
           }
       }
       return false;
   }


   private bool CheckForEnemy(Vector2Int offset)
   {
       Vector2Int adjacentTileLocation = startTile.gridLocation + offset;


       if (GridManager.Instance.tiles.ContainsKey(adjacentTileLocation))
       {
           Tile adjacentTile = GridManager.Instance.tiles[adjacentTileLocation];
           Transform[] childTransforms = adjacentTile.GetComponentsInChildren<Transform>();


           foreach (Transform childTransform in childTransforms)
           {
               BaseEnemy baseEnemy = childTransform.GetComponent<BaseEnemy>();
               baseEnemy.TakeDamage(10);
               if (childTransform.CompareTag("Enemy"))
               {
                   return true;
               }
           }
       }
       return false;
   }
}



