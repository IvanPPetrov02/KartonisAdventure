using UnityEngine;

public class MusicSwitcher : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the AudioSource playing the music
    public AudioClip backgroundMusic; // Background music clip
    public AudioClip itemPickupMusic; // Music that plays when item is picked up

    private bool hasPickedUpItem = false;

    void Start()
    {
        // Debug: Check if the AudioSource is assigned
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned!");
            return; // Stop execution if AudioSource is not assigned
        }

        // Start playing background music at the start
        audioSource.clip = backgroundMusic;
        audioSource.Play();
    }

private void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("Triggered with: " + other.gameObject.name); // Check if this log appears when the player touches the item
    if (other.CompareTag("Item") && !hasPickedUpItem)
    {
        hasPickedUpItem = true;
        Destroy(other.gameObject);
        SwitchMusic();
    }
}

    void SwitchMusic()
    {
        Debug.Log("Switching music to item pickup"); // Debug to check if the method is being called

        // Stop the current background music
        audioSource.Stop();

        // Switch to the new music clip and play it
        audioSource.clip = itemPickupMusic;
        audioSource.Play();
    }
}
