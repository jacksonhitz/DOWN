using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

public class GameOverManager : MonoBehaviour
{
    public Slider bar; 
    public GameObject gameOverText; 
    public Button backButton; 
    public AudioSource gameMusic; 
    public float maxBarValue = 100f; 

    private bool isGameOver = false;

    void Start()
    {
        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }

        if (backButton != null)
        {
            backButton.gameObject.SetActive(false);
            backButton.onClick.AddListener(ReturnToMainMenu); 
        }
    }

    void Update()
    {
        if (bar != null && bar.value >= maxBarValue && !isGameOver)
        {
            TriggerGameOver();
        }
    }

    void TriggerGameOver()
    {
        isGameOver = true;

        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
        }

        if (backButton != null)
        {
            backButton.gameObject.SetActive(true);
        }

        Time.timeScale = 0f; 

        if (gameMusic != null)
        {
            gameMusic.Pause();
        }
    }

    void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
    }

    public void returnToMain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
