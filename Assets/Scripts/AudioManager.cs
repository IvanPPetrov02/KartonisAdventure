using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // Singleton instance

    public AudioSource musicSource; // Main AudioSource for background music
    private AudioClip currentClip; // Keep track of the currently playing clip
    public float defaultFadeDuration = 1.0f; // Default duration for fades

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Prevent destruction
            Debug.Log("AudioManager instance created.");
        }
        else
        {
            Debug.Log("Duplicate AudioManager detected. Destroying this instance.");
            Destroy(gameObject); // Destroy duplicates
        }
    }

    // Play music with optional fade
    public void PlayMusic(AudioClip musicClip, float fadeDuration = -1f)
    {
        if (musicClip == null)
        {
            Debug.LogWarning("Attempted to play a null music clip.");
            return;
        }

        if (musicSource == null)
        {
            Debug.LogError("No AudioSource assigned to AudioManager!");
            return;
        }

        if (currentClip == musicClip)
        {
            Debug.Log("Music already playing: " + musicClip.name);
            return; // If the same clip is already playing, do nothing
        }

        if (fadeDuration < 0f) fadeDuration = defaultFadeDuration;

        // Stop the current music with a fade-out and play the new music
        StartCoroutine(SwitchMusicWithFade(musicClip, fadeDuration));
    }

    public void StopMusic(float fadeDuration = -1f)
    {
        if (musicSource.isPlaying)
        {
            if (fadeDuration < 0f) fadeDuration = defaultFadeDuration;
            StartCoroutine(FadeOut(fadeDuration));
        }
    }

    private IEnumerator SwitchMusicWithFade(AudioClip newClip, float fadeDuration)
    {
        if (musicSource.isPlaying)
        {
            // Fade out the current music
            yield return StartCoroutine(FadeOut(fadeDuration));
        }

        // Switch to the new clip
        currentClip = newClip;
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in the new music
        yield return StartCoroutine(FadeIn(fadeDuration));
    }

    private IEnumerator FadeOut(float duration)
    {
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume; // Reset volume to the original value
    }

    private IEnumerator FadeIn(float duration)
    {
        float startVolume = 0f;
        musicSource.volume = 0f;

        while (musicSource.volume < 1f)
        {
            musicSource.volume += Time.deltaTime / duration;
            yield return null;
        }

        musicSource.volume = 1f; // Ensure it's set to full volume
    }
}
