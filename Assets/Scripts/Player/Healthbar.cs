using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image[] hearts; // Array of heart images to represent health
    [SerializeField] private Sprite fullHeart; // Sprite for a full heart
    [SerializeField] private Sprite emptyHeart; // Sprite for an empty heart

    private void Start() 
    {
        // Initialize the health bar to full hearts at the start
        UpdateHealthBar(hearts.Length, hearts.Length);
    }
    
    public void UpdateHealthBar(float currentHealth, float startingHealth)
    {
        // Loop through each heart image
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                // If the heart index is within the current health, set it to full
                hearts[i].sprite = fullHeart;
            }
            else
            {
                // If the heart index exceeds current health, set it to empty
                hearts[i].sprite = emptyHeart;
            }
        }
    }
}