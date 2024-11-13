using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] public float startingHealth = 3;
    public float currentHealth { get; private set; }
    [SerializeField] private float iframeDuration = 1f; // Duration of invincibility frames after taking damage

    private Animator anim;
    private bool isDead;
    private bool isInvincible;
    private float iframeTimer;
    private Collider2D colliderComponent;
    private bool isInDamageZone;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        colliderComponent = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            iframeTimer -= Time.deltaTime;
            if (iframeTimer <= 0)
            {
                isInvincible = false;
                EnableHitbox();
                if (isInDamageZone)
                {
                    TakeDamage(1);
                }
            }
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead || isInvincible) return;
        
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("Hurt");
            ActivateIframes();
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
    
    public void IncreaseHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, startingHealth);
        FindObjectOfType<Healthbar>()?.UpdateHealthBar(currentHealth, startingHealth);
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
/*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Spike") || collision.gameObject.CompareTag("Enemy"))
        {
            isInDamageZone = true;
            TakeDamage(1);
        }
    }
    

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Spike") || collision.gameObject.CompareTag("Enemy"))
        {
            isInDamageZone = false;
        }
    }
    */

    private void ActivateIframes()
    {
        isInvincible = true;
        iframeTimer = iframeDuration;
    }
    

    private void EnableHitbox()
    {
        if (colliderComponent != null)
        {
            colliderComponent.enabled = true;
        }
    }
}
