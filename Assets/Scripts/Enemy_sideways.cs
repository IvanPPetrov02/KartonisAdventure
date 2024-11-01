using UnityEngine;

public class Enemy_sideways : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float iframeDuration = 0.5f; // Duration of iFrame in seconds
    private float lastDamageTime;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();

            if (playerHealth != null && playerHealth.currentHealth > 0)
            {
                if (Time.time >= lastDamageTime + iframeDuration)
                {
                    playerHealth.TakeDamage(damage);
                    lastDamageTime = Time.time;
                }
            }
        }
    }
}