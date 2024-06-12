using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    public Text volumetext;
    public Text indextext;
    MusicManager musicManager;
    private const string MusicVolumeKey = "MusicVolume";

    private void Start()
    {
        GameObject audioObject = GameObject.FindGameObjectWithTag("Audio");
        musicManager = audioObject.GetComponent<MusicManager>();
            // Load the saved volume from PlayerPrefs
            if (PlayerPrefs.HasKey(MusicVolumeKey))
        {
            float savedVolume = PlayerPrefs.GetFloat(MusicVolumeKey);
            musicSlider.value = savedVolume;
            audioMixer.SetFloat("music", savedVolume);
            volumetext.text = "Volume: " + savedVolume;
        }
        else
        {
            // Set default volume if no value is saved
            float defaultVolume = musicSlider.value;
            audioMixer.SetFloat("music", defaultVolume);
            volumetext.text = "Volume: " + defaultVolume;
        }

        // Add listener to save volume whenever the slider value changes
        musicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("music", volume);
        volumetext.text = "Volume: " + volume;

        // Save the volume to PlayerPrefs
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
        PlayerPrefs.Save();
    }
    public void NextSong()
    {
        musicManager.PlayNextSong();
        indextext.text = musicManager.currentSongIndex.ToString();
    }
    public void PreviousSong()
    {
        musicManager.PlayPreviousSong();
        indextext.text = musicManager.currentSongIndex.ToString();
    }

}
