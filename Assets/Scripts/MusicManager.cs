using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    public List<AudioClip> songs = new List<AudioClip>();
    public int currentSongIndex = 0;
    private bool isSongPlaying = false;
    private static MusicManager instance;
    private bool isMuted = false;
    public DayAndNight dayandnight;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    public Text volumetext;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        musicSource.outputAudioMixerGroup = musicMixerGroup;
    }
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("music", volume);
        volumetext.text = "Volume:" + volume;
    }
    public void ToggleMute()
    {
        if (isMuted)
        {
            // If currently muted, set volume to normalVolume
            musicSource.volume = 0.16f;
        }
        else
        {
            // If not currently muted, set volume to mutedVolume
            musicSource.volume = 0;
        }

        // Toggle the state
        isMuted = !isMuted;
    }

    private void Start()
    {
        if (!isSongPlaying)
        {
            musicSource.clip = songs[0];
            musicSource.Play();
            isSongPlaying = true;
        }
    }
    public void PlayNextSong()
    {
        currentSongIndex = (currentSongIndex + 1) % songs.Count;
        PlaySong(currentSongIndex);
    }
    public void PlayPreviousSong()
    {
        // Subtract 1 from currentSongIndex
        currentSongIndex--;
        // If currentSongIndex goes below 0, wrap around to the last song
        if (currentSongIndex < 0)
        {
            currentSongIndex = songs.Count - 1;
        }

        PlaySong(currentSongIndex);
    }

    private void PlaySong(int index)
    {
        musicSource.Stop();
        musicSource.clip = songs[index];
        musicSource.Play();
    }
    public void PlaySongDayNNite()
    {
        // Check if the song at index 1 is already playing
        if (musicSource.clip == songs[1] && musicSource.isPlaying)
        {
            Debug.Log("Song is already playing.");
            return; // Exit if the song is already playing
        }

        // Find the DayAndNight component in the scene
        dayandnight = FindObjectOfType<DayAndNight>();

        if (dayandnight != null)
        {
            // Check the isNight boolean value
            if (dayandnight.isNight)
            {
                musicSource.Stop();
                musicSource.clip = songs[1];
                musicSource.Play();
            }
        }
    }

}
