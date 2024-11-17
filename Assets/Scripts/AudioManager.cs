using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource musicSource;
    private AudioClip currentClip;
    public float defaultFadeDuration = 1.0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("AudioManager instance created.");
        }
        else
        {
            Debug.Log("Duplicate AudioManager detected. Destroying this instance.");
            Destroy(gameObject);
        }
    }
    
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
            return;
        }

        if (fadeDuration < 0f) fadeDuration = defaultFadeDuration;
        
        StartCoroutine(SwitchMusicWithFade(musicClip, fadeDuration));
    }

    public void StopMusic(float fadeDuration = -1f)
    {
        if (musicSource == null)
        {
            Debug.LogError("No AudioSource assigned to AudioManager!");
            return;
        }

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
            yield return StartCoroutine(FadeOut(fadeDuration));
        }
        
        currentClip = newClip;
        musicSource.clip = newClip;
        musicSource.Play();
        
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
        musicSource.volume = startVolume;
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

        musicSource.volume = 1f;
    }
}
