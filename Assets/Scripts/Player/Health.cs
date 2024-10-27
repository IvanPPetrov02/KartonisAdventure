using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth = 3; 
    public float currentHealth { get; private set; }  

    private Animator anim;
    private bool isDead;
    private Healthbar healthbar; // Reference to Healthbar

    private void Awake()
    {
        currentHealth = startingHealth; 
        anim = GetComponent<Animator>(); 
        healthbar = FindObjectOfType<Healthbar>(); // Get reference to Healthbar
        healthbar.UpdateHealthBar(currentHealth, startingHealth); // Initialize healthbar
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("Hurt");
        }
        else if (!isDead)
        {
            Die();
        }

        healthbar?.UpdateHealthBar(currentHealth, startingHealth); // Update the health bar
    }

    private void Die()
    {
        isDead = true;
        anim.SetTrigger("Die");
        GetComponent<SpriteRenderer>().flipY = true;
        GetComponent<PlayerMovement>().enabled = false;
        Invoke("ReloadScene", 2f);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            TakeDamage(1);
        }
    }
}