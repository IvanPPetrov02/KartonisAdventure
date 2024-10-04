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

        // Debug: Check if the background music started
        Debug.Log("Background music started: " + audioSource.clip.name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered with: " + other.gameObject.name); // Log the object we collided with

        // Check if we collided with an object tagged as "Item" and if the item hasn't been picked up already
        if (other.CompareTag("Item") && !hasPickedUpItem)
        {
            Debug.Log("Item picked up: " + other.gameObject.name); // Confirm item was picked up

            hasPickedUpItem = true;

            // Destroy the item
            Destroy(other.gameObject);

            // Switch the music
            SwitchMusic();
        }
        else
        {
            Debug.Log("Triggered with something that is not the item or item already picked up.");
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

        // Debug: Check if the item pickup music started
        Debug.Log("Item pickup music started: " + audioSource.clip.name);
    }
}
