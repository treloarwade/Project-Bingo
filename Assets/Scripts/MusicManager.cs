using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    public List<AudioClip> songs = new List<AudioClip>();
    private int currentSongIndex = 0;
    private bool isSongPlaying = false;
    private static MusicManager instance;
    private bool isMuted = false;
    public DayAndNight dayandnight;

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
        if (songs.Count == 0)
        {
            Debug.LogWarning("No songs added to the list.");
            return;
        }

        currentSongIndex = (currentSongIndex + 1) % songs.Count;
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
