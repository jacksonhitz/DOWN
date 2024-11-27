using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    [SerializeField] private GameObject selected;
    [SerializeField] private Tile tile;
    [SerializeField] private bool door;
    [SerializeField] private bool station;

    public bool open;

    public bool isOccupied;
    public Tile previous;
    public int G;
    public int H;
    public int F { get { return G + H; } }
    public Vector2Int gridLocation;

    private Animator animator;
    private GridManager gridManager;

    public float pressure = 0f;
    [SerializeField] private float maxPressure = 100.0f;

    private AudioSource doorOpenAudio; // door open sound

    private SpriteRenderer spriteRenderer; // sprite renderer

    private bool soundPlayed = false; // track if the sound has been played

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        animator = GetComponent<Animator>();

        doorOpenAudio = GetComponent<AudioSource>();  // get audio source component
        spriteRenderer = GetComponent<SpriteRenderer>(); // get sprite renderer component

        if (!door && !station)
        {
            open = true;
        }
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
        DecreasePressure(Time.deltaTime);
    }


    public void IncreasePressure(float amount)
    {
        // check if the sprite in the sprite renderer is named "Breach1"
        if (spriteRenderer.sprite != null && spriteRenderer.sprite.name == "Breach1")
        {
            if (!soundPlayed)  // only play sound if it's the first time
            {
                doorOpenAudio.Play();
                soundPlayed = true;  // mark the sound as played
            }
        }
        else
        {
            soundPlayed = false;  // reset the ssound if sprite is not "Breach1" anymore
        }

        pressure = Mathf.Min(pressure + amount * 100, maxPressure);
        UpdateColor();

    }
    public void DecreasePressure(float amount)
    {

        if (pressure > 0)
        {
            pressure -= (amount * 5);
        }
        UpdateColor();
    }

    private void UpdateColor()
    {
        float pressurePercentage = Mathf.Clamp01(pressure / maxPressure);
        Color tileColor = Color.Lerp(Color.white, Color.red, pressurePercentage);
        GetComponent<SpriteRenderer>().color = tileColor;
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

            if (animator.GetBool("Open"))
            {
                open = true;
                doorOpenAudio.Play();
            }
            else
            {
                open = false;
            }
        }
        else if (station)
        {
            // Station-specific logic if required
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
        if (door && !open)
        {
            if (collider.CompareTag("Crew"))
            {
                animator.SetBool("Open", true);

                doorOpenAudio.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (door && !open)
        {
            if (collider.CompareTag("Crew"))
            {
                animator.SetBool("Open", false);

            }
        }
    }
}