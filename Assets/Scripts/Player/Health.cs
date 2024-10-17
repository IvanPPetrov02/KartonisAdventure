using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }

    private Animator anim;
    private bool isDead;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("Hurt");
        }
        else
        {
            if (!isDead)
            {
                Die();
            }
        }

        // Update the health bar whenever health changes
        FindObjectOfType<Healthbar>().UpdateHealthBar(currentHealth, startingHealth);
    }

    private void Die()
    {
        isDead = true;

        // Play death animation or any other death-related behavior
        anim.SetTrigger("Die");

        // Optionally flip the player on their back
        GetComponent<SpriteRenderer>().flipY = true;

        // Disable player movement or other components
        GetComponent<PlayerMovement>().enabled = false;
    }

    // For testing purposes
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(1);
        }
    }
}