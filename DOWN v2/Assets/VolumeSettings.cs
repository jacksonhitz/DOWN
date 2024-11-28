using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    public static VolumeSettings Instance;

    public AudioSource backGroundMusic;
    public AudioSource shipDrillSound;
    private bool isInitialized = false;

    private void Start()
    {
        if (!isInitialized)
        {
            float savedVolume = PlayerPrefs.GetFloat("Volume", 0.5f);
            SetVolume(savedVolume);
            isInitialized = true;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("VolumeSettings Instance Created");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Duplicate VolumeSettings Instance Destroyed");
        }
    }


    public void SetVolume(float volume)
    {
        if (backGroundMusic != null)
        {
            backGroundMusic.volume = volume;
            Debug.Log($"Background Music Volume Set To: {volume}");
        }
        else
        {
            Debug.LogError("BackGroundMusic is null!");
        }

        if (shipDrillSound != null)
        {
            shipDrillSound.volume = volume;
            Debug.Log($"Ship Drill Sound Volume Set To: {volume}");
        }
        else
        {
            Debug.LogError("ShipDrillSound is null!");
        }
    }


    public float GetVolume()
    {
        // Return the volume of the first AudioSource, assuming both are the same
        return backGroundMusic != null ? backGroundMusic.volume : 0.5f;
    }
}
