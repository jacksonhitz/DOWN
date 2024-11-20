using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public bool selected;
    private Pathfinder pathfinder;
    public List<Tile> path = new List<Tile>();
    [SerializeField] private Tile startTile;
    public Tile endTile;
    [SerializeField] private int speed;
    private GridManager gridManager;
    [SerializeField] private GameObject highlight;
    private Animator animator;
    public int health;
    public int currentHealth;

    // Start is called before the first frame update
    public void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        startTile = transform.parent.GetComponent<Tile>();
        startTile.isOccupied = true;
        pathfinder = new Pathfinder();
        animator = GetComponent<Animator>();
        FindCrew();
        currentHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        if (path.Count > 0)
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

        path = pathfinder.FindPath(startTile, endTile);

        if (startTile == endTile)
        {
            endTile = null;
        }
    }

    void FindCrew()
    {
        float closestDistance = Mathf.Infinity;
        BaseCrew closestCrew = null; 

        foreach (BaseCrew crew in GridManager.Instance.crew)
        {
            float distance = Vector3.Distance(transform.position, crew.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCrew = crew;
            }
        }

        if (closestCrew != null)
        {
            List<Tile> neighborTiles = pathfinder.GetNeighborTiles(closestCrew.transform.parent.GetComponent<Tile>());
            Tile nearestNeighbor = null;
            float closestNeighborDistance = Mathf.Infinity;

            foreach (Tile neighbor in neighborTiles)
            {
                float neighborDistance = Vector3.Distance(transform.position, neighbor.transform.position);
                if (neighborDistance < closestNeighborDistance)
                {
                    closestNeighborDistance = neighborDistance;
                    nearestNeighbor = neighbor;
                }
            }

            if (nearestNeighbor != null)
            {
                endTile = nearestNeighbor;
            }
        }
    }


    public void MoveAlongPath()
    {
        transform.position = Vector2.MoveTowards(transform.position, path[0].transform.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, path[0].transform.position) < 0.00001f)
        {
            FinishedMoving(path[0]);
            path.RemoveAt(0);
        }
    }

    public void FinishedMoving(Tile tile)
    {
        startTile = transform.parent.GetComponent<Tile>();
        transform.parent = path[0].transform;
    }

    public void TakeDamage(int dmg)
    {
        if (dmg > currentHealth)
        {
            Destroy(gameObject);
        }
        else
        {
            currentHealth -= dmg;
        }
    }
}
