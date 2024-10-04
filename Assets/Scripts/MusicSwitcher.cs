using UnityEngine;

public class MusicSwitcher : MonoBehaviour
{
    public AudioClip itemPickupMusic; // The specific music for this item

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered with: " + other.gameObject.name); // Log the object we collided with

        // Check if the player collided with this item
        if (other.CompareTag("Player"))
        {
            Debug.Log("Item picked up: " + gameObject.name); // Confirm item was picked up

            // Destroy this item
            Destroy(gameObject);

            // Switch to this item's music using the AudioManager
            SwitchMusic();
        }
    }

    private void SwitchMusic()
    {
        Debug.Log("Switching music to item pickup"); // Debug to check if the method is being called

        // Use AudioManager to switch to the new music
        if (AudioManager.instance != null && itemPickupMusic != null)
        {
            AudioManager.instance.PlayMusic(itemPickupMusic);
        }
        else
        {
            Debug.LogError("Cannot switch music. Missing AudioManager or itemPickupMusic.");
        }
    }
}