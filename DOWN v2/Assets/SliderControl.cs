using UnityEngine;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour
{
    public Slider volumeSlider;

    private void Start()
    {
        // Initialize the slider value and add a listener for value changes
        InitializeSlider();
    }

    private void InitializeSlider()
    {
        if (VolumeSettings.Instance != null)
        {
            // Set the slider to the saved volume value
            volumeSlider.value = VolumeSettings.Instance.GetVolume();

            // Add listener to handle slider value changes
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

            Debug.Log("Slider initialized with volume value: " + volumeSlider.value);
        }
        else
        {
            Debug.LogError("VolumeSettings Instance is null! Ensure VolumeSettings is properly loaded.");
        }
    }

    private void OnVolumeChanged(float value)
    {
        if (VolumeSettings.Instance != null)
        {
            // Update the volume in VolumeSettings
            VolumeSettings.Instance.SetVolume(value);

            Debug.Log($"Volume updated via slider to: {value}");
        }
        else
        {
            Debug.LogError("VolumeSettings Instance is null during volume change!");
        }
    }
}
