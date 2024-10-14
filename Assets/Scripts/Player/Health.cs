using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]private float startingHealth;
    public float currentHealth {get; private set;}
    private Animator anim;
    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
    }
    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if(currentHealth > 0)
        {
            anim.SetTrigger("Hurt");
        }
        else
        {
        // Disable the animation trigger for death
        // anim.SetTrigger("die");

        // Flip the player on their back
        // This could be done by rotating or flipping the sprite
        GetComponent<SpriteRenderer>().flipY = true;  // Flips the sprite vertically
        
        // Optionally disable player movement or other components as needed
        GetComponent<PlayerMovement>().enabled = false;
        }
    }
    //for testing
    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.E))
            TakeDamage(1);
    }
}
