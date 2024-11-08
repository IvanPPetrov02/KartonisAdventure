using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void StartNewGame()
    {
        SceneManager.LoadScene("2nd level"); // Ensure your scene name is exactly "2nd level"
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit"); // This will log the message in the Unity Editor (won't show in a build)
    }
}