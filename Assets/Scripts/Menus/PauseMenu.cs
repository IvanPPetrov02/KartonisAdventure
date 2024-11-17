using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Drag the PauseMenu Panel here in the Inspector
    private bool isPaused = false;

    void Update()
    {
        // Toggle pause menu with Esc key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f; // Resume the game
        isPaused = false;
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true); // Show the pause menu
        Time.timeScale = 0f; // Pause the game
        isPaused = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume time

        // Destroy the AbilityManager to ensure it is reinitialized on scene reload
        if (AbilityManager.Instance != null)
        {
            Destroy(AbilityManager.Instance.gameObject);
        }

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f; // Resume time

        // Stop the current music
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();
        }

        // Destroy the AbilityManager if transitioning to a completely different scene
        if (AbilityManager.Instance != null)
        {
            Destroy(AbilityManager.Instance.gameObject);
        }

        // Load the main menu scene
        SceneManager.LoadScene("Main menu"); // Replace with your actual main menu scene name
    }
}