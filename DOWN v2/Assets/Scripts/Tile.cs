using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    [SerializeField] private GameObject selected;
    [SerializeField] private Tile tile;
    [SerializeField] private bool door;
    [SerializeField] private bool station;
    [SerializeField] private bool setOpen;

    public bool isOccupied;
    public Tile previous;
    public int G;
    public int H;
    public int F { get { return G + H; } }
    public Vector2Int gridLocation;

    private Animator animator;
    private GridManager gridManager;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        animator = GetComponent<Animator>();

    }

    private void Update()
    {
        if (!door && !station)
        {
            if (transform.childCount > 3)
            {
                isOccupied = true;
            }
            else
            {
                isOccupied = false;
            }
        }
    }

    private void OnMouseEnter()
    {
        highlight.SetActive(true);
    }
    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }
    private void OnMouseDown()
    {
        if (door)
        {
            animator.SetBool("Open", !animator.GetBool("Open"));

            if (animator.GetBool("Open") == true)
            {
                setOpen = true;
            }
            else
            {
                setOpen = false;
            }

        }
        else if (station)
        {

        }
        else
        {
            foreach (BaseCrew crew in GridManager.Instance.crew)
            {
                if (crew.selected)
                {
                    crew.endTile = this;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (door && !setOpen)
        {
            if (collider.CompareTag("Crew"))
            {
                animator.SetBool("Open", true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (door && !setOpen)
        {
            if (collider.CompareTag("Crew"))
            {
                animator.SetBool("Open", false);
            }
        }
    }
}

 