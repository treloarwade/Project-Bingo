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

}
