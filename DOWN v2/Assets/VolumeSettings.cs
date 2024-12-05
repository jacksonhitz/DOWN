using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    public static VolumeSettings Instance;

    private bool isInitialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this object across scenes
            Debug.Log("VolumeSettings Instance Created");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Duplicate VolumeSettings Instance Destroyed");
        }
    }

    private void Start()
    {
        if (!isInitialized)
        {
            float savedVolume = PlayerPrefs.GetFloat("Volume", 0.5f);
            ApplyVolumeToScene(savedVolume);
            isInitialized = true;
        }
    }

    public void SetVolume(float volume)
    {
        // Save the volume setting
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();

        // Apply the volume setting to the current scene
        ApplyVolumeToScene(volume);
        Debug.Log($"Volume Set To: {volume}");
    }

    public float GetVolume()
    {
        // Retrieve the saved volume
        return PlayerPrefs.GetFloat("Volume", 0.5f);
    }

    private void ApplyVolumeToScene(float volume)
    {
        // Find all AudioSources in the current scene and apply the volume
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in audioSources)
        {
            source.volume = volume;
        }

        Debug.Log($"Applied Volume To {audioSources.Length} AudioSources in Scene");
    }
}
