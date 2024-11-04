using UnityEngine;
using UnityEngine.UI;

public class CharacterDialogue : MonoBehaviour
{
    public string dialogueMessage = "Dad, you are awake!"; // The default message
    public GameObject dialogueUI; // UI element that displays the dialogue message
    public GameObject listenButtonUI; // UI button for the "Listen" option

    private bool playerInRange = false;

    void Start()
    {
        if (dialogueUI != null)
            dialogueUI.SetActive(false); // Hide dialogue UI initially
        if (listenButtonUI != null)
            listenButtonUI.SetActive(false); // Hide listen button initially
    }

    void Update()
    {
        if (playerInRange)
        {
            dialogueUI.SetActive(true); // Show dialogue message when player is in range

            if (Input.GetKeyDown(KeyCode.E)) // Press 'E' to listen
            {
                ListenToCharacter();
            }
        }
        else
        {
            dialogueUI.SetActive(false);
            listenButtonUI.SetActive(false);
        }
    }

    private void ListenToCharacter()
    {
        listenButtonUI.SetActive(true); // Show the full interaction UI
        dialogueUI.GetComponent<Text>().text = dialogueMessage;
        // Additional logic can go here, like playing an audio clip or showing more dialogue options.
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            listenButtonUI.SetActive(false); // Hide listen button when out of range
        }
    }
}
