using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    [SerializeField] private float healthIncrease = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player collided with the heart pickup
        if (collision.CompareTag("Player"))
        {
            // Get the Health component from the player
            Health playerHealth = collision.GetComponent<Health>();

            // Only increase health if it's below the max (starting health)
            if (playerHealth != null && playerHealth.currentHealth < playerHealth.startingHealth)
            {
                // Use the IncreaseHealth method to add health
                playerHealth.IncreaseHealth(healthIncrease);

                // Destroy the heart pickup to prevent re-collection
                Destroy(gameObject);
            }
            // If health is already at max, do nothing (the heart stays)
        }
    }
}