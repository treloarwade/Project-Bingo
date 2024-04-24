using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    public AudioClip song;
    private bool isSongPlaying = false;
    private static MusicManager instance;

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

    private void Start()
    {
        if (!isSongPlaying)
        {
            musicSource.clip = song;
            musicSource.Play();
            isSongPlaying = true;
        }
    }

}
