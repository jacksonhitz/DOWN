using UnityEngine;
using UnityEngine.UI;

public class PressureBarController : MonoBehaviour
{
    public static PressureBarController Instance { get; private set; }

    [SerializeField] private Slider pressureBar;
    [SerializeField] private Image fillImage; // Reference to the Fill's Image component

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateBar(float normalizedPressure)
    {
        if (pressureBar != null)
        {
            pressureBar.value = normalizedPressure;

            if (fillImage != null)
            {
                // Change the fill color based on pressure
                fillImage.color = Color.Lerp(Color.green, Color.red, normalizedPressure);
            }
            else
            {
                Debug.LogError("Fill Image is not assigned!");
            }

            Debug.Log($"Pressure Bar Updated: {normalizedPressure * 100}%");
        }
        else
        {
            Debug.LogError("Pressure Bar Slider is not assigned!");
        }
    }
}
