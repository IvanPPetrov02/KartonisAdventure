using UnityEngine;

public class MusicSwitcher : MonoBehaviour
{
    public AudioClip AreaMusic;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SwitchMusic();
        }
    }

    private void SwitchMusic()
    {
        if (AudioManager.instance != null && AreaMusic != null)
        {
            AudioManager.instance.PlayMusic(AreaMusic);
        }
        else
        {
            Debug.LogError("Cannot switch music. Missing AudioManager or itemPickupMusic.");
        }
    }
}