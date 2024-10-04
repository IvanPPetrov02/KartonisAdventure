using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // Singleton instance

    public AudioSource musicSource; // Main AudioSource for background music

    private void Awake()
    {
        // Implement singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Ensure the AudioManager persists across scenes
        }
        else
        {
            Destroy(gameObject); // If an instance already exists, destroy this duplicate
        }
    }

    // Play music (background music)
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource.clip != musicClip)
        {
            musicSource.Stop(); // Stop current music if different
            musicSource.clip = musicClip;
            musicSource.Play();
            Debug.Log("Now playing: " + musicClip.name); // Display currently playing music in Console
        }
    }
}