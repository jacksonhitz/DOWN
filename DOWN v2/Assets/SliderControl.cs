using UnityEngine;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour
{
    public Slider volumeSlider;

    private void Start()
    {
        // Wait for VolumeSettings to initialize
        StartCoroutine(InitializeSlider());
    }

    private System.Collections.IEnumerator InitializeSlider()
    {
        // Wait until the VolumeSettings instance is ready
        while (VolumeSettings.Instance == null)
        {
            yield return null;
        }

        // Set the slider to match the saved or default volume
        volumeSlider.value = VolumeSettings.Instance.GetVolume();

        // Add listener to the slider
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float value)
    {
        if (VolumeSettings.Instance != null)
        {
            VolumeSettings.Instance.SetVolume(value);
            PlayerPrefs.SetFloat("Volume", value);
            PlayerPrefs.Save();
            Debug.Log($"Slider Value Changed: {value}");
        }
        else
        {
            Debug.LogError("VolumeSettings Instance is null!");
        }
    }
}
