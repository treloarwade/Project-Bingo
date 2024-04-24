using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbiterScript : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    public AudioClip song;
    public AudioClip song2;
    public AudioClip song3;
    private bool isSongPlaying = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if music is already playing
        if (!musicSource.isPlaying)
        {
            isSongPlaying = false;
        }
        if (!isSongPlaying)
        {
            // Generate a random number to choose between song and song2
            int randomIndex = Random.Range(0, 3);

            switch (randomIndex)
            {
                case 0:
                    musicSource.clip = song;
                    break;
                case 1:
                    musicSource.clip = song2;
                    break;
                case 2:
                    musicSource.clip = song3;
                    break;
                default:
                    Debug.LogError("Invalid random index for song selection!");
                    break;
            }
            // Play the selected music clip
            musicSource.Play();
            isSongPlaying = true;
        }
    }

}
