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
    private GameObject highlight;
    private Animator animator;
    public float health;

    public bool fixing;

    [SerializeField] Slider healthBar;
    [SerializeField] Slider progressBar;
    [SerializeField] float fillSpeed = 0.001f;
    float targetProgress = 1f;

    GridManager gridManager;

    public float damageInterval = 1f;
    private float nextDamageTime;

    public void Start()
    {
        startTile = transform.parent.GetComponent<Tile>();
        startTile.isOccupied = true;
        pathfinder = new Pathfinder();
        GridManager.Instance.crew.Add(this);
        animator = GetComponent<Animator>();
        highlight = transform.Find("Highlight").gameObject;

        gridManager = FindObjectOfType<GridManager>();

        StationCheck();
        nextDamageTime = Time.time; 
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

        highlight.SetActive(selected);

        path = pathfinder.FindPath(startTile, endTile);

        if (startTile == endTile)
        {
            endTile = null;
        }

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

        // Apply damage based on pressure if standing on a tile with pressure
        ApplyPressureDamage();
    }

    public void OnMouseDown()
    {
        foreach (BaseCrew crew in GridManager.Instance.crew)
        {
            crew.selected = false;
        }
        selected = true;
    }

    public void MoveAlongPath()
    {
        transform.position = Vector2.MoveTowards(transform.position, path[0].transform.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, path[0].transform.position) < 0.00001f)
        {
            FinishedMoving(path[0]);
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
    void ApplyPressureDamage()
    {
        if (startTile != null)
        {
            if (Time.time >= nextDamageTime)
            {
                nextDamageTime = Time.time + damageInterval; 
                health -= startTile.pressure / 5; 
                health = Mathf.Max(health, 0); 
                healthBar.value = health / 100f; 
            }
        }
    }
}
