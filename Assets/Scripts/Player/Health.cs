using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] public float startingHealth = 3; // Player starts with 3 health
    public float currentHealth { get; private set; }  // Exposed property for current health

    private Animator anim;
    private bool isDead;

    private void Awake()
    {
        currentHealth = startingHealth;  // Set current health to starting health
        anim = GetComponent<Animator>(); // Get Animator component for triggering animations
    }

    // Method to take damage
    public void TakeDamage(float damage)
    {
        if (isDead) return;  // Do nothing if the player is dead

        // Reduce current health, but not below 0
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

        FindObjectOfType<Healthbar>()?.UpdateHealthBar(currentHealth, startingHealth);
    }

    // Method to increase health
    public void IncreaseHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, startingHealth);
        FindObjectOfType<Healthbar>()?.UpdateHealthBar(currentHealth, startingHealth);
    }

    // Death handling
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