using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;

    private void Start() 
    {
        // Set total health bar to be completely filled at the start
        totalHealthBar.fillAmount = 1f;
    }

    public void UpdateHealthBar(float currentHealth, float startingHealth)
    {
        currentHealthBar.fillAmount = currentHealth / startingHealth;
    }
}